using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Wima.Log;
using Wima.Lora.Model;

namespace Wima.Lora
{
    public class ClientServer : LoraServerBase
    {

        /// <summary>
        /// 服务器角色
        /// </summary>
        public override ServerRole ServerRole { get => ServerRole.CSEnabled; }


        /// <summary>
        /// 构造函数
        /// </summary>
        public ClientServer()
        {
            LogMan.Info("ClientServer开始初始化……");
            LoadConfig("cs.config");
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
        public override void Start()
        {
            if (IsRunning) return;
            LogMan.Info("ClientServer开始启动……");

            try
            {
                WebServerCert = new X509Certificate2(WebServerPfxCertInfo.PfxFile, WebServerPfxCertInfo.Key);
                CaCert = X509Certificate.CreateFromCertFile(CaCertName); //注意证书文件不存在会导致相关Man初始化失败
            }
            catch (Exception ex) { LogMan.Error("加载证书时出错！", ex); }


            if (this.ServerRole.HasFlag(ServerRole.CSEnabled))
            {
                IPEndPoint ipEp = MyCsHttpEp.ToIpEndPoint();
                if (ipEp != null) ManList.Add(new HTTPMan(this, MyCsEUI, ipEp) { AssociatedServerRole = ServerRole.CSEnabled });
                ManList.Add(new UDPMan(this, MyCsEUI, MyCsUdpEp.FirstOrDefault()) { AssociatedServerRole = ServerRole.CSEnabled });
                AppLayer.PurifierMan = new SignalRMan(this);
            }

            //启动通讯代理
            ManList.AsParallel().ForAll(i => i.Start());

            base.Start();
        }


        /// <summary>
        /// 处理来自AS服务器的包
        /// </summary>
        /// <param name="inPackage"></param>
        public override void ProcessLoraPackage(ASCSPackage inPackage)
        {
            if (inPackage == null) return;
            CSASPackage outPackage = null;
            MoteEntry mote = null;

            if (!inPackage.IsOutbound || inPackage.IsInternal)
            {
                //CS处理分支：
                //Join
                if (inPackage.mote?.join?.appeui != null)
                {
                    mote = new MoteEntry()
                    {
                        DevEUI = Utils.Hex2Bytes(inPackage.mote.eui)
                    };
                    CsMoteEntries.AddOrUpdate(mote);
                    //TODO：CS处理入网模拟

                }
                //Upstream 
                else if (inPackage.app?.moteeui != null && inPackage.app?.userdata?.payload != null)
                {

                    if (CsMoteEntries.TryGetMoteByEui(inPackage.app.moteeui, out mote))
                    {
                        mote.LastRcvServerTime = DateTime.Now;
                        //生成出站包
                        outPackage = MakeCSASPackage(mote.DevEUIStr, AppLayer.ProcessAppPld(inPackage, mote, Utils.FromLoraBase64Str(inPackage.app.userdata.payload)));
                    }
                }

                //将包放入处理池处理
                AddToPool(outPackage, inPackage.SId, mote);
            }
            inPackage.SetProcessed();
        }


    }
}
