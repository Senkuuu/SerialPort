using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace 串口测试
{
    public class WaitRec
    {
        /// <summary>
        /// 发送的内容
        /// </summary>
        public string strSend;

        /// <summary>
        /// 发送一次等待的秒数
        /// </summary>
        public int second;

        /// <summary>
        /// 超时时间
        /// </summary>
        public int time_To_Timeout;

        /// <summary>
        /// 声明
        /// </summary>
        /// <param name="strSend">发送的数据</param>
        /// <param name="second">发送之后进行判断的时间间隔</param>
        /// <param name="time_To_Timeout">发送数据之后的超时时间</param>
        public WaitRec(string strSend, int second, int time_To_Timeout)
        {
            this.strSend = strSend;
            this.second = second;
            this.time_To_Timeout = time_To_Timeout;
        }
    }
    public class SerialPortHelper
    {
        //串口类
        private SerialPort sp;
        //处理接收信息的方法
        public Action<string> action;
        public Action<string> TimeOutAction;  //超时之后执行的操作
        #region 构造参数
        /// <summary>
        /// 初始化信息
        /// </summary>
        /// <param name="com">端口号，COM1</param>
        /// <param name="btl">波特率，9600</param>
        /// <param name="sjw">数据位，8</param>
        /// <param name="tzw">停止位，StopBits.One</param>
        /// <param name="xyw">校验位，Parity.None</param>
        /// <param name="func">处理接收数据的方法</param>
        public SerialPortHelper(string com, int btl, int sjw, StopBits tzw, Parity xyw, Action<string> action, Action<string> TimeOutAction)
        {
            sp = new SerialPort();
            sp.PortName = com;
            sp.BaudRate = btl;
            sp.DataBits = sjw;
            sp.StopBits = tzw;
            sp.Parity = xyw;
            sp.DataReceived += serialPort1_DataReceived;
            this.action = action;
            this.TimeOutAction = TimeOutAction;
            sp.Open();
        }
        #endregion
        /// <summary>
        /// 将string转换到Byte[]
        /// </summary>
        /// <param name="Str_Datasource"></param>
        /// <returns></returns>
        public byte[] StringToBytes(string Str_Datasource)
        {
            Str_Datasource = Str_Datasource.Replace(" ", "");
            byte[] BytesResult = new byte[Str_Datasource.Length / 2];
            for (int i = 0; i < Str_Datasource.Length / 2; i++)
            {
                BytesResult[i] = (byte)Convert.ToInt32(Str_Datasource.Substring(i * 2, 2), 16);
            }
            return BytesResult;
        }
        /// <summary>
        /// 下发数据
        /// </summary>
        /// <param name="Str_Source"></param>
        public void SendData(string Str_Source)
        {
            byte[] BytescmdTosend = StringToBytes(Str_Source);
            sp.Write(BytescmdTosend, 0, BytescmdTosend.Length);
        }
        private string strTemp = ""; //这是一个用来标识状态的字符 接收数据之后这个就自动更改成为“接收”
        /// <summary>
        /// 下发数据
        /// </summary>
        /// <param name="str">需要下发的数据</param>
        /// <param name="second">系统每隔单位时间去判断的时间间隔</param>
        /// <param name="time_To_Timeout">每次接收数据设置的超时时间</param>
        public void SendData(string str, int second, int time_To_Timeout)
        {
            strTemp = ""; //接收到的数据 每次发送之前需要手工清零一下
            byte[] BytescmdTosend = StringToBytes(str);
            sp.Write(BytescmdTosend, 0, BytescmdTosend.Length);
            WaitRec wr = new WaitRec(str, second, time_To_Timeout);
            ParameterizedThreadStart ParStart = new ParameterizedThreadStart(WaitRec);
            Thread myThread = new Thread(ParStart);
            myThread.IsBackground = true;
            myThread.Start(wr);
        }
        /// <summary>
        /// 字符串的取和计算
        /// </summary>
        /// <param name="Str_Source"></param>
        /// <returns></returns>
        public string FunctionGetSum(string Str_Source)
        {
            string Str_Sum = string.Empty;
            int Int_Sum = 0;
            for (int i = 0; i < Str_Source.Length; i += 2)
            {
                Int_Sum += Convert.ToInt32(Str_Source.Substring(i, 2), 16);
            }
            Int_Sum = Int_Sum % 0x100;
            Str_Sum = Convert.ToString(Int_Sum, 16);
            return Str_Sum;
        }
        /// <summary>
        /// 判断格式是否正确
        /// </summary>
        /// <param name="Str_Source"></param>
        /// <returns></returns>
        public bool FunctionGetCmdCorrect(string Str_Source)
        {
            bool b_Correct = false;
            //姑且设置两个条件若是最后是0x16 而且符合376.1规约的校验位规则 则姑且认为是正确的数据
            //首先剔除所有空格
            Str_Source = Str_Source.Replace(" ", "");
            //判断最后一位是否是0x16
            if (Str_Source != "")
            {
                if (Str_Source.Substring(Str_Source.Length - 2, 2) == "16")
                {
                    //如果符合这一步再进行下边的操作 下边进行判断是否位数是偶数
                    if (Str_Source.Length % 2 == 0)
                    {
                        //位数肯定是可以被整除的
                        string Str_Part = Str_Source.Substring(12, Str_Source.Length - 12 - 4);//需要进行校验位计算的字符串
                        if (FunctionGetSum(Str_Part).ToUpper() == Str_Source.Substring(Str_Source.Length - 4, 2).ToUpper())
                        {
                            //这个地方忘了区分大小写了 失误失误
                            //如果需要进行校验位计算的数据和计算出来的校验位一致才证明是正确的 试试吧
                            b_Correct = true;
                        }
                    }
                }
            }
            return b_Correct;
        }
        /// <summary>
        /// 是一个循环，如果没有接受到需要的数据继续等待直到等到超时时间为止
        /// </summary>
        /// <param name="obj"></param>
        private void WaitRec(object obj)
        {
            WaitRec wr = (WaitRec)obj;
            bool b = false; //这个布尔型的标识主要用来标识是否已经接收到返回值
            int Int_SpendAlready = 0; //当前已经消耗的时间
            for (int i = 0; i < wr.time_To_Timeout / wr.second; i++)
            {
                if (FunctionGetCmdCorrect(strTemp) == true)
                {
                    b = true;
                    break;
                }
                else
                {
                    //返回的数据strTemp到目前为止还没有符合376.1规则的要求
                    for (int j = 0; j < wr.second; j++)
                    {
                        Thread.Sleep(1000);
                        Int_SpendAlready++;
                        //这里的意思是在前台的UI上边展示每次的超时时间和已经消耗的时间
                        //Application.OpenForms["Frm_JZQMYPLGX"].Controls.Find("Lbl_Wait", true)[0].Text = "当前耗时:" + Int_SpendAlready + "秒 " + "\r\n超时时间:" + wr.time_To_Timeout + "秒";
                        if (FunctionGetCmdCorrect(strTemp) == true)
                        {
                            //自己定义个函数用来判断返回的数据是否符合要求
                            b = true;
                            break;
                        }
                        else
                        {
                            b = false;
                            continue;
                        }
                    }
                }
            }
            #region 执行完操作之后的程序
            if (b == true)
            {
                action(strTemp);
            }
            else
            {
                sp.Close();
                TimeOutAction("操作时间" + wr.time_To_Timeout + "秒已到，请检查报文！");
            }
            #endregion
        }
        public void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string currentline = ""; ;
                byte[] lAryBytes = new byte[sp.ReadBufferSize];//这就是接收到的数据二进制文件
                int lIntLen = sp.Read(lAryBytes, 0, lAryBytes.Length);
                if (lIntLen > 0)
                {
                    byte[] lAryData = new byte[lIntLen];
                    for (int i = 0; i < lIntLen; i++)
                    {
                        lAryData[i] = lAryBytes[i];
                    }
                    currentline = ByteToString(lAryData);//转化为16进制数显示
                    //strTemp += currentline; //2014年10月21日我把程序修改了一下改成直接返回接收到的数据
                    //这个地方需要修改一下 如果当前接收到的数据已经符合了376.1规约的要求，则进行提示
                    //action(currentline);//这里是把接收到的数据反应到前台
                    //2014年10月27日修改程序如果当前接收到的数据已经可以满足要求则推送到前天，若是不能满足要求则继续等待
                    if (FunctionGetCmdCorrect(currentline) == true)
                    {
                        strTemp += currentline;
                    }
                    else
                    {
                        //如果没有满足规约要求则进行等待
                        strTemp += currentline;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Console.WriteLine(ex.Message.ToString() + "请检查指令流！");
            }
        }
        public void Close()
        {
            sp.Close();
        }
        ///  <summary>  
        /// 字符数组转字符串16进制  
        ///  </summary>  
        ///  <param name="InBytes"> 二进制字节 </param>  
        ///  <returns>类似"01 02 0F" </returns>  
        public string ByteToString(byte[] InBytes)
        {
            string StringOut = "";
            foreach (byte InByte in InBytes)
            {
                StringOut = StringOut + String.Format("{0:X2}", InByte) + " ";
            }
            return StringOut.Trim();
        }
        ///  <summary>  
        /// strhex 转字节数组  
        ///  </summary>  
        ///  <param name="InString">类似"01 02 0F" 用空格分开的  </param>  
        ///  <returns> </returns>  
        public byte[] StringToByte(string InString)
        {
            string[] ByteStrings;
            ByteStrings = InString.Split(" ".ToCharArray());
            byte[] ByteOut;
            ByteOut = new byte[ByteStrings.Length];
            for (int i = 0; i <= ByteStrings.Length - 1; i++)
            {
                ByteOut[i] = byte.Parse(ByteStrings[i], System.Globalization.NumberStyles.HexNumber);
            }
            return ByteOut;
        }
        ///  <summary>  
        ///  strhex转字节数组  
        ///  </summary>  
        ///  <param name="InString">类似"01 02 0F" 中间无空格 </param>  
        ///  <returns> </returns>  
        public byte[] StringToByte_2(string InString)
        {
            byte[] ByteOut;
            InString = InString.Replace(" ", "");
            try
            {
                string[] ByteStrings = new string[InString.Length / 2];
                int j = 0;
                for (int i = 0; i < ByteStrings.Length; i++)
                {
                    ByteStrings[i] = InString.Substring(j, 2);
                    j += 2;
                }
                ByteOut = new byte[ByteStrings.Length];
                for (int i = 0; i <= ByteStrings.Length - 1; i++)
                {
                    ByteOut[i] = byte.Parse(ByteStrings[i], System.Globalization.NumberStyles.HexNumber);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ByteOut;
        }
        ///  <summary>  
        /// 字符串 转16进制字符串  
        ///  </summary>  
        ///  <param name="InString">unico </param>  
        ///  <returns>类似“01 0f” </returns>  
        public string Str_To_0X(string InString)
        {
            return ByteToString(UnicodeEncoding.Default.GetBytes(InString));
        }
    }
}
