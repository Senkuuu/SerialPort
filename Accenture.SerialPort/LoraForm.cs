﻿using System;
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
        private void LoraForm_Load(object sender, EventArgs e)
        {
            ReadIp();
        }

        #region 读取已保存IP地址
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
        #endregion

        #region 网关代理启动
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
        #endregion

        #region 返回数据显示
        /// <summary>
        /// 接收到数据由udpserver推送
        /// </summary>
        /// <param name="package">ASCS数据包</param>
        private void Udpserver_ShowEvent(ASCSPackage package)
        {
            string deveui = package.app?.moteeui;
            string errorcode = "";
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
                                errorcode = data.SubArray(12, 4).ToHexString();
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
                                outdata += "唤醒周期：" + Convert.ToInt32(data.SubArray(22, 4).ToHexString().ToUpper(), 16) + "秒\r\n";
                                outdata += "********************End*********************" + "\r\n";
                                outdata += "错误码：" + data.SubArray(26, 4).ToHexString().ToUpper() + "\r\n";
                                errorcode = data.SubArray(26, 4).ToHexString();
                                outdata += "回执指令：" + data.SubArray(30, 2).ToHexString().ToUpper() + "\r\n";
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
                                errorcode = data.SubArray(15, 4).ToHexString();
                                outdata += "回执指令：" + data.SubArray(14, 2).ToHexString().ToUpper() + "\r\n";
                                break;
                        }

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

                            //错误码不为00000000 整行醒目提示
                            if (errorcode != "" && errorcode != "00000000")
                            {
                                dgvr.DefaultCellStyle.BackColor = Color.Red;
                            }

                            string selsql = "select count(moteeui) from collectionTest where moteeui = '" + package.app.moteeui.ToString() + "'";
                            int count = (int)DBHelper.MyExecuteScalar(selsql);
                            //插入数据库
                            if (count > 0)
                            {
                                //更新数据
                                string updsql = string.Format(@"update collectionTest set systime='{0}',freq='{1}',rssi='{2}',datr='{3}',lsnr='{4}',hexdata='{5}',strdata='{6}'", package.app.gwrx[0].time.ToString(),
                                                package.app.motetx.freq.ToString(), package.app.gwrx[0].rssi.ToString(), package.app.motetx.datr, package.app.gwrx[0].lsnr.ToString(),
                                                data.ToHexString(), outdata);
                                DBHelper.MyExecuteNonQuery(updsql);
                            }
                            else
                            {
                                //插入数据
                                string inssql = string.Format(@"insert into collectionTest(systime,moteeui,freq,rssi,datr,lsnr,hexdata,strdata) 
                                            values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", package.app.gwrx[0].time.ToString(), package.app.moteeui.ToString(),
                                            package.app.motetx.freq.ToString(), package.app.gwrx[0].rssi.ToString(), package.app.motetx.datr, package.app.gwrx[0].lsnr.ToString(),
                                            data.ToHexString(), outdata);
                                DBHelper.MyExecuteNonQuery(inssql);
                            }
                            //勾选显示，没勾选隐藏
                            if (checkedListBox1.CheckedItems.Contains(package.app.moteeui.ToString()))
                            {
                                dgv.Rows[index].Visible = true;
                            }
                            else
                            {
                                dgv.Rows[index].Visible = false;
                            }
                            dgv.Refresh();
                            //向终端列表添加新的终端
                            if (!checkedListBox2.Items.Contains(package.app.moteeui.ToString()))
                            {
                                checkedListBox1.Items.Add(package.app.moteeui.ToString());
                                checkedListBox2.Items.Add(package.app.moteeui.ToString());
                            }
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
        #endregion

        #region 窗口左下角时间显示
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
        #endregion

        #region 数据网格点击事件
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
            //显示详细信息
            if (dataGridView1.CurrentRow.Cells["strdata"].Value != null)
            {
                textBox1.Text = dataGridView1.CurrentRow.Cells["strdata"].Value.ToString();
            }
        }
        #endregion

        #region 勾选隐藏或显示采集数据
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
        #endregion

        #region 终端列表过滤
        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            List<string> clist = new List<string>();
            for (int i = 0; i < checkedListBox2.Items.Count; i++)
            {
                clist.Add(checkedListBox2.Items[i].ToString());
            }
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                //查询栏没有内容时还原列表信息
                checkedListBox1.Items.Clear();
                for (int i = 0; i < clist.Count; i++)
                {
                    checkedListBox1.Items.Add(clist[i]);
                }
            }
            else
            {
                //过滤查询
                List<string> nlist = clist.Where(x => x.Contains(textBox3.Text)).ToList();
                checkedListBox1.Items.Clear();
                for (int i = 0; i < nlist.Count; i++)
                {
                    checkedListBox1.Items.Add(nlist[i]);
                }
            }
        }
        #endregion

        #region 终端列表清除
        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            checkedListBox2.Items.Clear();
        }
        #endregion

        #region 清除数据列表
        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            textBox1.Clear();
        }
        #endregion

        #region 窗口关闭事件
        private void LoraForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //关闭窗口时关闭网关通讯
            UdpServer.Stop();
        }
        #endregion
    }
}