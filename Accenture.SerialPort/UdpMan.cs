using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using Wima.Lora.Model;
using Wima.Lora.NPLinkCompatibility;
using Wima.Lora;
using Wima.Log;

namespace Accenture.SerialPort
{
    public class UdpMan
    {
        public static LogMan log = new LogMan("LoRaCS");
        /// <summary>
        /// 显示推送委托事件
        /// </summary>
        /// <param name="package"></param>
        public delegate void ShowDataHandle(ASCSPackage package);
        public event ShowDataHandle ShowEvent;
        public UdpClient UdpServer { get; set; }
        private DnsEndPoint Ep { get; set; } = new DnsEndPoint("127.0.0.1", 1234);
        public DateTime BeatRcvTime { get; set; }
        private bool IsRuning = false;
        private string appeui;
        private bool IsLoad = true;

        public UdpMan(DnsEndPoint endPoint, string appeui, bool isload = true)
        {
            if (isload)
                Ep = endPoint;
            this.appeui = appeui;
            IsLoad = isload;
        }
        public void Start()
        {
            try
            {
                UdpServer = new UdpClient(30000);
            }
            catch (Exception ex)
            {
                throw new Exception("UdpServer初始化失败，原因：" + ex.Message);
            }
            SendLoginBeatSend();
            IPEndPoint tbip = null;
            if (IsLoad)
                try
                {
                    UdpServer.Client.ReceiveTimeout = 5 * 1000;
                    byte[] dataLogin = UdpServer.Receive(ref tbip);
                    LoginBeatRec BeatRec = ProcessJsonLogin(dataLogin.ToASCIIString());
                    if (BeatRec != null && BeatRec.las != null && BeatRec.las.login_success)
                    { }
                    else throw new Exception("服务器拒绝登陆，登陆失败");
                }
                catch { throw new Exception("服务器无响应登陆失败，Time=" + UdpServer.Client.ReceiveTimeout + "MS"); }
            UdpServer.Client.ReceiveTimeout = 5 * 60 * 1000;
            IsRuning = true;
            Task.Run(() =>
            {
                while (IsRuning)
                {
                    try
                    {
                        IPEndPoint ip = null;
                        byte[] data = UdpServer.Receive(ref ip);
                        if (!IsLoad) Ep = ip.ToDnsEndPoint();
                        if (data.Length > 0)
                        {
                            if (data[2] == 'a' && data[3] == 's')
                            {
                                LoginBeatRec nas = ProcessJsonLogin(data.ToASCIIString());
                                if (nas?.las != null && nas.las.login_success)
                                    BeatRcvTime = DateTime.Now;
                            }
                            else ProcessLoraPackage(data);
                        }
                    }
                    catch (Exception ex) { log.Error(ex); }
                }
            });
        }
        public void Stop()
        {
            IsRuning = false;
            UdpServer.Close();
            UdpServer.Dispose();
            UdpServer = null;
        }
        public bool Send(byte[] data)
        {
            try
            {
                UdpServer.Send(data, data.Length, Ep.ToIpEndPoint());
            }
            catch (Exception ex) { log.Error(ex); return false; }
            return true;
        }
        /// <summary>
        /// 把收到的数据处理成字符串
        /// </summary>
        /// <param name="data"></param>
        public void ProcessLoraPackage(byte[] data)
        {
            ProcessJsonData(Encoding.ASCII.GetString(data));
        }
        /// <summary>
        /// 处理json数据
        /// </summary>
        /// <param name="jsText"></param>
        public void ProcessJsonData(string jsText)
        {
            ASCSPackage jsObject = default(ASCSPackage);
            if (!string.IsNullOrEmpty(jsText))
            {
                try
                {
                    jsObject = JObject.Parse(jsText).ToObject<ASCSPackage>();
                    if (jsObject != null && jsObject is ASCSPackage) ShowEvent?.BeginInvoke(jsObject as ASCSPackage, null, null);
                }
                catch (Exception ex) { log.Error(ex); }
            }
        }
        /// <summary>
        /// 处理登陆包
        /// </summary>
        /// <param name="jsText"></param>
        /// <returns></returns>
        public LoginBeatRec ProcessJsonLogin(string jsText)
        {
            LoginBeatRec jsObject = default(LoginBeatRec);
            if (!string.IsNullOrEmpty(jsText))
            {
                try
                {
                    jsObject = JObject.Parse(jsText.Insert(2, "l")).ToObject<LoginBeatRec>();
                    return jsObject;
                }
                catch (Exception ex) { log.Error(ex); }
            }
            return null;
        }
        /// <summary>
        /// 发送登陆包
        /// </summary>
        /// <returns></returns>
        public bool SendLoginBeatSend()
        {
            LoginBeatSend data = new LoginBeatSend()
            {
                las = new LoginAsSend()
                {
                    app_id = appeui,
                    login_name = "DTU",
                    login_pwd = "DTU",
                    login_type = 1,
                    token = 1
                }
            };
            return Send(Encoding.ASCII.GetBytes(data.ToCompactJsonString().Remove(2, 1)));
        }
        /// <summary>
        /// 发送下行包
        /// </summary>
        /// <param name="deveui"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public bool SendPacket(string deveui, string Data)
        {
            CSASPackage outPackage = new CSASPackage()
            {
                app = new App()
                {
                    moteeui = deveui,
                    token = (ushort)new Random().Next(1, 65535),
                    dir = App.DirectionString.Down,
                    userdata = new Userdata()
                    {
                        port = 3,
                        payload = Encoding.ASCII.GetBytes(Data).ToLoraBase64Str()

                    },
                }
            };
            return Send(outPackage.ToCompactJsonByteArray());
        }
        public bool SendPacket(string deveui, byte[] Data)
        {
            CSASPackage outPackage = new CSASPackage()
            {
                app = new App()
                {
                    moteeui = deveui,
                    token = (ushort)new Random().Next(1, 65535),
                    dir = App.DirectionString.Down,
                    userdata = new Userdata()
                    {
                        port = 3,
                        payload = Data.ToLoraBase64Str()
                    },
                }
            };
            return Send(outPackage.ToCompactJsonByteArray());
        }
    }
}
