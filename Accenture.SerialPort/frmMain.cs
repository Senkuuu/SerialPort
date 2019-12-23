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

namespace Accenture.SerialPort
{
    public partial class frmMain : Form
    {
        int A;
        int B;
        int bie;
        string Variable = "";//接收和校验的数据
        private string Inscode;
        private System.IO.Ports.SerialPort serialPort = new System.IO.Ports.SerialPort();



        #region 自动打开串口
        public frmMain()
        {
            InitializeComponent();

            //this.btnSend.Enabled = false;
            this.cbbComList.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
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


            this.serialPort.DataReceived += new SerialDataReceivedEventHandler(this.Com_DataReceived);//绑定事件
        }
        /// <summary>
        /// 打开
        /// </summary>
        public void Open()
        {
            if (cbbComList.Items.Count <= 0)
            {
                textBox1.Enabled = false;
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
        /// 关闭
        /// </summary>
        public void Off()
        {
            if (cbbComList.Items.Count <= 0)
            {
                textBox1.Enabled = false;
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
                textBox1.Enabled = false;
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
        public bool SendData(byte[] data)
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
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SendData(string data)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Write(data);//发送数据
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
            string QECODE = this.textBox3.Text.Trim();
            Inscode = this.ClassBox.SelectedValue.ToString();
            string data = "";
            if (count == 0)
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
            string aCode = "";//协议整合
            string aCodeHand = "FD0D0A";//帧头
            string FrameLength = "";//帧长
            string address = "";//地址
            string Code = this.ClassBox.SelectedValue.ToString();//指令码
            string Data = getData(count);//16进制数据
            string Validation = "";//效验
            string aCodeTail = "0D0ADF";//帧尾
            Inscode = this.ClassBox.SelectedValue.ToString();//指令码

            try
            {
                #region 得到16进制地址（硬件编号）
                string QECODE = this.textBox3.Text.Trim();
                //数据库查询硬件地址（硬件编号）
                string getaddress = string.Format("select MACID from [dbo].[YiBao]  where [QECODE] = '{0}'", QECODE);
                address = DBHelper.MyExecuteScalar(getaddress).ToString();
                if (address.Length != 8)//验证ID是否为八位数，否则在前面用'0'补齐
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
                byte[] address1 = num1(address);
                string adr = "";
                for (int i = 0; i < address1.Length; i++)
                {
                    adr += address1[i];
                }
                int sum = 0;
                address = adr;
                address = adr = "22222322";//测试用地址
                for (int i = 0; i < adr.Length / 2; i++)
                {
                    sum += Convert.ToInt32(adr.Substring(i * 2, 2), 16);
                }

                #endregion

                #region 得到16进制帧长
                //根据指令码决定帧长
                if (Inscode == "50")
                    FrameLength = "17";
                else if (Inscode == "51")
                    FrameLength = "30";
                else if (Inscode == "52")
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
                listBox1.Items.Insert(count, txtSendData.Text);
                //SendData(txtSendData.Text);//直接下发协议给串口
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
            textBox1.Clear();
            if (serialPort.IsOpen == false)
            {
                MessageBox.Show("请先打开串口！");
                return;
            }
            if (textBox3.Text.Length != 43)
            {
                MessageBox.Show("请确认序列号是否正确！");
                return;
            }
            String txt = this.textBox3.Text.Trim();
            string sql = "select OUTPUTDATA from Log where QECODE = '" + textBox3.Text + "'";
            object re = DBHelper.MyExecuteScalar(sql);
            if (txt == "")
            {
                SoundPlayer player = new SoundPlayer();


                MessageBox.Show("请先扫描二维码");
                ClearSelf();


            }
            else if (re != null)
            {
                if (!string.IsNullOrWhiteSpace(re.ToString()))
                {
                    MessageBox.Show("每个设备只能操作一次");
                    return;
                }
            }
            else if (WakeupTxt.Enabled)
            {
                MessageBox.Show("请先锁定参数后再操作！");
                return;
            }
            else
            {
                //btn();
                for (int i = 0; i < 4; i++)
                {
                    getAgreementCode(i);

                    byte[] sendData = null;

                    if (rbtnSendHex.Checked)
                    {
                        sendData = strToHexByte(txtSendData.Text.Trim());
                    }
                    else if (rbtnSendASCII.Checked)
                    {
                        sendData = Encoding.ASCII.GetBytes(txtSendData.Text.Trim());
                    }
                    else if (rbtnSendUTF8.Checked)
                    {
                        sendData = Encoding.UTF8.GetBytes(txtSendData.Text.Trim());
                    }
                    else if (rbtnSendUnicode.Checked)
                    {
                        sendData = Encoding.Unicode.GetBytes(txtSendData.Text.Trim());
                    }
                    else
                    {
                        sendData = Encoding.ASCII.GetBytes(txtSendData.Text.Trim());
                    }

                    if (this.SendData(sendData))//发送数据成功计数
                    {
                        lblSendCount.Invoke(new MethodInvoker(delegate
                        {
                            lblSendCount.Text = (int.Parse(lblSendCount.Text) + txtSendData.Text.Length).ToString();
                        }));
                    }
                    Thread.Sleep(500);
                }
            }
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

        private byte[] strToHexByte(string hexString)
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
            if (textBox3.Text.Length == 43)
            {
                //Open();
                //btnSend_Click(sender, e);
                //GetDataBtn_Click(sender, e);

            }
        }


        #endregion

        #region 数据接收区

        /// <summary>
        /// 接收数据 
        /// </summary>
        /// <param name="content"></param>
        private void AddContent(string content)
        {
            lock (locker)
            {
                bie = 0;
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    if (chkAutoLine.Checked && txtShowData.Text.Length > 0)
                    {
                    }

                    txtShowData.Text = "";
                    txtShowData.Text = content;
                    string str = txtShowData.Text.Replace(" ", "").Trim();//取消字符之间的空格
                    if (Regex.IsMatch(str, @"FE[0-9]{4}[0-9A-F]{8}[0-9A-F]{8}[0-9]{16}[0-9]{8}[0-9A-F]{2}EF"))
                    {

                        //string[] strArray = str.Split(new string[] { "FE", "EF" }, StringSplitOptions.RemoveEmptyEntries);
                        int feIndex = str.IndexOf("FE");
                        string temp = str.Substring(feIndex, 50);
                        string Checksum = temp.Substring(temp.Length - 12, 8);//截取接收设备运行状态的数据 
                        string SUN = temp.Substring(4, 42);//截取接收的数据然后做累加
                        string SumCheck = temp.Substring(46, 2);//截取的校验值
                        byte[] BIE1 = Bie(SUN);

                        int a = bie;
                        String strAq = Convert.ToString(a, 16).Substring(1, 2).ToUpper();
                        if (SumCheck == strAq)//对比和校验是否一致
                        {
                            if (Checksum == "00000000")
                            {
                                // MessageBox.Show("接收的数据正确");
                                string BackCID = temp.Substring(6, 8);
                                // MessageBox.Show(BackCID);
                                string BackUID = temp.Substring(14, 24);
                                // MessageBox.Show(BackUID);
                                string Sql_Update = string.Format(@"UPDATE [dbo].[YiBao] set[MCUID] ='{0}',[CREATEDON]= getdate() WHERE [MACID]='{1}'", BackUID, BackCID);//查询出刚刚接收返回的ID是否存在于数据库中
                                int Update = DBHelper.MyExecuteNonQuery(Sql_Update);
                                if (Update == 1)
                                {
                                    //MessageBox.Show("绑定成功");
                                    Off();
                                    PlaySound1();
                                    string Sql_select = string.Format(@"select count(*) from [dbo].[YiBao] where [MCUID] ='{0}'", BackUID);
                                    int Select_MCUID = Convert.ToInt32(DBHelper.MyExecuteScalar(Sql_select));
                                    if (Select_MCUID == 1)
                                    {
                                        Off();
                                        ClearSelf();
                                        textBox3.Text = "";
                                    }
                                    else
                                    {
                                        MessageBox.Show("数据重复，请更换其他盒子！");

                                        ClearSelf();
                                        textBox3.Text = "";
                                        Off();
                                    }
                                }
                            }

                            else if (Checksum == "00000001")
                            {
                                MessageBox.Show("收到数据校验错误");
                                ClearSelf();
                                Off();
                            }
                            else if (Checksum == "00000002")
                            {
                                MessageBox.Show("写ID   EEPPROM错误");
                                ClearSelf();
                                Off();
                            }
                            else if (Checksum == "00000004")
                            {
                                MessageBox.Show("读ID   EEPPROM校验错误");
                                ClearSelf();
                                Off();
                            }
                            else if (Checksum == "00000008")
                            {
                                MessageBox.Show("写1024个密码EEPROM错误");
                                ClearSelf();
                                Off();
                            }
                            else if (Checksum == "00000010")
                            {
                                MessageBox.Show("读1024个密码校验错误");
                                ClearSelf();
                                Off();
                            }
                            else if (Checksum == "00000020")
                            {
                                MessageBox.Show("读写密钥链表校验错误");
                                ClearSelf();
                                Off();
                            }
                            else
                            {
                                MessageBox.Show("返回数据不符合通信协议！！！");
                                ClearSelf();
                                Off();
                            }

                        }
                        else
                        {
                            MessageBox.Show("和校验失败");
                            ClearSelf();
                            Off();
                        }
                    }
                }));
            }
        }

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
            textBox1.Clear();
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
        /// <summary>
        /// 接收数据格式
        /// </summary>
        /// <param name="data">字节数组</param>
        public void AddData(byte[] data)
        {
            rbtnUTF8.Checked = true;
            Thread th = new Thread(new ThreadStart(() =>
            {

                if (rbtnHex.Checked)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < data.Length; i++)
                    {
                        sb.AppendFormat("{0:x2}" + " ", data[i]);
                    }
                    AddContent(sb.ToString().ToUpper());

                }
                else if (rbtnASCII.Checked)
                {
                    AddContent(new ASCIIEncoding().GetString(data));
                }
                else if (rbtnUTF8.Checked)
                {
                    AddContent(new UTF8Encoding().GetString(data));
                }

                else
                { }

                lblRevCount.Invoke(new MethodInvoker(delegate
                {
                    lblRevCount.Text = (int.Parse(lblRevCount.Text) + data.Length).ToString();
                }));

            }));
            th.Start();
        }

        #endregion

        #region 成品校验区
        public void getOutData()
        {
            string outData = serialPort.ReadExisting();
        }


        /// <summary>
        /// 当接收到串口发包数据时触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Thread.Sleep(100);//等待0.1秒 
            //byte[] ReDatas = new byte[serialPort.BytesToRead];
            //serialPort.Read(ReDatas, 0, serialPort.BytesToRead);
            string ifmation = "";//System.Text.Encoding.Default.GetString(ReDatas);
            #region 计算出返回协议的位置
            string test1 = serialPort.ReadExisting();//serialPort.ReadTo("Display OVER");
            ifmation = test1;
            string outdata = "";
            string ifm = "";
            #endregion 

            #region 拼接返回值
            try
            {
                if (test1.IndexOf("FE 0D 0A") > 0)
                {
                    int i = test1.IndexOf("USART3 Rec From Conf:") + "USART3 Rec From Conf:".Length;
                    int i1 = test1.Substring(i, test1.Length - i).IndexOf("0D 0A EF") + "0D 0A EF".Length;
                    test1 = test1.Substring(i, i1);
                    test1 = test1.Replace("\r\n", "").Replace("FE 0D 0A", "");
                    outdata += "帧长：" + Convert.ToInt32(test1.Replace(" ", "").Substring(0, 2), 16) + "\r\n";
                    outdata += "地址：" + test1.Replace(" ", "").Substring(2, 8) + "\r\n";
                    outdata += "外设启用：" + Convert.ToInt32(test1.Replace(" ", "").Substring(10, 4), 16) + "\r\n";
                    outdata += "指令码：0X" + test1.Replace(" ", "").Substring(14, 2) + "\r\n";
                    if (test1.Replace(" ", "").Substring(14, 2) == "10")
                    {
                        outdata += "频段选择：" + test1.Replace(" ", "").Substring(16, 2) + "\r\n";
                        outdata += "错误码：" + test1.Replace(" ", "").Substring(18, 8) + "\r\n";
                        outdata += "回执指令：" + test1.Replace(" ", "").Substring(26, 4) + "\r\n";
                    }
                    else if (test1.Replace(" ", "").Substring(14, 2) == "11")
                    {
                        outdata += "版本号：" + Convert.ToInt32(test1.Replace(" ", "").Substring(16, 2), 16) + "\r\n";
                        outdata += "触发方式：" + Convert.ToInt32(test1.Replace(" ", "").Substring(18, 2), 16) + "\r\n";
                        outdata += "回执指令：" + Convert.ToInt32(test1.Replace(" ", "").Substring(20, 2), 16) + "\r\n";
                        outdata += "********************Playload****************\r\n";
                        outdata += "空气温度：" + Convert.ToInt32(test1.Replace(" ", "").Substring(22, 4), 16) + "\r\n";
                        outdata += "空气湿度：" + Convert.ToInt32(test1.Replace(" ", "").Substring(26, 4), 16) + "\r\n";
                        outdata += "时间戳：" + Convert.ToInt32(test1.Replace(" ", "").Substring(30, 8), 16) + "\r\n";
                        outdata += "唤醒周期：" + Convert.ToInt32(test1.Replace(" ", "").Substring(38, 8), 16) + "\r\n";
                        outdata += "********************End*********************\r\n";
                        outdata += "错误码：" + test1.Replace(" ", "").Substring(46, 8) + "\r\n";
                        outdata += "回执指令：" + test1.Replace(" ", "").Substring(54, 4) + "\r\n";
                    }
                    else if (test1.Replace(" ", "").Substring(14, 2) == "12")
                    {
                        outdata += "标定类型：" + Convert.ToInt32(test1.Replace(" ", "").Substring(16, 2), 16) + "\r\n";
                        outdata += "接收的标定值：" + Convert.ToInt32(test1.Replace(" ", "").Substring(28, 4), 16) + "\r\n";
                        outdata += "采集的标定值：" + Convert.ToInt32(test1.Replace(" ", "").Substring(22, 4), 16) + "\r\n";
                        outdata += "温度已经标定数量：" + Convert.ToInt32(test1.Replace(" ", "").Substring(26, 2), 16) + "\r\n";
                        outdata += "湿度已经标定数量：" + Convert.ToInt32(test1.Replace(" ", "").Substring(28, 2), 16) + "\r\n";
                        //outdata += "包芯温度已经标定数量：" + Convert.ToInt32(test1.Replace(" ", "").Substring(30, 2), 16) + "\r\n";
                        outdata += "错误码：" + test1.Replace(" ", "").Substring(30, 8) + "\r\n";
                        outdata += "回执指令：" + test1.Replace(" ", "").Substring(38, 4) + "\r\n";
                    }
                    else if (test1.Replace(" ", "").Substring(14, 2) == "13")
                    {
                        outdata += "需要接收的下一个程序帧：" + Convert.ToInt32(test1.Replace(" ", "").Substring(16, 4), 16) + "\r\n";
                        outdata += "程序下载总帧：" + Convert.ToInt32(test1.Replace(" ", "").Substring(20, 4), 16) + "\r\n";
                        outdata += "错误码：" + test1.Replace(" ", "").Substring(24, 8) + "\r\n";
                        outdata += "回执指令：" + test1.Replace(" ", "").Substring(32, 4) + "\r\n";
                    }
                    outdata += "———————————————————\r\n";
                }
                if (test1.IndexOf("Collect Data") > 0)
                {
                    string errorcode = "错误码：" + getInformation(ifmation, "ErrorCode=", 8) + "\r\n";
                    string UNIXTIME = "时间戳：" + getInformation(ifmation, "UNIXTIME=", 8) + "\r\n";
                    string WakeUpTime = "唤醒周期：" + Convert.ToInt32(getInformation(ifmation, "WakeUpTime=", 8), 16) + "秒" + "\r\n";
                    string Power = "电量：" + getInformation(ifmation, "Power=", 1) + "\r\n";
                    string AirTemp = "温度：" + getInformation(ifmation, "AirTemp=", 5) + "\r\n";
                    string AirHum = "湿度：" + getInformation(ifmation, "AirHum=", 5) + "\r\n";
                    string S_Version = "版本号：" + getInformation(ifmation, "S_Version=", 2) + "\r\n";
                    string LORA_Channel = "LORA_Channel：" + getInformation(ifmation, "LORA_Channel=", 1) + "\r\n";
                    string WakeUpMethods = "唤醒方式：" + getInformation(ifmation, "WakeUpMethods=", 1) == "1" ? "自动唤醒" : "手动唤醒" + "\r\n";

                    ifm = errorcode + UNIXTIME + WakeUpTime + Power + AirTemp + AirHum + S_Version + LORA_Channel + WakeUpMethods;
                }

                if (!string.IsNullOrWhiteSpace(outdata) || !string.IsNullOrWhiteSpace(ifm))
                {
                    string sql1 = "select OUTPUTDATA from Log where QECODE = '" + textBox3.Text + "'";
                    object re1 = DBHelper.MyExecuteScalar(sql1);
                    string sql2 = "select INFORMATION from Log where QECODE = '" + textBox3.Text + "'";
                    object re2 = DBHelper.MyExecuteScalar(sql2);
                    if (re1 == null && re2 == null)
                    {
                        string sql = string.Format("insert into Log values('{0}','{1}','{2}','{3}','{4}')", textBox3.Text, txtSendData.Text, DateTime.Now, outdata, ifm);
                        DBHelper.MyExecuteNonQuery(sql);
                    }
                    else if (re1 == null || (string)re1 == "")
                    {
                        if (!string.IsNullOrWhiteSpace(outdata))
                        {
                            DBHelper.MyExecuteNonQuery("update Log set OUTPUTDATA = '" + outdata + "' where QECODE = '" + textBox3.Text + "'");
                        }
                    }
                    else if (re2 == null || (string)re1 == "")
                    {
                        if (!string.IsNullOrWhiteSpace(ifm))
                        {
                            DBHelper.MyExecuteNonQuery("update Log set INFORMATION = '" + ifm + "' where QECODE = '" + textBox3.Text + "'");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }

            #endregion

            this.textBox1.Text += outdata;

            //serialPort.Read(ReDatas, 0, ReDatas.Length);//读取数据
            //this.AddData(ReDatas);//输出数据
        }

        /// <summary>
        /// 修改数据库数据
        /// </summary>
        private void editData() {

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
                textbox1(temp);
            }), this.txtShowData, macId, mcuId);//接收显示解析的macid和mcuid的值
        }
        /// <summary>
        /// 把接收解析的值显示到textbox1中然后查询出对应的序列号。
        /// </summary>
        /// <param name="txttemp"></param>
        private void textbox1(string txttemp)
        {
            string MACID = txttemp.Substring(4, 8);
            string MCUID = txttemp.Substring(17, 24);
            string Select_AppendText = string.Format(@"select * from [dbo].[YiBao] where [MACID]='{0}'and [MCUID]='{1}' ", MACID, MCUID);
            SqlDataReader dr = DBHelper.MyExecuteReader(Select_AppendText);
            if (dr.Read())
            {
                var BATCHNO = dr["BATCHNO"].ToString();
                string Update_EndTime = string.Format(@"update dbo.YiBao set EndDate=getdate() where BATCHNO='{0}'", BATCHNO);//插入最后下数据的时间来判断是否大于八个小时
                int endtime = DBHelper.MyExecuteNonQuery(Update_EndTime);

                textBox1.Text += DateTime.Now + " 序列号为：" + BATCHNO + "\r\n";
                PlaySound();//语音提示
            }
            else
            {
                MessageBox.Show("找不到该盒子序列号！！！");

                ClearSelf();

            }
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


        /// <summary>
        /// 不停的向TextBox1文本框中追加数据时，使光标的焦点始终显示在最后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBox1.Focus();//让文本框获取焦点
            this.textBox1.Select(this.textBox1.TextLength, 0);//设置光标的位置到文本尾部
            this.textBox1.ScrollToCaret();//滚动到控件光标处
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
                string sql = "select INFORMATION from Log where QECODE = '" + id + "'";
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


    }
}
