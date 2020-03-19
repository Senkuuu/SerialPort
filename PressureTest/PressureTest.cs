using System;
using System.Net;
using Wima.Lora.Model;
using Wima.Lora;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Wima.Lora.NPLinkCompatibility;
using Newtonsoft.Json;
using System.Text;
using System.Threading;

namespace PressureTest
{
    public class PressureTest
    {
        private static DateTime goTime;

        private static bool fristSend = false;

        private static NpcUdpMan udp = new NpcUdpMan(new Server(), "", new DnsEndPoint("0.0.0.0", 1701));
        public static void Main(string[] args)
        {
            goTime = DateTime.Now;
            Console.WriteLine("按 '7' 开始测试");
            if (Console.ReadLine() == "7")
            {
                TestStart();
            }
        }

        /// <summary>
        /// 字符串转换16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>

        private static byte[] strToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0) hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Replace(" ", ""), 16);
            return returnBytes;
        }

        #region 压力测试udp下发
        /// <summary>
        /// 
        /// </summary>
        private static void TestStart()
        {
            //压力测试

            //NpcUdpMan udp = new NpcUdpMan(new Server(), "", new DnsEndPoint("0.0.0.0", 1701));
            udp.Start();

            while (true) { Thread.Sleep(5000); };

        }

        private static void sendGo(DnsEndPoint dep)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    string information = Utils.ToLoraBase64Str(strToHexByte("fe0d0a2500000150c00011200121ffffffff5e0b78b50000001e20000000000007590d0aef"));
                    for (int i = 1; i <= 1000000; i++)
                    {
                        string d1 = "{\"app\":{\"moteeui\":\"00000000352211ad\",\"MoteEuiReversed\":\"ad11223500000000\",\"dir\":\"up\",\"seqno\":22" +
                                    ",\"userdata\":{\"port\":2,\"payload\":\"" + information + "\"}" +
                                    ",\"motetx\":{\"freq\":472.9,\"datr\":\"SF9BW125\",\"codr\":\"4 / 5\u0013\",\"adr\":false},\"gwrx\":[{\"eui\":\"0000000000bc61ad\"" +
                                    ",\"time\":\"" + DateTime.Now.ToString() + "\",\"timefromgateway\":true,\"rssi\":-55,\"chan\":5,\"lsnr\":11.0}]}," +
                                    "\"ID\":2347,\"header\":{\"pv\":2,\"token\":422,\"actid\":18,\"gweui\":\"as01020304050607\"},\"SId\":338}";
                        byte[] data = Encoding.Default.GetBytes(d1);
                        Thread.Sleep(100);
                        udp.UDPSend(data, dep);
                        Console.WriteLine("···已发送" + i + "条···总用时 " + DateTime.Now.Subtract(goTime) + " 秒···");
                    }
                }
                catch (Exception ex)
                {
                    Console.Write("处理异常！");
                    throw;
                }
            });
        }

        #endregion


        #region 重写server
        /// <summary>
        /// 重写server接口
        /// </summary>
        private class Server : IServer
        {
            public ConcurrentDictionary<string, GatewayEntry> GWEntries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public MoteBook NsMoteEntries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public MoteBook AsMoteEntries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public MoteBook CsMoteEntries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public string MyNsEUI => throw new NotImplementedException();

            public string MyAsEUI => throw new NotImplementedException();

            public string MyCsEUI => throw new NotImplementedException();

            public string MyJsEUI => throw new NotImplementedException();

            public ServerRole ServerRole => throw new NotImplementedException();

            public ConcurrentDictionary<string, GatewayEntry> KnownGw => throw new NotImplementedException();

            public ConcurrentDictionary<string, ServerEntry> KnownNs => throw new NotImplementedException();

            public ConcurrentDictionary<string, ServerEntry> KnownAs => throw new NotImplementedException();

            public ConcurrentDictionary<string, ServerEntry> KnownCs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public ConcurrentDictionary<string, ServerEntry> KnownJs => throw new NotImplementedException();

            public bool RouteToAnyCs => throw new NotImplementedException();

            public void AddToPool(LoraPackage outPackage, long sid, MoteEntry mote = null)
            {
                throw new NotImplementedException();
            }

            public CSASPackage MakeCSASPackage(string moteEui, byte[] appPld)
            {
                throw new NotImplementedException();
            }

            public Task<T> ProcessJson<T>(string jsText, ServerEntry serverContext, GWMPHeader overrideHeader = null)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region 重写udpman
        /// <summary>
        /// NPLink兼容的UdpMan
        /// </summary>
        public class NpcUdpMan : UDPMan
        {
            public NpcUdpMan(IServer refServer, string associateServerEui, DnsEndPoint ep) : base(refServer, associateServerEui, ep)
            { }

            protected override void ProcessPacket(byte[] dataRecv, DnsEndPoint remoteEndPoint)
            {
                if (fristSend) return;
                //合法包长下限
                if (dataRecv.Length >= 12)
                {
                    if (dataRecv[2] == 'a' && dataRecv[3] == 's') //我们的dtu软件也在用这个登陆需要 不开nplink兼容也能登陆
                    {
                        fristSend = true;
                        //NPLink NCTT 登录处理
                        LoginBeatSend las = JsonConvert.DeserializeObject<LoginBeatSend>(dataRecv.ToASCIIString().Insert(2, "l"));
                        LoginAsRec lar = new LoginAsRec() { app_id = las?.las?.app_id, login_name = las?.las?.login_name, login_success = true, token = las.las.token };
                        UDPSend(Encoding.ASCII.GetBytes(new LoginBeatRec() { las = lar }.ToCompactJsonString().Remove(2, 1)), remoteEndPoint);

                        sendGo(remoteEndPoint);
                        //((LoraServer)RefServer).AddOrUpdateNPLinkCompatibleCsUdpEp(GenerateCompatibleCsEui(remoteEndPoint), remoteEndPoint, las.las.app_id);
                        //LogMan.Info("【NPLink兼容CS】登录信息已更新！");
                    }
                    else
                    {
                        base.ProcessPacket(dataRecv, remoteEndPoint);
                    }
                }
            }
        }
        #endregion
    }
}
