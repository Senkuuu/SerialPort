using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Accenture.CBox;
using Nancy;
using Nancy.Hosting.Self;
using System.Text.RegularExpressions;
using NPOI.Util;
using System.Media;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Accenture.SerialPort
{
    public partial class frmMain : Form
    {
        int A;
        int B;
        int bie;
        string Variable = "";//接收和校验的数据
        private string Inscode;
        private static System.IO.Ports.SerialPort serialPort = new System.IO.Ports.SerialPort();




        #region 自动打开串口
        public frmMain()
        {
            InitializeComponent();

            //this.btnSend.Enabled = false;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    try
                    {
                        if (cbbComList.Items.Count < System.IO.Ports.SerialPort.GetPortNames().Count())
                        {
                            if (serialPort.IsOpen)
                            {
                                Off();
                            }
                            foreach (var item in System.IO.Ports.SerialPort.GetPortNames())
                            {
                                if (!cbbComList.Items.Contains(item))
                                {
                                    if (System.IO.Ports.SerialPort.GetPortNames().Count() - cbbComList.Items.Count == 1)
                                    {
                                        this.cbbComList.Items.Add(item);
                                        cbbComList.SelectedItem = item;
                                        Open(item);
                                    }
                                    else
                                    {
                                        this.cbbComList.Items.Add(item);
                                    }
                                }
                            }
                        }
                        else if (cbbComList.Items.Count > System.IO.Ports.SerialPort.GetPortNames().Count())
                        {
                            string[] plist = System.IO.Ports.SerialPort.GetPortNames();
                            Off();
                            for (int i = 0; i < cbbComList.Items.Count; i++)
                            {
                                if (!plist.Contains(cbbComList.Items[i]))
                                {
                                    this.cbbComList.Items.Remove(cbbComList.Items[i]);
                                }
                            }
                            //foreach (var item in cbbComList.Items)
                            //{
                            //    if (!plist.Contains(item))
                            //    {
                            //        this.cbbComList.Items.Remove(item);
                            //    }
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        throw;
                    }

                    //this.cbbComList.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                }
            });

            if (this.cbbComList.Items.Count > 0)
            {
                this.cbbComList.SelectedIndex = 0;
            }
            this.cbbBaudRate.SelectedIndex = 11;
            this.cbbDataBits.SelectedIndex = 0;
            this.cbbParity.SelectedIndex = 0;
            this.cbbStopBits.SelectedIndex = 0;
            this.pictureBox1.BackgroundImage = Properties.Resources.red;

            #region 初始化下拉框

            //=============================指令类型下拉框初始化========================
            DataTable dt = new DataTable();
            dt.TableName = "dt";
            dt.Columns.Add("Value");
            dt.Columns.Add("Text");
            DataRow dr1 = dt.NewRow();
            dr1["Value"] = "50";
            dr1["Text"] = "LORA通信地址配置";
            dt.Rows.Add(dr1);
            DataRow dr2 = dt.NewRow();
            dr2["Value"] = "51";
            dr2["Text"] = "时间参数配置";
            dt.Rows.Add(dr2);
            DataRow dr3 = dt.NewRow();
            dr3["Value"] = "52";
            dr3["Text"] = "校准温湿度";
            dt.Rows.Add(dr3);

            this.ClassBox.DataSource = dt;
            this.ClassBox.DisplayMember = "Text";
            this.ClassBox.ValueMember = "Value";

            //=============================频段选择下拉框初始化========================
            DataTable dt1 = new DataTable();
            dt1.TableName = "dt";
            dt1.Columns.Add("Value");
            dt1.Columns.Add("Text");
            DataRow drr1 = dt1.NewRow();
            drr1["Value"] = "0";
            drr1["Text"] = "0(0,7)";
            dt1.Rows.Add(drr1);
            DataRow drr2 = dt1.NewRow();
            drr2["Value"] = "1";
            drr2["Text"] = "1(8,15)";
            dt1.Rows.Add(drr2);
            DataRow drr3 = dt1.NewRow();
            drr3["Value"] = "2";
            drr3["Text"] = "2(16，23)";
            dt1.Rows.Add(drr3);
            DataRow drr4 = dt1.NewRow();
            drr4["Value"] = "3";
            drr4["Text"] = "3(24，31)";
            dt1.Rows.Add(drr4);
            DataRow drr5 = dt1.NewRow();
            drr5["Value"] = "4";
            drr5["Text"] = "4(32,39)";
            dt1.Rows.Add(drr5);
            DataRow drr6 = dt1.NewRow();
            drr6["Value"] = "5";
            drr6["Text"] = "5(40,47)";
            dt1.Rows.Add(drr6);
            DataRow drr7 = dt1.NewRow();
            drr7["Value"] = "6";
            drr7["Text"] = "6(48,55)";
            dt1.Rows.Add(drr7);
            DataRow drr8 = dt1.NewRow();
            drr8["Value"] = "7";
            drr8["Text"] = "7(56,63)";
            dt1.Rows.Add(drr8);
            DataRow drr9 = dt1.NewRow();
            drr9["Value"] = "8";
            drr9["Text"] = "8(64,71)";
            dt1.Rows.Add(drr9);
            DataRow drr10 = dt1.NewRow();
            drr10["Value"] = "9";
            drr10["Text"] = "9(72,79)";
            dt1.Rows.Add(drr10);
            DataRow drr11 = dt1.NewRow();
            drr11["Value"] = "10";
            drr11["Text"] = "10(80,87)";
            dt1.Rows.Add(drr11);
            DataRow drr12 = dt1.NewRow();
            drr12["Value"] = "11";
            drr12["Text"] = "11(88,95)";
            dt1.Rows.Add(drr12);
            DataRow drr13 = dt1.NewRow();
            drr13["Value"] = "12";
            drr13["Text"] = "12(0,31)";
            dt1.Rows.Add(drr13);
            DataRow drr14 = dt1.NewRow();
            drr14["Value"] = "13";
            drr14["Text"] = "13(32,63)";
            dt1.Rows.Add(drr14);
            DataRow drr15 = dt1.NewRow();
            drr15["Value"] = "14";
            drr15["Text"] = "14(64,95)";
            dt1.Rows.Add(drr15);
            DataRow drr16 = dt1.NewRow();
            drr16["Value"] = "15";
            drr16["Text"] = "15(0,95)";
            dt1.Rows.Add(drr16);

            this.BandBox.DataSource = dt1;
            this.BandBox.DisplayMember = "Text";
            this.BandBox.ValueMember = "Value";
            this.BandBox.Visible = true;
            this.BandLab.Visible = true;


            //=============================校准设备标志下拉框初始化========================

            DataTable dt2 = new DataTable();
            dt2.TableName = "dt";
            dt2.Columns.Add("Value");
            dt2.Columns.Add("Text");
            DataRow drrr1 = dt2.NewRow();
            drrr1["Value"] = "00";
            drrr1["Text"] = "0X00-查询标定空间";
            dt2.Rows.Add(drrr1);
            DataRow drrr2 = dt2.NewRow();
            drrr2["Value"] = "01";
            drrr2["Text"] = "0X01-清除所有校准";
            dt2.Rows.Add(drrr2);
            DataRow drrr3 = dt2.NewRow();
            drrr3["Value"] = "02";
            drrr3["Text"] = "0X02-空气温度校准";
            dt2.Rows.Add(drrr3);
            DataRow drrr4 = dt2.NewRow();
            drrr4["Value"] = "03";
            drrr4["Text"] = "0X03-空气温度校准清除";
            dt2.Rows.Add(drrr4);
            DataRow drrr5 = dt2.NewRow();
            drrr5["Value"] = "04";
            drrr5["Text"] = "0X04-空气湿度校准";
            dt2.Rows.Add(drrr5);
            DataRow drrr6 = dt2.NewRow();
            drrr6["Value"] = "05";
            drrr6["Text"] = "0X05-空气湿度校准清除";
            dt2.Rows.Add(drrr6);
            DataRow drrr7 = dt2.NewRow();
            drrr7["Value"] = "06";
            drrr7["Text"] = "0X06-包芯温度校准";
            dt2.Rows.Add(drrr7);
            DataRow drrr8 = dt2.NewRow();
            drrr8["Value"] = "07";
            drrr8["Text"] = "0X07-包芯温度校准清除";
            dt2.Rows.Add(drrr8);
            DataRow drrr9 = dt2.NewRow();
            drrr9["Value"] = "08";
            drrr9["Text"] = "";
            dt2.Rows.Add(drrr9);
            DataRow drrr10 = dt2.NewRow();
            drrr10["Value"] = "09";
            drrr10["Text"] = "";
            dt2.Rows.Add(drrr10);
            DataRow drrr11 = dt2.NewRow();
            drrr11["Value"] = "FF";
            drrr11["Text"] = "0XFF-清除所有未发送成功的数据";
            dt2.Rows.Add(drrr11);

            this.markBox.DataSource = dt2;
            this.markBox.DisplayMember = "Text";
            this.markBox.ValueMember = "Value";
            this.markBox.Visible = false;
            this.markLab.Visible = false;

            #endregion


            serialPort.DataReceived += new SerialDataReceivedEventHandler(this.Com_DataReceived);//绑定事件
        }
        /// <summary>
        /// 打开
        /// </summary>
        public void Open()
        {
            if (cbbComList.Items.Count <= 0)
            {
                textBox4.Enabled = false;
                MessageBox.Show("没有发现串口,请检查线路！");
                return;

            }
            if (serialPort.IsOpen == false)
            {
                serialPort.PortName = cbbComList.SelectedItem.ToString();
                serialPort.BaudRate = Convert.ToInt32(cbbBaudRate.SelectedItem.ToString());
                serialPort.Parity = (Parity)Convert.ToInt32(cbbParity.SelectedIndex.ToString());
                serialPort.DataBits = Convert.ToInt32(cbbDataBits.SelectedItem.ToString());
                serialPort.StopBits = (StopBits)Convert.ToInt32(cbbStopBits.SelectedItem.ToString());
                try
                {
                    serialPort.Open();
                    btnSend.Enabled = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                btnOpen.Text = "关闭串口";
                pictureBox1.BackgroundImage = Properties.Resources.green;
                btnCheck.Enabled = false;
            }
            cbbComList.Enabled = !serialPort.IsOpen;
            cbbBaudRate.Enabled = !serialPort.IsOpen;
            cbbParity.Enabled = !serialPort.IsOpen;
            cbbDataBits.Enabled = !serialPort.IsOpen;
            cbbStopBits.Enabled = !serialPort.IsOpen;

        }

        /// <summary>
        /// 打开指定串口
        /// </summary>
        /// <param name="prot">串口名称</param>
        public void Open(string prot)
        {
            if (cbbComList.Items.Count <= 0)
            {
                textBox4.Enabled = false;
                MessageBox.Show("没有发现串口,请检查线路！");
                return;

            }
            if (serialPort.IsOpen == false)
            {
                serialPort.PortName = prot;
                serialPort.BaudRate = Convert.ToInt32(cbbBaudRate.SelectedItem.ToString());
                serialPort.Parity = (Parity)Convert.ToInt32(cbbParity.SelectedIndex.ToString());
                serialPort.DataBits = Convert.ToInt32(cbbDataBits.SelectedItem.ToString());
                serialPort.StopBits = (StopBits)Convert.ToInt32(cbbStopBits.SelectedItem.ToString());
                try
                {
                    serialPort.Open();
                    btnSend.Enabled = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                btnOpen.Text = "关闭串口";
                pictureBox1.BackgroundImage = Properties.Resources.green;
                btnCheck.Enabled = false;
            }
            cbbComList.Enabled = !serialPort.IsOpen;
            cbbBaudRate.Enabled = !serialPort.IsOpen;
            cbbParity.Enabled = !serialPort.IsOpen;
            cbbDataBits.Enabled = !serialPort.IsOpen;
            cbbStopBits.Enabled = !serialPort.IsOpen;

        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Off()
        {
            if (cbbComList.Items.Count <= 0)
            {
                textBox4.Enabled = false;
                MessageBox.Show("没有发现串口,请检查线路！");
                return;

            }
            if (serialPort.IsOpen == true)
            {
                try
                {
                    serialPort.Close();
                    //btnSend.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                btnOpen.Text = "打开串口";
                pictureBox1.BackgroundImage = Properties.Resources.red;
                btnCheck.Enabled = true;
            }

            cbbComList.Enabled = !serialPort.IsOpen;
            cbbBaudRate.Enabled = !serialPort.IsOpen;
            cbbParity.Enabled = !serialPort.IsOpen;
            cbbDataBits.Enabled = !serialPort.IsOpen;
            cbbStopBits.Enabled = !serialPort.IsOpen;
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (cbbComList.Items.Count <= 0)
            {
                textBox4.Enabled = false;
                MessageBox.Show("没有发现串口,请检查线路！");

                return;

            }
            if (serialPort.IsOpen == false)
            {
                serialPort.PortName = cbbComList.SelectedItem.ToString();
                serialPort.BaudRate = Convert.ToInt32(cbbBaudRate.SelectedItem.ToString());
                serialPort.Parity = (Parity)Convert.ToInt32(cbbParity.SelectedIndex.ToString());
                serialPort.DataBits = Convert.ToInt32(cbbDataBits.SelectedItem.ToString());
                serialPort.StopBits = (StopBits)Convert.ToInt32(cbbStopBits.SelectedItem.ToString());
                try
                {
                    serialPort.Open();
                    //btnSend.Enabled = false;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                btnOpen.Text = "关闭串口";
                pictureBox1.BackgroundImage = Properties.Resources.green;
                btnCheck.Enabled = false;

            }
            else
            {
                try
                {
                    serialPort.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                btnOpen.Text = "打开串口";
                pictureBox1.BackgroundImage = Properties.Resources.red;
                btnCheck.Enabled = true;
                btnSend.Enabled = true;
            }
            cbbComList.Enabled = !serialPort.IsOpen;
            cbbBaudRate.Enabled = !serialPort.IsOpen;
            cbbParity.Enabled = !serialPort.IsOpen;
            cbbDataBits.Enabled = !serialPort.IsOpen;
            cbbStopBits.Enabled = !serialPort.IsOpen;

        }

        #endregion

        #region 数据发送区

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public static bool SendData(byte[] data)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Write(data, 0, data.Length);//发送数据
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("串口未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool SendData(string str)
        {

            //string[] array = str.Split(new char[]
            //{
            //    ' '
            //});
            //StringBuilder stringBuilder = new StringBuilder();
            //for (int i = 0; i < array.Length; i++)
            //{
            //    stringBuilder.Append(array[i]);
            //}
            //str = stringBuilder.ToString();
            //if (str.Length % 2 != 0)
            //{
            //    str = str.Insert(str.Length - 5, "0");
            //}
            //byte[] array2 = new byte[str.Length / 2];
            //for (int j = 0; j < str.Length / 2; j++)
            //{
            //    try
            //    {
            //        array2[j] = Convert.ToByte(str.Substring(j * 2, 2), 16);
            //    }
            //    catch
            //    {
            //        MessageBox.Show("包含非16进制字符，发送失败！", "提示");
            //        return false;
            //    }
            //}
            //if (this.serialPort.IsOpen)
            //{
            //    try
            //    {
            //        this.serialPort.Write(array2, 0, array2.Length);
            //        return true;
            //    }
            //    catch (Exception)
            //    {
            //        MessageBox.Show("发送数据时发生错误, 串口将被关闭！", "错误提示");
            //        return false;
            //    }
            //}
            //if (this.serialPort.IsOpen)
            //{
            //    try
            //    {
            //        this.serialPort.Write(array2, 0, array2.Length);
            //        return true;
            //    }
            //    catch (Exception)
            //    {
            //        MessageBox.Show("发送数据时发生错误, 串口将被关闭！", "错误提示");
            //        return false;
            //    }
            //}
            //MessageBox.Show("串口未打开，请先打开串口！", "错误提示");

            if (serialPort.IsOpen)
            {
                try
                {
                    Encoding encoding = Encoding.GetEncoding("GB2312");
                    byte[] bytes = encoding.GetBytes(str);
                    int num = bytes.Length;
                    serialPort.Write(bytes, 0, num);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("串口未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
        string txt = "";//接收ID的值  

        /// <summary>
        /// 数组循环
        /// </summary>
        /// <returns></returns>
        public byte[] num(string value)
        {
            {
                byte[] array = new byte[1024];
                for (int i = 0; i < value.Length / 2; i++)
                {
                    string hexString = value.Substring(i * 2, 2);
                    //hexString += hexString;
                    byte b;
                    byte.TryParse(hexString, NumberStyles.HexNumber, null, out b);
                    array[i] = b;
                }
                for (int i = 0; i < array.Length; i++)
                {
                    A += array[i];
                }
                return array;
            }
        }

        /// <summary>
        /// macid累加
        /// </summary>
        /// <param name="value1"></param>
        /// <returns></returns>
        public byte[] num1(string value1)
        {
            byte[] array1 = new byte[4];
            for (int i1 = 0; i1 < value1.Length / 2; i1++)
            {
                string hexString1 = value1.Substring(i1 * 2, 2);
                //hexString += hexString;
                byte b1;
                byte.TryParse(hexString1, NumberStyles.HexNumber, null, out b1);
                array1[i1] = b1;
            }
            for (int i = 0; i < array1.Length; i++)
            {
                B += array1[i];
            }
            int a = 4 + 9 + B + A;//将需要发送的数据累加进行和校验
            String strA = Convert.ToString(a, 16);
            Variable = strA;
            return array1;
        }

        /// <summary>
        /// 查询二维码是否存在于数据库中做判断
        /// </summary>
        private void btn()
        {
            A = B = 0;
            string QECODE = this.textBox3.Text.Trim();
            // textBox3.Text = this.textBox3.Text.Substring(textBox3.TextLength - 10, 10);
            string qecode = QECODE;
            string Sql_select = string.Format("select count(*) from [dbo].[YiBao]  where [QECODE] = '{0}'", qecode);
            int t1 = Convert.ToInt32(DBHelper.MyExecuteScalar(Sql_select));
            if (t1 == 1)
            {
                //  MessageBox.Show("查到二维码存在此表中");
                //查询出刚刚插入的数据显示在文本框中textBox2中
                string sql_selectID = string.Format(@"select [MACID],[MACKEY],[MCUID],[QECODE],[BATCHNO] from  [dbo].[YiBao] where [QECODE]='{0}'", qecode);
                SqlDataReader dr = DBHelper.MyExecuteReader(sql_selectID);
                if (dr.Read())
                {
                    var id = dr["MACID"].ToString();
                    var randomKey = dr["MACKEY"].ToString();
                    var chkSum = dr["MACKEY"].ToString();
                    var chkid = dr["MACID"].ToString();
                    var QeCode = dr["QeCode"].ToString();
                    if (chkid.Length != 8)//验证ID是否为八位数，否则在前面用'0'补齐
                    {
                        while (true)
                        {
                            txt = "0" + txt;
                            if (txt.Length == 8)
                            {
                                break;
                            }
                        }
                    }
                    byte[] First = num(chkSum);
                    byte[] Second = num1(chkid);
                    var aaq = Convert.ToString(Variable.Substring(Variable.Length - 2, 2));//截取和校验最后两位数据。
                    //string TxtVal = "FD" + "0" + 4 + "0" + 9 + id + randomKey + aaq + "DF";//拼接发送的数据
                    string TxtVal = "FD" + "0" + "D" + "0" + "A" + id + randomKey + aaq + "DF";
                    string select_QeCode = string.Format(@"select count(*) from dbo.YiBao where QeCode = '{0}'and MCUID is NULL ", QeCode);
                    int qeCode = Convert.ToInt32(DBHelper.MyExecuteScalar(select_QeCode));
                    if (qeCode == 1)
                    {
                        txtSendData.Text = TxtVal;
                    }
                    else
                    {
                        MessageBox.Show("二维码已被使用");
                        ClearSelf();
                        MessageBox.Show("二维码已被使用");
                        Off();
                    }
                }
            }
            else
            {
                MessageBox.Show("二维码不存在");
                ClearSelf();
                Off();
            }
        }


        #region 2019年12月 协议更变 新增拼接新协议方案
        /// <summary>
        /// 得到下发指令的数据
        /// </summary>
        /// <returns></returns>
        private string getData(int count)
        {
            string MACID = this.textBox3.Text.Trim();
            Inscode = this.ClassBox.SelectedValue.ToString();
            string data = "";
            if (count == 0)
            {
                //频段选择转16进制(2019/12/29 协议更变)
                //data += Convert.ToString(Convert.ToInt32(BandBox.SelectedValue), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(BandBox.SelectedValue), 16) : "0" + Convert.ToString(Convert.ToInt32(BandBox.SelectedValue), 16);
                //指令回执转16进制
                string dcount = Convert.ToString(Convert.ToInt32(DataCount.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(DataCount.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(DataCount.Text), 16);
                if (dcount.Length < 4)
                {
                    for (int i = 0; i < 4 - dcount.Length; i++)
                    {
                        data += "0";
                    }
                }
                data += dcount;
            }
            else if (count == 1)
            {
                //data += Convert.ToString(Convert.ToInt32(TimeStampTxt.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(TimeStampTxt.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(TimeStampTxt.Text), 16);
                //效准时间戳 暂时写死80 00 00 00
                TimeStampTxt.ReadOnly = true;
                data += "80000000";
                //data += Convert.ToString(Convert.ToInt32(WakeupTxt.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(WakeupTxt.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(WakeupTxt.Text), 16);
                //唤醒周期1 转16进制 且判断是否为4字节 不足补0
                string WakeupData = Convert.ToString(Convert.ToInt32(WakeupTxt.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(WakeupTxt.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(WakeupTxt.Text), 16);
                if (WakeupData.Length < 8)
                {
                    for (int i = 0; i < 8 - WakeupData.Length; i++)
                    {
                        data += "0";
                    }
                }
                data += WakeupData;
                //预定的唤醒周期2 暂时用不上
                data += "00000000";
                //预定的唤醒周期3 暂时用不上
                data += "0000";
                //指令回执转16进制
                string dcount = Convert.ToString(Convert.ToInt32(DataCount.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(DataCount.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(DataCount.Text), 16);
                if (dcount.Length < 4)
                {
                    for (int i = 0; i < 4 - dcount.Length; i++)
                    {
                        data += "0";
                    }
                }
                data += dcount;
            }
            else if (count == 2)
            {
                data += "01";//清除所有校准
                data += "00C8";//data += string.Format("标准值（{0}）,", 2);
                string dcount = Convert.ToString(Convert.ToInt32(DataCount.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(DataCount.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(DataCount.Text), 16);
                if (dcount.Length < 4)
                {
                    for (int i = 0; i < 4 - dcount.Length; i++)
                    {
                        data += "0";
                    }
                }
                data += dcount;
            }
            else if (count == 3)
            {
                data += "FF";//清除所有未发送成功的数据
                data += "00C8";//data += string.Format("标准值（{0}）,", 2);
                string dcount = Convert.ToString(Convert.ToInt32(DataCount.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(DataCount.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(DataCount.Text), 16);
                if (dcount.Length < 4)
                {
                    for (int i = 0; i < 4 - dcount.Length; i++)
                    {
                        data += "0";
                    }
                }
                data += dcount;
            }

            #region 单条发送（改为连续发送）
            /*
            //判断指令码
            if (Inscode == "50")
            {
                //频段选择转16进制
                data += Convert.ToString(Convert.ToInt32(BandBox.SelectedValue), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(BandBox.SelectedValue), 16) : "0" + Convert.ToString(Convert.ToInt32(BandBox.SelectedValue), 16);
                //指令回执转16进制
                string dcount = Convert.ToString(Convert.ToInt32(DataCount.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(DataCount.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(DataCount.Text), 16);
                if (dcount.Length < 4)
                {
                    for (int i = 0; i < 4 - dcount.Length; i++)
                    {
                        data += "0";
                    }
                }
                data += dcount;
            }
            else if (Inscode == "51")
            {
                //data += Convert.ToString(Convert.ToInt32(TimeStampTxt.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(TimeStampTxt.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(TimeStampTxt.Text), 16);
                //效准时间戳 暂时写死80 00 00 00
                TimeStampTxt.ReadOnly = true;
                data += "80000000";
                //data += Convert.ToString(Convert.ToInt32(WakeupTxt.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(WakeupTxt.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(WakeupTxt.Text), 16);
                //唤醒周期1 转16进制 且判断是否为4字节 不足补0
                string WakeupData = Convert.ToString(Convert.ToInt32(WakeupTxt.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(WakeupTxt.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(WakeupTxt.Text), 16);
                if (WakeupData.Length < 8)
                {
                    for (int i = 0; i < 8 - WakeupData.Length; i++)
                    {
                        data += "0";
                    }
                }
                data += WakeupData;
                //预定的唤醒周期2 暂时用不上
                data += "00000000";
                //预定的唤醒周期3 暂时用不上
                data += "0000";
                //指令回执转16进制
                string dcount = Convert.ToString(Convert.ToInt32(DataCount.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(DataCount.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(DataCount.Text), 16);
                if (dcount.Length < 4)
                {
                    for (int i = 0; i < 4 - dcount.Length; i++)
                    {
                        data += "0";
                    }
                }
                data += dcount;
            }
            else if (Inscode == "52")
            {
                data += markBox.SelectedValue.ToString();
                data += "00C8";//data += string.Format("标准值（{0}）,", 2);
                string dcount = Convert.ToString(Convert.ToInt32(DataCount.Text), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(DataCount.Text), 16) : "0" + Convert.ToString(Convert.ToInt32(DataCount.Text), 16);
                if (dcount.Length < 4)
                {
                    for (int i = 0; i < 4 - dcount.Length; i++)
                    {
                        data += "0";
                    }
                }
                data += dcount;
            }*/
            #endregion

            return data;
        }


        /// <summary>
        /// 拼接并下发协议数据
        /// </summary>
        /// <returns></returns>
        private void getAgreementCode(int count)
        {
            Dictionary<int, string> diy = new Dictionary<int, string>();
            if (count == 4)
            {
                diy.Add(count, "");
                Push(diy);
                return;
            }
            string aCode = "";//协议整合
            string aCodeHand = "FD0D0A";//帧头
            string FrameLength = "";//帧长
            string address = "";//地址
            string Code = this.ClassBox.SelectedValue.ToString();//指令码
            string Data = getData(count);//16进制数据
            string Validation = "";//效验
            string aCodeTail = "0D0ADF";//帧尾
            //Inscode = this.ClassBox.SelectedValue.ToString();//指令码
            if (count == 0) Code = "50";
            if (count == 1) Code = "51";
            if (count == 2 || count == 3) Code = "52";

            try
            {
                #region 得到16进制地址（硬件编号）
                string MACID = this.textBox3.Text.Trim();
                //数据库查询硬件地址（硬件编号）
                //string getaddress = string.Format("select MACID from [dbo].[YiBao]  where [MACID] = '{0}'", MACID);
                //address = DBHelper.MyExecuteScalar(getaddress).ToString();
                if (address.Trim().Length != 8)//验证ID是否为八位数，否则在前面用'0'补齐
                {
                    while (true)
                    {
                        address = "0" + address;
                        if (address.Length == 8)
                        {
                            break;
                        }
                    }
                }
                //转16进制
                //byte[] address1 = num1(address);
                //string adr = "";
                //for (int i = 0; i < address1.Length; i++)
                //{
                //    adr += address1[i];
                //}
                int sum = 0;
                address = MACID;
                for (int i = 0; i < address.Length / 2; i++)
                {
                    sum += Convert.ToInt32(address.Substring(i * 2, 2), 16);
                }

                #endregion

                #region 得到16进制帧长
                //根据指令码决定帧长
                if (Code == "50")
                    FrameLength = "16";
                else if (Code == "51")
                    FrameLength = "30";
                else if (Code == "52")
                    FrameLength = "19";
                FrameLength = Convert.ToString(Convert.ToInt32(FrameLength), 16);
                #endregion

                #region 得到16进制和效验
                int sumData = 0;
                for (int i = 0; i < Data.Length / 2; i++)
                {
                    sumData += Convert.ToInt32(Data.Substring(i * 2, 2), 16);
                }
                Validation = Convert.ToString(Convert.ToInt32("0X" + FrameLength, 16) + sum + Convert.ToInt32(Code, 16) + sumData, 16);
                Validation = Validation.Length % 2 == 0 ? Validation : "0" + Validation;
                if (Validation.Length < 4)
                {
                    for (int i = 0; i <= 4 - Validation.Length; i++)
                    {
                        Validation = "0" + Validation;
                    }
                }
                #endregion

                //拼接下发协议并显示在程序上
                aCode = aCodeHand + FrameLength + address + Code + Data + Validation + aCodeTail;
                //this.txtSendData.Text = txtSendData.Text + aCode + "\r\n";
                this.txtSendData.Text = aCode;
                //成功就+1，用于指令回执
                this.DataCount.Text = (Convert.ToInt32(this.DataCount.Text) + 1).ToString();

                diy.Add(count, txtSendData.Text);
                //避免队列中出现重复数据
                if (!_queues.Contains(diy))
                {
                    Push(diy);
                }

                listBox1.Items.Insert(count, txtSendData.Text);

                //listBox1.Items.Insert(listBox1.Items.Count, txtSendData.Text);
                //SendData(txtSendData.Text);//直接下发协议给串口
                //string updatasql = string.Format("update Log set INPUTDATA = '{0}' where DOCOUNT = '{1}' and MACID = '{2}'", txtSendData.Text, count, textBox3.Text);
                //DBHelper.MyExecuteNonQuery(updatasql);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

        }
        #endregion

        #region 扫码提示，判断完之后下发数据
        /// <summary>
        /// 扫描提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            btn_Running();
        }

        #region 开始下发数据
        private void btn_Running()
        {
            listBox1.Items.Clear();
            button3.BackColor = button4.BackColor = button6.BackColor = button5.BackColor = System.Drawing.Color.Transparent;
            textBox4.Clear();
            textBox2.Clear();

            txt_box3.Text = textBox3.Text;
            #region 下发前的一系列判断
            string txt = this.textBox3.Text.Trim();
            if (serialPort.IsOpen == false)
            {
                MessageBox.Show("请先打开串口！");
                return;
            }
            if (WakeupTxt.Enabled)
            {
                MessageBox.Show("请先锁定参数后再操作！");
                return;
            }
            if (txt == "")
            {
                SoundPlayer player = new SoundPlayer();

                MessageBox.Show("请先扫描二维码,或输入设备序列号");
                ClearSelf();
                return;
            }
            if (textBox3.Text.Length != 8)
            {
                senderror.Text = "请检查序列号是否正确！";
                return;
            }
            else if (!(Convert.ToInt32(textBox3.Text, 16) > 0 && Convert.ToInt32(textBox3.Text, 16) <= 650))
            {
                //string sel = "select count(MACID) from Log where MACID = '" + textBox3.Text + "'";
                //int count = (int)DBHelper.MyExecuteScalar(sel);
                //if (count < 1)
                //{
                senderror.Text = "该序列号不在指定范围内请确认！";
                //MessageBox.Show("请检查序列号是否正确！");
                return;
                //}
            }
            //string sql = "select outputdata from Log where MACID = '" + txt_box3.Text + "' and docount = '0'";
            //object re = DBHelper.MyExecuteScalar(sql);
            //if (re != null)
            //{
            //    if (!string.IsNullOrWhiteSpace(re.ToString()))
            //    {
            //        MessageBox.Show("每个设备只能操作一次");
            //        return;
            //    }
            //}
            #endregion

            //循环发送4条，第5条为空数据 （以便取值时更好的获取数据
            for (int i = 0; i < 5; i++)
            {
                //string insert = string.Format("insert into Log(MACID,DOCOUNT,CREATEDATE) values('{0}','{1}','{2}')", textBox3.Text, i, DateTime.Now);
                //DBHelper.MyExecuteNonQuery(insert);

                getAgreementCode(i);

                //if (this.SendData(strToHexByte(txtSendData.Text.Trim())))//发送数据成功计数
                //{
                //    Thread.Sleep(500);
                //}
            }
        }
        #endregion

        /// <summary>
        /// 此方法用于将16进制的字符串转换成16进制的字节数组
        /// </summary>
        /// <param name="_hex16String">要转换的16进制的字符串。</param>
        public static byte[] Hex16StringToHex16Byte(string _hex16String)
        {
            //去掉字符串中的空格。
            _hex16String = _hex16String.Replace(" ", "");
            if (_hex16String.Length / 2 == 0)
            {
                _hex16String += " ";
            }
            //声明一个字节数组，其长度等于字符串长度的一半。
            byte[] buffer = new byte[_hex16String.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                //为字节数组的元素赋值。
                buffer[i] = Convert.ToByte((_hex16String.Substring(i * 2, 2)), 16);
            }
            //返回字节数组。
            return buffer;
        }

        /// <summary>
        /// 锁定参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "锁定参数")
            {
                if ((int)MessageBox.Show("确认后唤醒时间和频段无法再次更改，请核对后确认！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != 1)
                {
                    return;
                }
                else
                {
                    button2.Text = "解除锁定";
                    WakeupTxt.Enabled = false;
                    BandBox.Enabled = false;
                }
            }
            else
            {
                button2.Text = "锁定参数";
                WakeupTxt.Enabled = true;
                BandBox.Enabled = true;
            }

        }
        #endregion

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
        object locker = new object();

        /// <summary>
        /// 清空文本框
        /// </summary>
        public void ClearSelf()
        {
            textBox3.Text = string.Empty;
            txtSendData.Text = string.Empty;
            txtShowData.Text = string.Empty;
        }

        /// <summary>
        /// 清空发送区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearSend_Click(object sender, EventArgs e)
        {
            // PlaySound();
            txtSendData.Clear();
            textBox3.Clear();
        }



        /// <summary>
        /// 扫码枪直接输入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Length != 0)
            {
                senderror.Text = "";
                listBox1.Items.Clear();
            }
            if (textBox3.Text.Length == 8)
            {

                try
                {
                    string selsql = string.Format("select outputdata from log where macid='{0}' and docount='0'", textBox3.Text);
                    object re = DBHelper.MyExecuteScalar(selsql);
                    if (re != null)
                    {
                        if (!string.IsNullOrWhiteSpace(re.ToString()))
                        {
                            senderror.Text = "该数据非首次发送！";
                            txt_box3.Text = textBox3.Text;
                            listBox1.Items.Clear();
                            //显示之前发送过的数据
                            string sel = string.Format("select inputdata,docount from log where macid='{0}'", txt_box3.Text);
                            SqlDataReader sdr = DBHelper.MyExecuteReader(sel);
                            while (sdr.Read())
                            {
                                listBox1.Items.Insert(Convert.ToInt32(sdr[1]), sdr[0]);
                            }
                            textBox3.Clear();
                            return;
                        }
                    }
                    btn_Running();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        #endregion

        #region 数据接收区



        /// <summary>
        /// 接收的数据进行累加
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Bie(string value)
        {
            {
                byte[] array2 = new byte[21];
                for (int i = 0; i < value.Length / 2; i++)
                {
                    string hexString = value.Substring(i * 2, 2);
                    //hexString += hexString;
                    byte b;
                    byte.TryParse(hexString, NumberStyles.HexNumber, null, out b);
                    array2[i] = b;
                }
                for (int i = 0; i < array2.Length; i++)
                {
                    bie += array2[i];
                }
                return array2;
            }
        }

        /// <summary>
        /// 清空接收区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearRev_Click(object sender, EventArgs e)
        {

            txtShowData.Clear();
            textBox4.Clear();
        }

        /// <summary>
        /// 取消跨线程检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

        }

        #endregion

        #region 成品校验区

        /// <summary>
        /// 当接收到串口发包数据时触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Running();
        }

        private string getInformation(string data, string d1, int index)
        {
            data = data.Replace(" ", "");
            string result = "";
            try
            {
                int a = data.LastIndexOf(d1) + d1.Length;
                result = data.Substring(a, index);
            }
            catch (Exception)
            {
                result = "";
            }
            return result;
        }

        /// <summary>
        /// 当接收到艺宝盒子发包数据时触发的事件
        /// </summary>

        internal void OnPayload(string macId, string mcuId)
        {
            txtShowData.Text = "";
            string temp = txtShowData.Text.Trim();
            this.txtShowData.Invoke(new Action<TextBox, string, string>((txt, mac, mcu) =>
            {
                temp += string.Format("mac={0},mcu={1}\r\n", mac, mcu);
            }), this.txtShowData, macId, mcuId);//接收显示解析的macid和mcuid的值
        }
        private IisHost host;

        /// <summary>
        /// 成品校验
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (null == host)
            {
                // netsh http add urlacl url="http://+:11080/" user="Everyone"
                host = new IisHost(new Uri("http://localhost:11080/"));
                host.OnPayloaded += this.OnPayload;
                host.Start();

                txtShowData.Enabled = true;
                btnOpen.Enabled = false;
                btnCheck.Text = "停止";
            }
            else
            {
                host.Stop();
                host.Dispose();
                host = null;

                txtShowData.Enabled = false;

                btnOpen.Enabled = true;
                btnCheck.Text = "成品检验";
            }
        }



        #endregion

        #region 数据导出
        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            using (var frm = new frmExport())
            {
                frm.ShowDialog();
            }
        }
        #endregion

        #region 语音提醒区
        void PlaySound()
        {
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = @"蜂鸣器滴一声.wav";
            player.Load(); //同步加载声音
            player.Play(); //启用新线程播放

        }
        void PlaySound1()
        {
            SoundPlayer player = new SoundPlayer();
            player.SoundLocation = @"绑定成功.wav";
            player.Load(); //同步加载声音
            player.Play(); //启用新线程播放

        }

        #endregion

        #region 下拉框隐藏与显示事件
        private void ClassBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string sel = this.ClassBox.SelectedValue.ToString();
            ////只显示相关内容
            //if (sel == "50")
            //{
            //    this.BandBox.Visible = true;
            //    this.BandLab.Visible = true;
            //    //唤醒周期控件
            //    this.WakeupLab.Visible = false;
            //    this.WakeupTxt.Visible = false;
            //    //时间戳效准控件
            //    this.TimeStampLab.Visible = false;
            //    this.TimeStampTxt.Visible = false;
            //    this.markBox.Visible = false;
            //    this.markLab.Visible = false;
            //}
            //else if (sel == "51")
            //{
            //    this.BandBox.Visible = false;
            //    this.BandLab.Visible = false;
            //    //唤醒周期控件
            //    this.WakeupLab.Visible = true;
            //    this.WakeupTxt.Visible = true;
            //    //时间戳效准控件
            //    this.TimeStampLab.Visible = true;
            //    this.TimeStampTxt.Visible = true;
            //    this.markBox.Visible = false;
            //    this.markLab.Visible = false;
            //}
            //else
            //{
            //    this.BandBox.Visible = false;
            //    this.BandLab.Visible = false;
            //    //唤醒周期控件
            //    this.WakeupLab.Visible = false;
            //    this.WakeupTxt.Visible = false;
            //    //时间戳效准控件
            //    this.TimeStampLab.Visible = false;
            //    this.TimeStampTxt.Visible = false;
            //    this.markBox.Visible = true;
            //    this.markLab.Visible = true;
            //}
        }
        #endregion

        private void GetDataBtn_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string id = selID.Text;
            if (!string.IsNullOrWhiteSpace(id))
            {
                string ifm = "";
                string sql = "select inputdata from Log where MACID = '" + id + "'";
                object re = DBHelper.MyExecuteScalar(sql);
                if (re == null)
                {
                    MessageBox.Show("输入的ID错误，请核对！");
                }
                else
                {
                    ifm = re.ToString();
                    txtShowData.Text = ifm;
                }
            }
            else
            {
                MessageBox.Show("请输入ID");
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                return;
            }
            txtSendData.Text = listBox1.SelectedItem.ToString();
            string seldata = string.Format("select outputdata from log where docount = '{0}' and macid = '{1}'", listBox1.SelectedIndex, txt_box3.Text);
            if (!string.IsNullOrWhiteSpace(textBox3.Text))
            {
                seldata = string.Format("select outputdata from log where docount = '{0}' and macid = '{1}'", listBox1.SelectedIndex, textBox3.Text);
            }

            object data = DBHelper.MyExecuteScalar(seldata);
            if (data != null)
            {
                if (!string.IsNullOrWhiteSpace(data.ToString()))
                {
                    textBox4.Text = data.ToString();
                }
                else
                {
                    textBox4.Text = "返回数据错误";
                }
            }
            else
            {
                textBox4.Text = "下发数据错误";
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            LoraForm lf = new LoraForm();
            lf.cycle = WakeupTxt.Text;
            lf.Show();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            throw new NotImplementedException();
        }

        #region 逐条下发协议（消息队列

        private static ConcurrentQueue<Dictionary<int, string>> _queues = new ConcurrentQueue<Dictionary<int, string>>();

        private void Start()
        {
            //Start(() => { Running(); });
            new Thread(() => { Running(); }).Start();
        }

        internal static void Push(Dictionary<int, string> nDiy)
        {
            Task.Factory.StartNew((d) => { _queues.Enqueue((Dictionary<int, string>)d); }, nDiy);
        }


        #region 队列发送模式
        private void Running()
        {
            while (true)
            {
                #region 队列处理
                if (_queues.IsEmpty)
                {
                    Thread.Sleep(500);
                    return;
                }
                else
                {
                    Dictionary<int, string> request = new Dictionary<int, string>();
                    if (_queues.TryDequeue(out request))
                    {
                        try
                        {
                            foreach (var item in request)
                            {
                                if (item.Key == 0)
                                {
                                    //记录进数据库
                                    string insert = string.Format("insert into Log(MACID,DOCOUNT,CREATEDATE,INPUTDATA) values('{0}','{1}','{2}','{3}')", textBox3.Text, item.Key, DateTime.Now, item.Value);
                                    DBHelper.MyExecuteNonQuery(insert);

                                    //发送数据
                                    SendData(strToHexByte(item.Value.Trim()));
                                    return;
                                }
                                else
                                {
                                    Thread.Sleep(1000);
                                    if (item.Key == 4) Thread.Sleep(2000);
                                    string data = serialPort.ReadExisting();
                                    //textBox2.Text += data;
                                    data = data.Replace(" ", "");

                                    if (data.IndexOf("USART3RecFromISR:") < 1)
                                        return;

                                    //判断是否有返回协议
                                    if (isOutData(data, item.Key))
                                    {
                                        #region 发送成功对应标签变色
                                        if (item.Key - 1 == 0)
                                        {
                                            button3.BackColor = System.Drawing.Color.Green;
                                        }
                                        if (item.Key - 1 == 1)
                                        {
                                            button4.BackColor = System.Drawing.Color.Green;
                                        }
                                        if (item.Key - 1 == 2)
                                        {
                                            button5.BackColor = System.Drawing.Color.Green;
                                        }
                                        if (item.Key - 1 == 3)
                                        {
                                            button6.BackColor = System.Drawing.Color.Green;
                                        }
                                        #endregion

                                        getInformation(data, item.Key);

                                        if (item.Key != 4)
                                        {
                                            //记录进数据库
                                            string insert = string.Format("insert into Log(MACID,DOCOUNT,CREATEDATE,INPUTDATA) values('{0}','{1}','{2}','{3}')", txt_box3.Text, item.Key, DateTime.Now, item.Value);
                                            DBHelper.MyExecuteNonQuery(insert);

                                            //发送数据
                                            SendData(strToHexByte(item.Value.Trim()));
                                        }
                                    }
                                    else
                                    {
                                        #region 发送失败对应标签变色
                                        if (item.Key - 1 == 0)
                                        {
                                            button3.BackColor = System.Drawing.Color.Red;
                                        }
                                        if (item.Key - 1 == 1)
                                        {
                                            button4.BackColor = System.Drawing.Color.Red;
                                        }
                                        if (item.Key - 1 == 2)
                                        {
                                            button5.BackColor = System.Drawing.Color.Red;
                                        }
                                        if (item.Key - 1 == 3)
                                        {
                                            button6.BackColor = System.Drawing.Color.Red;
                                        }
                                        #endregion
                                        MessageBox.Show("未获取到上一条发送返回的协议数据，请重试！");
                                        break;
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                if (_queues.IsEmpty)
                {
                    break;
                }
                #endregion
            }
        }
        #endregion

        #region 判断是否有返回数据
        /// <summary>
        /// 是否有返回协议
        /// </summary>
        /// <param name="data">串口返回的所有数据</param>
        /// <param name="index">下发协议对应的listbox的序号</param>
        /// <returns></returns>
        private bool isOutData(string data, int index)
        {
            //找到上条发送协议的位置
            int d1 = data.IndexOf(listBox1.Items[index - 1].ToString()) + listBox1.Items[index - 1].ToString().Length;
            int d2 = data.IndexOf("\r\nUSART3RecFromConf:\r\n", d1) + "\r\nUSART3RecFromConf:\r\n".Length;
            //判断其是否有返回数据
            if (data.IndexOf("\r\nUSART3RecFromConf:\r\n", d1) < 0)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 解析返回协议数据
        /// <summary>
        /// 解析返回协议数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="listboxindex"></param>
        private void getInformation(string data, int listboxindex)
        {
            if (data.IndexOf("USART3RecFromISR:") < 1)
                return;
            listboxindex = listboxindex - 1;
            #region 拼接返回值
            try
            {
                textBox3.Clear();
                string outdata = "";
                int i1 = data.IndexOf(listBox1.Items[listboxindex].ToString()) + listBox1.Items[listboxindex].ToString().Length;
                int i3 = data.IndexOf("\r\nUSART3RecFromConf:\r\n", i1) + "\r\nUSART3RecFromConf:\r\n".Length;
                int i2 = data.Substring(i3, data.Length - i3).IndexOf("0D0AEF") + "0D0AEF".Length;
                string test2 = data.Substring(i3, i2);
                test2 = test2.Replace("\r\n", "").Replace("FE0D0A", "");
                outdata += "帧长：" + Convert.ToInt32(test2.Substring(0, 2), 16) + "\r\n";
                outdata += "地址：" + test2.Substring(2, 8) + "\r\n";
                outdata += "外设启用：" + test2.Substring(10, 4) + "\r\n";
                outdata += "指令码：0X" + test2.Substring(14, 2) + "\r\n";
                //仅显示一次
                if (!textBox2.Text.Contains("地址"))
                {
                    textBox2.Text += @"地址：" + test2.Substring(2, 8) + "\r\n" +
                    "外设启用：" + test2.Substring(10, 4) + "\r\n";
                }

                if (test2.Substring(14, 2) == "10")
                {
                    outdata += "频段选择：" + test2.Substring(16, 2) + "\r\n";
                    outdata += "错误码：" + test2.Substring(18, 8) + "\r\n";
                    outdata += "回执指令：" + test2.Substring(26, 4) + "\r\n";
                    textBox2.Text += "错误码：" + test2.Substring(18, 8) + "\r\n";
                    textBox2.Text += "————————" + "\r\n";
                }
                else if (test2.Substring(14, 2) == "11")
                {
                    outdata += "版本号：" + Convert.ToInt32(test2.Substring(16, 2), 16) + "\r\n";
                    outdata += "触发方式：" + Convert.ToInt32(test2.Substring(18, 2), 16) + "\r\n";
                    outdata += "电池电压：" + Convert.ToInt32(test2.Substring(20, 2), 16) + "\r\n";
                    outdata += "********************Playload****************\r\n";
                    outdata += "空气温度：" + Convert.ToInt32(test2.Substring(22, 4), 16) + "\r\n";
                    outdata += "空气湿度：" + Convert.ToInt32(test2.Substring(26, 4), 16) + "\r\n";
                    outdata += "时间戳：" + Convert.ToInt32(test2.Substring(30, 8), 16) + "\r\n";
                    outdata += "唤醒周期：" + Convert.ToInt32(test2.Substring(38, 8), 16) + "\r\n";
                    outdata += "********************End*********************\r\n";
                    outdata += "错误码：" + test2.Substring(46, 8) + "\r\n";
                    outdata += "回执指令：" + test2.Substring(54, 4) + "\r\n";
                    textBox2.Text += "空气温度：" + Convert.ToInt32(test2.Substring(22, 4), 16) + "\r\n" +
                    "空气湿度：" + Convert.ToInt32(test2.Substring(26, 4), 16) + "\r\n" +
                    "时间戳：" + Convert.ToInt32(test2.Substring(30, 8), 16) + "\r\n" +
                    "唤醒周期：" + Convert.ToInt32(test2.Substring(38, 8), 16) + "\r\n" +
                    "错误码：" + test2.Substring(46, 8) + "\r\n";
                    textBox2.Text += "————————" + "\r\n";
                }
                else if (test2.Substring(14, 2) == "12")
                {
                    outdata += "标定类型：" + Convert.ToInt32(test2.Substring(16, 2), 16) + "\r\n";
                    outdata += "接收的标定值：" + Convert.ToInt32(test2.Substring(18, 4), 16) + "\r\n";
                    outdata += "采集的标定值：" + Convert.ToInt32(test2.Substring(22, 4), 16) + "\r\n";
                    outdata += "温度已经标定数量：" + Convert.ToInt32(test2.Substring(26, 2), 16) + "\r\n";
                    outdata += "湿度已经标定数量：" + Convert.ToInt32(test2.Substring(28, 2), 16) + "\r\n";
                    //outdata += "包芯温度已经标定数量：" + Convert.ToInt32(test2.Substring(30, 2), 16) + "\r\n";
                    outdata += "错误码：" + test2.Substring(30, 8) + "\r\n";
                    outdata += "回执指令：" + test2.Substring(38, 4) + "\r\n";
                    textBox2.Text += "接收的标定值：" + Convert.ToInt32(test2.Substring(18, 4), 16) + "\r\n" +
                    "采集的标定值：" + Convert.ToInt32(test2.Substring(22, 4), 16) + "\r\n" +
                    "温度已经标定数量：" + Convert.ToInt32(test2.Substring(26, 2), 16) + "\r\n" +
                    "湿度已经标定数量：" + Convert.ToInt32(test2.Substring(28, 2), 16) + "\r\n" +
                    "错误码：" + test2.Substring(30, 8) + "\r\n";
                    textBox2.Text += "————————" + "\r\n";
                }
                else if (test2.Substring(14, 2) == "13")
                {
                    outdata += "需要接收的下一个程序帧：" + Convert.ToInt32(test2.Substring(16, 4), 16) + "\r\n";
                    outdata += "程序下载总帧：" + Convert.ToInt32(test2.Substring(20, 4), 16) + "\r\n";
                    outdata += "错误码：" + test2.Substring(24, 8) + "\r\n";
                    outdata += "回执指令：" + test2.Substring(32, 4) + "\r\n";
                }
                if (!string.IsNullOrWhiteSpace(outdata))
                {
                    string updatesql = string.Format("update Log set OUTPUTDATA = '{0}' where MACID = '{1}' and DOCOUNT = '{2}'", outdata, txt_box3.Text, listboxindex);
                    DBHelper.MyExecuteNonQuery(updatesql);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

            #endregion
        }
        #endregion

        #endregion
    }
}
