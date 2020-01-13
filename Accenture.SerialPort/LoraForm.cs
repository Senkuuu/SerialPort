using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Wima.Lora.Model;
using Wima.Lora;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using Wima.Log;
using System.Configuration;
using Newtonsoft.Json;
using System.Threading;

namespace Accenture.SerialPort
{
    public partial class LoraForm : Form
    {
        /// <summary>
        /// 服务器是否打开
        /// </summary>
        private bool ISOpen { get; set; } = false;
        /// <summary>
        /// udp代理
        /// </summary>
        private UdpMan UdpServer { get; set; }
        /// <summary>
        /// 心跳包下发时间用于判断超时
        /// </summary>
        private DateTime BeatSendTime { get; set; }
        /// <summary>
        /// 界面时间控制下发心跳包
        /// </summary>
        private int TimeCount { get; set; } = 28;
        /// <summary>
        /// 是否下发了心跳
        /// </summary>
        private bool IsSendBeat = false;
        /// <summary>
        /// 计时当前心跳包已下发时间知道收到回复或者超时
        /// </summary>
        private int SendBeatTimeCount = 0;
        private LogMan log => UdpMan.log;

        private int MAXCOUNT = 500;

        public LoraForm()
        {
            InitializeComponent();
        }

        public CSConfig ReadConfig()
        {
            try
            {
                using (StreamReader sr = new StreamReader("CSConfig.config"))
                {
                    return JsonConvert.DeserializeObject<CSConfig>(sr.ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new CSConfig();
            }
        }

        public void ReadIp()
        {
            if (File.Exists("Iplist.config"))
            {
                try
                {
                    using (StreamReader sr = new StreamReader("Iplist.config"))
                    {
                        while (!sr.EndOfStream)
                        {
                            txt_ip.Items.Add(sr.ReadLine());
                        }
                    }
                }
                catch { }

            }
        }


        private void Btn_kq_Click(object sender, EventArgs e)
        {
            if (btn_kq.Text == "开启")
            {
                try
                {
                    string strip = txt_ip.Text.Trim();
                    string appeui = tb_appeui.Text.Trim().ToLower();
                    if (!string.IsNullOrEmpty(strip) && int.TryParse(txt_port.Text.Trim(), out int port) && !string.IsNullOrEmpty(appeui) && appeui.Length == 16 && Utils.IsLegalHexStr(appeui))
                    {
                        if (UdpServer?.UdpServer != null) UdpServer.Stop();
                        UdpServer = new UdpMan(new DnsEndPoint(txt_ip.Text.Trim(), port), appeui);
                        UdpServer.Start();
                        UdpServer.ShowEvent += Udpserver_ShowEvent;
                        btn_kq.Text = "关闭";
                        txt_ip.Enabled = false;
                        txt_port.Enabled = false;
                        tb_appeui.Enabled = false;
                        lab_img.BackColor = Color.Green;
                        ISOpen = true;

                        if (txt_ip.Items.Contains(strip))
                            txt_ip.Items.Remove(strip);
                        txt_ip.Items.Insert(0, strip);
                        if (txt_ip.Items.Count > 10)
                            txt_ip.Items.RemoveAt(txt_ip.Items.Count - 1);
                        txt_ip.Text = strip;
                    }
                    else MessageBox.Show("请检查参数设置！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    if (UdpServer != null)
                    {
                        UdpServer.Stop();
                        UdpServer.ShowEvent -= Udpserver_ShowEvent;
                    }
                    btn_kq.Text = "开启";
                    txt_ip.Enabled = true;
                    txt_port.Enabled = true;
                    lab_img.BackColor = Color.Gray;
                    ISOpen = false;
                    IsSendBeat = false;
                    tb_appeui.Enabled = true;
                    SendBeatTimeCount = 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }



        /// <summary>
        /// 接收到数据由udpserver推送
        /// </summary>
        /// <param name="package">ASCS数据包</param>
        private void Udpserver_ShowEvent(ASCSPackage package)
        {
            string deveui = package.app?.moteeui;
            try
            {
                string ShowJson = package.app.gwrx[0].time + "  " + package.app.moteeui + "  " + package.app.gwrx[0].chan + "  " + package.app.motetx.freq + "  " +
                    package.app.motetx.datr + "  " + package.app.motetx.adr + "  " + package.app.gwrx[0].rssi + "  " + package.app.gwrx[0].lsnr;
                if (deveui != null)
                {
                    byte[] data = Utils.FromLoraBase64Str(package?.app?.userdata?.payload);
                    string outdata = "";
                    // byte[] data = new byte[34] { 0x24, 0x13, 0x62, 0x57, 0xE6, 0x01, 0x1E, 0x00, 0x1E, 0x00, 0x44, 0x34, 0x0F, 0x5C, 0x0B, 0x09, 0x0B, 0x09, 0x0B, 0x09, 0x28, 0x9D, 0x0B, 0x5C, 0x34, 0x09, 0x34, 0x09, 0x34, 0x09, 0x02, 0x70, 0x0D, 0x0A };
                    if (data.Length > 6)
                    {
                        string moteid = data.SubArray(4, 4).ToHexString().ToUpper(); //短地址
                        string cmd = "0x" + new byte[] { data[10] }.ToHexString(); //指令
                        outdata += "地址：" + data.SubArray(4, 4).ToHexString().ToUpper() + "\r\n";
                        outdata += "外设启用：" + Convert.ToInt32(data.SubArray(8, 2).ToHexString().ToUpper(), 16) + "\r\n";
                        outdata += "指令码：" + cmd + "\r\n";
                        switch (cmd)
                        {
                            case "0x10":
                                outdata += "频段选择：" + data.SubArray(11, 1).ToHexString().ToUpper() + "\r\n";
                                outdata += "错误码：" + data.SubArray(12, 4).ToHexString().ToUpper() + "\r\n";
                                outdata += "回执指令：" + data.SubArray(16, 2).ToHexString().ToUpper() + "\r\n";
                                break;
                            case "0x11":
                                outdata += "版本号：" + data.SubArray(11, 1).ToHexString().ToUpper() + "\r\n";
                                outdata += "触发方式：" + (data.SubArray(12, 1).ToHexString().ToUpper() == "01" ? "自动唤醒" : "手动唤醒") + "\r\n";
                                outdata += "电池电压：" + Convert.ToInt32(data.SubArray(13, 1).ToHexString().ToUpper(), 16) + "\r\n";
                                outdata += "********************Playload****************" + "\r\n";
                                outdata += "空气温度：" + Convert.ToInt32(data.SubArray(14, 2).ToHexString().ToUpper(), 16) + "\r\n";
                                outdata += "空气湿度：" + Convert.ToInt32(data.SubArray(16, 2).ToHexString().ToUpper(), 16) + "\r\n";
                                outdata += "时间戳：" + data.SubArray(18, 4).ToHexString().ToUpper() + "\r\n";
                                outdata += "唤醒周期：" + Convert.ToInt32(data.SubArray(22, 4).ToHexString().ToUpper(), 16) + "\r\n";
                                outdata += "********************End*********************" + "\r\n";
                                outdata += "错误码：" + data.SubArray(26, 4).ToHexString().ToUpper() + "\r\n";
                                outdata += "回执指令：" + data.SubArray(30, 2).ToHexString().ToUpper() + "\r\n";
                                #region 原
                                //if (data.Length == 44)
                                //{
                                //    data = data.SubArray(6, 38);
                                //    int CWCycle = ((data[1] << 8) | data[0]);
                                //    int FBCycle = ((data[3] << 8) | data[2]);
                                //    DateTime motetime = Utils.TimeBaseline.AddHours(8).AddSeconds((data[7] << 24) | (data[6] << 16) | (data[5] << 8) | data[4]);
                                //    DateTime t1 = DateTime.Parse(motetime.Minute < 30 ? motetime.ToString("yyyy/MM/dd HH") + ":00:00" : motetime.ToString("yyyy/MM/dd HH") + ":30:00");
                                //    Dictionary<DateTime, float> ddtem1 = new Dictionary<DateTime, float>();
                                //    Dictionary<DateTime, float> ddhum1 = new Dictionary<DateTime, float>();
                                //    Dictionary<DateTime, float> ddtem2 = new Dictionary<DateTime, float>();
                                //    Dictionary<DateTime, float> ddhum2 = new Dictionary<DateTime, float>();
                                //    ddtem1.Add(t1, ((data[9] << 8) | data[8]) / 100f);
                                //    ddhum1.Add(t1, ((data[11] << 8) | data[10]) / 100f);
                                //    ddtem1.Add(t1.AddMinutes(-30), ((data[13] << 8) | data[12]) / 100f);
                                //    ddhum1.Add(t1.AddMinutes(-30), ((data[15] << 8) | data[14]) / 100f);
                                //    ddtem1.Add(t1.AddMinutes(-60), ((data[17] << 8) | data[16]) / 100f);
                                //    ddhum1.Add(t1.AddMinutes(-60), ((data[19] << 8) | data[18]) / 100f);
                                //    DateTime BFtime = Utils.TimeBaseline.AddHours(8).AddSeconds((data[23] << 24) | (data[22] << 16) | (data[21] << 8) | data[20]);
                                //    DateTime t2 = DateTime.Parse(BFtime.Minute < 30 ? BFtime.ToString("yyyy/MM/dd HH") + ":00:00" : BFtime.ToString("yyyy/MM/dd HH") + ":30:00");
                                //    ddtem2.Add(t2, ((data[25] << 8) | data[24]) / 100f);
                                //    ddhum2.Add(t2, ((data[27] << 8) | data[26]) / 100f);
                                //    ddtem2.Add(t2.AddMinutes(30), ((data[29] << 8) | data[28]) / 100f);
                                //    ddhum2.Add(t2.AddMinutes(30), ((data[31] << 8) | data[30]) / 100f);
                                //    ddtem2.Add(t2.AddMinutes(60), ((data[33] << 8) | data[32]) / 100f);
                                //    ddhum2.Add(t2.AddMinutes(60), ((data[35] << 8) | data[34]) / 100f);
                                //    int Paset = (data[36] >> 4) & 0x0f;
                                //    int Bt = data[36] & 0x0f;
                                //    int OutWarn = data[37] & 0b1;
                                //    int Version = data[37] >> 4;
                                //    List<float> lt1 = ddtem1.Values.ToList();
                                //    List<float> lm1 = ddhum1.Values.ToList();
                                //    List<float> lt2 = ddtem2.Values.ToList();
                                //    List<float> lm2 = ddhum2.Values.ToList();

                                //    string ShowString = "Now=" + DateTime.Now.ToString() + ",C=" + motetime.ToString() + ",M=" + moteid + ",CR=" + CWCycle + ",SR=" + FBCycle + ",BT=" + Bt + ",T1="
                                //    + t1.ToString() + "(TP=" + lt1[0] + "/" + lt1[1] + "/" + lt1[2] + ",HM=" + lm1[0] + "/" + lm1[1] + "/" + lm1[2] + "),T2=" + t2.ToString() + "(TP=" + lt2[0] + "/" + lt2[1] + "/" + lt2[2] + ",HM=" + lm2[0] + "/" + lm2[1] + "/" + lm2[2] + ")";
                                //    AddMote(moteid);
                                //    if (IsCheckMote(moteid)) AddShow(checkBox2.Checked ? ShowString : ShowJson, package);
                                //    RealMoteState real = null;
                                //    if (RealMoteStatic.TryGetValue(moteid, out real))
                                //    {
                                //        real.Time = (long)(motetime - DateTime.Now).TotalSeconds;
                                //        real.LastTP = lt1[0];
                                //        real.LastHm = lm1[0];
                                //        real.BT = Bt;
                                //        real.CR = CWCycle;
                                //        real.SR = FBCycle;
                                //        real.CKBJ = OutWarn;
                                //        real.LastRcvTime = DateTime.Now;
                                //        real.Version = Version;
                                //    }
                                //    else
                                //    {
                                //        real = new RealMoteState();
                                //        real.DevAddr = moteid;
                                //        real.DevEui = deveui;
                                //        real.Time = (long)(motetime - DateTime.Now).TotalSeconds;
                                //        real.LastTP = lt1[0];
                                //        real.LastHm = lm2[0];
                                //        real.BT = Bt;
                                //        real.CR = CWCycle;
                                //        real.SR = FBCycle;
                                //        real.CKBJ = OutWarn;
                                //        real.LastRcvTime = DateTime.Now;
                                //        real.Version = Version;
                                //        RealMoteStatic.TryAdd(moteid, real);
                                //    }
                                //    Task.Run(() =>
                                //    {
                                //        ddtem1.ToList().ForEach(i => { sql.TempInsert(new TempClass(moteid, i.Value, i.Key)); });
                                //        ddhum1.ToList().ForEach(i => { sql.HumInsert(new HumClass(moteid, i.Value, i.Key)); });

                                //        ddtem2.ToList().ForEach(i => { sql.TempInsert(new TempClass(moteid, i.Value, i.Key)); });
                                //        ddhum2.ToList().ForEach(i => { sql.HumInsert(new HumClass(moteid, i.Value, i.Key)); });

                                //        sql.RealTUpdateOrInsert(moteid, lt1[0], lm1[0], Bt, OutWarn, Version);
                                //        sql.BaseDataInsert(new RcvBase(cmd, moteid, ShowString, DateTime.Now));
                                //    });
                                //    bool timejz = Math.Abs((DateTime.Now - motetime).TotalMinutes) >= 5 && real.TimeInt >= (ulong)config.TimeJzInt;
                                //    real.TimeInt++;
                                //    if (cmd == "0x24" && config.IsAuto && (timejz || CWCycle != config.CwCycle || FBCycle != config.SendCycle))
                                //        Task.Run(() =>
                                //        {
                                //            List<byte> lb = new List<byte>();
                                //            lb.Add(0x24); //包头
                                //            lb = lb.Concat(Utils.Hex2Bytes(moteid)).ToList(); //moteid
                                //            lb.Add(0x01); //命令
                                //            lb.Add((byte)(config.CwCycle & 0xff));
                                //            lb.Add((byte)((config.CwCycle >> 8) * 0xff));
                                //            lb.Add((byte)(config.SendCycle & 0xff));
                                //            lb.Add((byte)((config.SendCycle >> 8) * 0xff));
                                //            int xz = 0, ms = 0;
                                //            if (timejz)
                                //            {
                                //                if ((DateTime.Now - motetime).TotalMinutes >= 5)
                                //                {
                                //                    xz = 1;
                                //                    ms = (int)(DateTime.Now - motetime).TotalSeconds;
                                //                }
                                //                else if ((DateTime.Now - motetime).TotalMinutes <= -5)
                                //                {
                                //                    xz = 2;
                                //                    ms = (int)(motetime - DateTime.Now).TotalSeconds;
                                //                }
                                //                real.TimeInt = 0;
                                //            }
                                //            lb.Add((byte)((config.OutKU << 4) | (byte)xz));
                                //            lb = lb.Concat(new byte[4] { (byte)(ms & 0xff), (byte)((ms >> 8) & 0xff), (byte)((ms >> 16) & 0xff), (byte)((ms >> 24) & 0xff) }).ToList();
                                //            lb.Add(0x00);
                                //            lb.Add(0x00);
                                //            Send(deveui, lb.ToArray());
                                //            log.Info("调整包发送！DevAddr=" + moteid + "条件=【ServerTime=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ",MoteTime=" + motetime.ToString("yyyy-MM-dd HH:mm:ss")
                                //          + ",MoteCR=" + CWCycle + ",ConfigCR=" + config.CwCycle + ",MoteSR=" + FBCycle + ",ConfigSR=" + config.SendCycle + "】");

                                //        });
                                //    else if (cmd == "0x25" && Math.Abs((DateTime.Now - motetime).TotalMinutes) >= 5)
                                //    {
                                //        Task.Run(() =>
                                //        {
                                //            List<byte> lb = new List<byte>();
                                //            lb.Add(0x25); //包头
                                //            long time = (long)(DateTime.Now.AddHours(-8) - Utils.TimeBaseline).TotalSeconds;
                                //            lb = lb.Concat(new byte[4] { (byte)(time & 0xff), (byte)((time >> 8) & 0xff), (byte)((time >> 16) & 0xff), (byte)((time >> 24) & 0xff) }).ToList();
                                //            Send(deveui, lb.ToArray());
                                //            log.Info("出厂测试包下发！DevAddr=" + moteid);
                                //        });
                                //    }
                                //}
                                //else log.Info("数据长度错误！数据=" + data.ToHexString());
                                #endregion
                                break;
                            case "0x12":
                                outdata += "标定类型：" + data.SubArray(11, 1).ToHexString().ToUpper() + "\r\n";
                                outdata += "接收的标定值：" + data.SubArray(12, 2).ToHexString().ToUpper() + "\r\n";
                                outdata += "采集的标定值：" + data.SubArray(14, 2).ToHexString().ToUpper() + "\r\n";
                                outdata += "温度已经标定数量：" + data.SubArray(16, 1).ToHexString().ToUpper() + "\r\n";
                                outdata += "湿度已经标定数量：" + data.SubArray(17, 1).ToHexString().ToUpper() + "\r\n";
                                outdata += "错误码：" + data.SubArray(18, 4).ToHexString().ToUpper() + "\r\n";
                                outdata += "回执指令：" + data.SubArray(22, 2).ToHexString().ToUpper() + "\r\n";
                                break;
                            case "0x13":
                                outdata += "需要接收的下一个程序帧：" + data.SubArray(11, 2).ToHexString().ToUpper() + "\r\n";
                                outdata += "程序下载总帧：" + data.SubArray(13, 2).ToHexString().ToUpper() + "\r\n";
                                outdata += "错误码：" + data.SubArray(15, 4).ToHexString().ToUpper() + "\r\n";
                                outdata += "回执指令：" + data.SubArray(14, 2).ToHexString().ToUpper() + "\r\n";
                                #region 原
                                //if (data.Length == 7 || data.Length == 8)
                                //{
                                //    int fsk = 0; int radio = 0; int power = 0;
                                //    if (data[5] == 0x01)
                                //    {
                                //        radio = data[6];
                                //        power = data[7];
                                //    }
                                //    else if (data[5] == 0x02) fsk = data[6];
                                //    Task.Run(() =>
                                //    {
                                //        string str = "Now=" + DateTime.Now.ToString() + ",Fsk=" + fsk + ",Radio=" + radio + ",Power=" + power;
                                //        sql.BaseDataInsert(new RcvBase(cmd, moteid, str, DateTime.Now));
                                //        sql.RealTUpdateOrInsert(moteid, fsk, radio, power);
                                //    });
                                //    if (config.IsAuto && power != 0 && (radio != config.Radio || power != config.Power))
                                //    {
                                //        Task.Run(() =>
                                //        {
                                //            List<byte> lb = new List<byte>();
                                //            lb.Add(data[0]);
                                //            lb = lb.Concat(Utils.Hex2Bytes(moteid)).ToList(); //moteid
                                //            lb.Add(0x01);
                                //            lb.Add((byte)(config.Radio * 0xff));
                                //            lb.Add((byte)(config.Power * 0xff));
                                //            Send(deveui, lb.ToArray());
                                //            log.Info("Fsk调整包发送！DevAddr=" + moteid + ",条件=【MoteRadio=" + radio + ",ConfigRadio=" + config.Radio + ",MotePower=" + power + ",ConfigPower=" + config.Power + "】");
                                //        });
                                //    }
                                //}
                                //else log.Info("数据长度错误！数据=" + data.ToHexString());
                                #endregion
                                break;
                        }
                        //if (dataGridView1.Rows.Count > 0)
                        //{
                        //    dataGridView1.Rows.Clear();
                        //}
                        DataGridView dgv = dataGridView1;
                        this.Invoke((Action)delegate
                        {
                            int index = dgv.Rows.Add();
                            DataGridViewRow dgvr = dataGridView1.Rows[index];
                            //序号
                            dgvr.Cells["index"].Value = "#0";
                            //系统时间
                            dgvr.Cells["systime"].Value = package.app.gwrx[0].time.ToString();
                            //终端ID
                            dgvr.Cells["moteeui"].Value = package.app.moteeui.ToString();
                            //接收频率
                            dgvr.Cells["freq"].Value = package.app.motetx.freq.ToString();
                            //信号强度
                            dgvr.Cells["rssi"].Value = package.app.gwrx[0].rssi.ToString();
                            //速率
                            dgvr.Cells["datr"].Value = package.app.motetx.datr;
                            //信噪比
                            dgvr.Cells["lsnr"].Value = package.app.gwrx[0].lsnr.ToString();
                            //16进制数据
                            dgvr.Cells["hexdata"].Value = data.ToHexString();
                            //字符串数据
                            dgvr.Cells["strdata"].Value = outdata;
                            dgv.Rows[index].Visible = false;
                            if (!checkedListBox1.Items.Contains(package.app.moteeui.ToString()))
                                checkedListBox1.Items.Add(package.app.moteeui.ToString());
                            //dataGridView1.Rows.Insert(dgv.NewRowIndex, dgvr);
                            //dgv.Rows.Add(dgvr);
                        });


                    }
                    else log.Info("数据长度错误！数据=" + data.ToHexString());
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// 向终端列表添加新的终端
        /// </summary>
        /// <param name="deveui"></param>
        private void AddMote(string deveui)
        {
            this.Invoke((Action)delegate
            {
                if (!checkedListBox1.Items.Contains(deveui))
                    checkedListBox1.Items.Add(deveui);
            });
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (ISOpen)
            {
                if (TimeCount == 0)
                {
                    if (UdpServer.SendLoginBeatSend())
                    {
                        BeatSendTime = DateTime.Now;
                        IsSendBeat = true;
                        SendBeatTimeCount = 0;
                        TimeCount = 28;
                    }
                    else lab_img.BackColor = Color.Gray;
                }
                else TimeCount--;

                if (IsSendBeat)
                {
                    SendBeatTimeCount++;
                    if (SendBeatTimeCount == 5)
                    {
                        if ((UdpServer.BeatRcvTime - BeatSendTime).TotalSeconds >= 0 && (UdpServer.BeatRcvTime - BeatSendTime).TotalSeconds <= 5) lab_img.BackColor = Color.Green;
                        else lab_img.BackColor = Color.Gray;
                        IsSendBeat = false;
                    }
                }
            }
            else
            {
                TimeCount = 30;
            }
            time.Text = DateTime.Now.ToString();
        }

        private void LoraForm_Load(object sender, EventArgs e)
        {
            ReadIp();
        }

        private void DataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 8)
            {
                MessageBox.Show(dataGridView1.CurrentRow.Cells["strdata"].Value.ToString());
            }
            else if (e.ColumnIndex == 7)
            {
                MessageBox.Show(dataGridView1.CurrentRow.Cells["hexdata"].Value.ToString());
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow.Cells["strdata"].Value != null)
            {
                textBox1.Text = dataGridView1.CurrentRow.Cells["strdata"].Value.ToString();
            }
        }

        private void CheckedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> clist = new List<string>();
            for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
            {
                clist.Add(checkedListBox1.CheckedItems[i].ToString());
            }
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (clist.Contains(dataGridView1.Rows[i].Cells["moteeui"].Value.ToString()))
                {
                    dataGridView1.Rows[i].Visible = true;
                }
                else
                {
                    dataGridView1.Rows[i].Visible = false;
                }
            }
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex > -1)
            //{
            //    if (e.ColumnIndex == 8)
            //    {
            //        dataGridView1.Rows.Add();
            //        dataGridView1.Rows[e.RowIndex].Visible = false;
            //    }
            //}
        }
    }
}
