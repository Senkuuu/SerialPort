using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using Wima.Lora.Model;
using Wima.Lora;
using System.IO;
using System.Collections.Concurrent;
using Wima.Log;
using NSClient.Plugins;
using NPoco;
using WiYun.Data;
using System.Data;
using ServiceStack.Redis;
using System.Threading.Tasks;
using System.Configuration;
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
        //获取Redis服务器地址
        public static string path = ConfigurationManager.AppSettings["RedisPath"];
        public static string Port = ConfigurationManager.AppSettings["Port"];
        public static string Password = ConfigurationManager.AppSettings["Password"];
        //连接Redis服务器,path:服务器地址，Port:端口，Password：密码，访问的数据库
        public static RedisClient Redis = new RedisClient(path, int.Parse(Port), Password, 0);
        //缓存池
        PooledRedisClientManager prcm = new PooledRedisClientManager();
        RedisHelper help = new RedisHelper();
        private static bool redisError = false;
        //唤醒周期
        public string cycle { get; set; }

        /// <summary>
        /// 
        /// </summary>
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
                    #region Redis缓存设备、报警规则
                    String eqsql = "select * from WMS_PB_Equipment WHERE ISNULL(IsDeleted,0)=0 AND ISNULL(IsValid,0)= 1";
                    string rulesql = "SELECT  c.NO+a.AlarmSource No ,a.* \n" +
                                    "FROM    WMS_BT_AlarmRule a \n" +
                                            "LEFT JOIN WMS_BT_AlarmRulePosition b ON a.WMS_BT_AlarmRuleId = b.WMS_BT_AlarmRuleId \n" +
                                            "INNER JOIN WMS_PB_Position c ON c.NO LIKE '' + b.PositionNO + '%' \n" +
                                    "WHERE ISNULL(a.IsDeleted, 0) = 0 \n" +
                                            "AND ISNULL(b.IsDeleted, 0) = 0 \n" +
                                            "AND ISNULL(a.IsValid,0)= 1 \n" +
                                            "AND ISNULL(c.IsDeleted,0)=0 \n" +
                                            "AND ISNULL(c.IsValid,0)= 1 \n" +
                                    "ORDER BY c.NO desc ";
                    string bindsql = "SELECT  a.EquipmentSN+'bind' EquipmentSNs, a.* \n" +
                                    "FROM dbo.WMS_BT_EquipmentBind a\n" +
                                    "WHERE  ISNULL(MonitorState, 0) = 1 \n" +
                                    "AND ISNULL(IsDeleted,0)= 0";
                    DataTable eqlist = DBHelper.GetDataTable(eqsql);
                    DataTable rulelist = DBHelper.GetDataTable(rulesql);
                    DataTable bindlist = DBHelper.GetDataTable(bindsql);

                    redisError = redisError ? true : !help.DtToRedis(eqlist, "WMS_PB_Equipment", Redis);

                    redisError = redisError ? true : !help.DtToRedis(rulelist, "WMS_BT_AlarmRule", Redis);

                    redisError = redisError ? true : !help.DtToRedis(bindlist, "WMS_BT_EquipmentBind", Redis);
                    //if (!help.DtToRedis(eqlist, "WMS_PB_Equipment", Redis))
                    //    return;
                    //if (!help.DtToRedis(rulelist, "WMS_BT_AlarmRule", Redis))
                    //    return;
                    //if (!help.DtToRedis(bindlist, "WMS_BT_EquipmentBind", Redis))
                    //    return;
                    #endregion

                    //并行库启动
                    if (!redisError)
                    {
                        Start();
                    }

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
            int timestamp = 0;
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
                        #region 解析收到得数据
                        string moteid = data.SubArray(4, 4).ToHexString().ToUpper(); //短地址
                        string cmd = "0x" + new byte[] { data[10] }.ToHexString(); //指令
                        outdata += "地址：" + data.SubArray(4, 4).ToHexString().ToUpper() + "\r\n";
                        outdata += "外设启用：" + data.SubArray(8, 2).ToHexString().ToUpper() + "\r\n";
                        outdata += "指令码：" + cmd + "\r\n";
                        switch (cmd)
                        {
                            case "0x10":
                                outdata += "频段选择：" + data.SubArray(11, 1).ToHexString().ToUpper() + "\r\n";
                                outdata += "错误信息：" + errorCode(data.SubArray(12, 4).ToHexString()) + "\r\n";
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
                                timestamp = Convert.ToInt32(data.SubArray(18, 4).ToHexString().ToUpper(), 16);
                                outdata += "唤醒周期：" + Convert.ToInt32(data.SubArray(22, 4).ToHexString().ToUpper(), 16) + "秒\r\n";
                                outdata += "********************End*********************" + "\r\n";
                                outdata += "错误信息：" + errorCode(data.SubArray(26, 4).ToHexString()) + "\r\n";
                                errorcode = data.SubArray(26, 4).ToHexString();
                                outdata += "回执指令：" + data.SubArray(30, 2).ToHexString().ToUpper() + "\r\n";
                                break;
                            case "0x12":
                                outdata += "标定类型：" + data.SubArray(11, 1).ToHexString().ToUpper() + "\r\n";
                                outdata += "接收的标定值：" + data.SubArray(12, 2).ToHexString().ToUpper() + "\r\n";
                                outdata += "采集的标定值：" + data.SubArray(14, 2).ToHexString().ToUpper() + "\r\n";
                                outdata += "温度已经标定数量：" + data.SubArray(16, 1).ToHexString().ToUpper() + "\r\n";
                                outdata += "湿度已经标定数量：" + data.SubArray(17, 1).ToHexString().ToUpper() + "\r\n";
                                outdata += "错误信息：" + errorCode(data.SubArray(18, 4).ToHexString()) + "\r\n";
                                outdata += "回执指令：" + data.SubArray(22, 2).ToHexString().ToUpper() + "\r\n";
                                break;
                            case "0x13":
                                outdata += "需要接收的下一个程序帧：" + data.SubArray(11, 2).ToHexString().ToUpper() + "\r\n";
                                outdata += "程序下载总帧：" + data.SubArray(13, 2).ToHexString().ToUpper() + "\r\n";
                                outdata += "错误信息：" + errorCode(data.SubArray(15, 4).ToHexString()) + "\r\n";
                                errorcode = data.SubArray(15, 4).ToHexString();
                                outdata += "回执指令：" + data.SubArray(14, 2).ToHexString().ToUpper() + "\r\n";
                                break;
                        }
                        #endregion

                        //是否存在
                        bool isexist = false;
                        DataGridView dgv = dataGridView1;
                        this.Invoke((Action)delegate
                        {
                            #region 不采取唯一显示
                            int index = dgv.Rows.Add();
                            DataGridViewRow dgvr = dataGridView1.Rows[index];
                            //序号
                            dgvr.Cells["index"].Value = "#0";
                            //唤醒方式
                            dgvr.Cells["wakeuptype"].Value = data.SubArray(12, 1).ToHexString().ToUpper() == "01" ? "自动唤醒" : "手动唤醒";
                            //系统时间
                            dgvr.Cells["systime"].Value = package.app.gwrx[0].time.ToString();
                            //终端ID
                            dgvr.Cells["moteeui"].Value = package.app.moteeui.ToString();
                            //接收频率
                            dgvr.Cells["freq"].Value = package.app.motetx.freq.ToString();
                            //电量
                            dgvr.Cells["power"].Value = Convert.ToInt32(data.SubArray(13, 1).ToHexString().ToUpper(), 16);
                            //信号强度
                            dgvr.Cells["rssi"].Value = package.app.gwrx[0].rssi.ToString();
                            //速率
                            dgvr.Cells["datr"].Value = package.app.motetx.datr;
                            //信噪比
                            dgvr.Cells["lsnr"].Value = package.app.gwrx[0].lsnr.ToString();
                            //唤醒周期
                            dgvr.Cells["wakeup"].Value = Convert.ToInt32(data.SubArray(22, 4).ToHexString().ToUpper(), 16) + "秒";
                            //温度
                            dgvr.Cells["temp"].Value = Convert.ToInt32(data.SubArray(14, 2).ToHexString().ToUpper(), 16);
                            //湿度
                            dgvr.Cells["hum"].Value = Convert.ToInt32(data.SubArray(16, 2).ToHexString().ToUpper(), 16);
                            //错误码
                            dgvr.Cells["ercode"].Value = errorCode(data.SubArray(26, 4).ToHexString());
                            //16进制数据
                            dgvr.Cells["hexdata"].Value = data.ToHexString();
                            //字符串数据
                            dgvr.Cells["strdata"].Value = outdata;


                            #region 采集程序数据传输
                            try
                            {
                                newAsEquipData request = new newAsEquipData();

                                request.id = Guid.NewGuid().ToString();

                                request.moteid = package.app.moteeui.ToString();

                                request.power = Convert.ToInt32(data.SubArray(13, 1).ToHexString().ToUpper(), 16);

                                request.wakeupmode = Convert.ToInt32(data.SubArray(22, 4).ToHexString().ToUpper(), 16) / 60;

                                request.testType = Convert.ToInt32(data.SubArray(12, 1).ToHexString());

                                request.version = Convert.ToInt32(data.SubArray(13, 1).ToHexString().ToUpper(), 16);

                                request.Details = new List<NSClient.AsEquipDataDT>();

                                NSClient.AsEquipDataDT dt = new NSClient.AsEquipDataDT(request.id, request.moteid, new NSClient.NsDataDT());

                                dt.Temperature = Convert.ToInt32(data.SubArray(14, 2).ToHexString().ToUpper(), 16);

                                dt.Humidity = Convert.ToInt32(data.SubArray(16, 2).ToHexString().ToUpper(), 16);

                                dt.DataTime = DateTime.Now;

                                request.Details.Add(dt);

                                if (!redisError)
                                {
                                    //加入队列
                                    Push(request);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                throw;
                            }
                            #endregion

                            //错误码不为00000000 整行醒目提示
                            if (errorcode != "" && errorcode != "00000000")
                            {
                                dgvr.DefaultCellStyle.BackColor = Color.Red;
                            }

                            //向终端列表添加新的终端
                            if (!checkedListBox2.Items.Contains(package.app.moteeui.ToString()))
                            {
                                checkedListBox1.Items.Add(package.app.moteeui.ToString());
                                checkedListBox1.SetItemChecked(checkedListBox1.Items.Count - 1, true);
                                checkedListBox2.Items.Add(package.app.moteeui.ToString());
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
                            #endregion

                            #region 判断终端信息是否存在，终端信息只显示最新的一条
                            //for (int i = 0; i < dgv.Rows.Count; i++)
                            //{
                            //    if (dgv.Rows[i].Cells["moteeui"].Value.ToString() == package.app.moteeui.ToString())
                            //    {
                            //        DataGridViewRow dgvr1 = dataGridView1.Rows[i];
                            //        //序号
                            //        dgvr1.Cells["index"].Value = "#0";
                            //        //系统时间
                            //        dgvr1.Cells["systime"].Value = package.app.gwrx[0].time.ToString();
                            //        //终端ID
                            //        dgvr1.Cells["moteeui"].Value = package.app.moteeui.ToString();
                            //        //接收频率
                            //        dgvr1.Cells["freq"].Value = package.app.motetx.freq.ToString();
                            //        //信号强度
                            //        dgvr1.Cells["rssi"].Value = package.app.gwrx[0].rssi.ToString();
                            //        //速率
                            //        dgvr1.Cells["datr"].Value = package.app.motetx.datr;
                            //        //信噪比
                            //        dgvr1.Cells["lsnr"].Value = package.app.gwrx[0].lsnr.ToString();
                            //        //16进制数据
                            //        dgvr1.Cells["hexdata"].Value = data.ToHexString();
                            //        //字符串数据
                            //        dgvr1.Cells["strdata"].Value = outdata;

                            //        //错误码不为00000000 整行醒目提示
                            //        if (errorcode != "" && errorcode != "00000000")
                            //        {
                            //            dgvr1.DefaultCellStyle.BackColor = Color.Red;
                            //        }
                            //        //勾选显示，没勾选隐藏
                            //        if (checkedListBox1.CheckedItems.Contains(package.app.moteeui.ToString()))
                            //        {
                            //            dgv.Rows[i].Visible = true;
                            //        }
                            //        else
                            //        {
                            //            dgv.Rows[i].Visible = false;
                            //        }

                            //        isexist = true;

                            //        dgv.Refresh();
                            //    }
                            //}

                            ////判断是否存在
                            //if (!isexist)
                            //{
                            //    int index = dgv.Rows.Add();
                            //    DataGridViewRow dgvr = dataGridView1.Rows[index];
                            //    //序号
                            //    dgvr.Cells["index"].Value = "#0";
                            //    //系统时间
                            //    dgvr.Cells["systime"].Value = package.app.gwrx[0].time.ToString();
                            //    //终端ID
                            //    dgvr.Cells["moteeui"].Value = package.app.moteeui.ToString();
                            //    //接收频率
                            //    dgvr.Cells["freq"].Value = package.app.motetx.freq.ToString();
                            //    //信号强度
                            //    dgvr.Cells["rssi"].Value = package.app.gwrx[0].rssi.ToString();
                            //    //速率
                            //    dgvr.Cells["datr"].Value = package.app.motetx.datr;
                            //    //信噪比
                            //    dgvr.Cells["lsnr"].Value = package.app.gwrx[0].lsnr.ToString();
                            //    //16进制数据
                            //    dgvr.Cells["hexdata"].Value = data.ToHexString();
                            //    //字符串数据
                            //    dgvr.Cells["strdata"].Value = outdata;

                            //    //错误码不为00000000 整行醒目提示
                            //    if (errorcode != "" && errorcode != "00000000")
                            //    {
                            //        dgvr.DefaultCellStyle.BackColor = Color.Red;
                            //    }

                            //    //勾选显示，没勾选隐藏
                            //    if (checkedListBox1.CheckedItems.Contains(package.app.moteeui.ToString()))
                            //    {
                            //        dgv.Rows[index].Visible = true;
                            //    }
                            //    else
                            //    {
                            //        dgv.Rows[index].Visible = false;
                            //    }

                            //    dgv.Refresh();
                            //}
                            #endregion

                            #region 数据库操作
                            string selsql = "select count(moteeui) from collectionTest where moteeui = '" + package.app.moteeui.ToString() + "'";
                            int count = (int)DBHelper.MyExecuteScalar(selsql);
                            //插入数据库     ————————————     2020/2/18 新增手动唤醒不需要存数据库
                            if (data.SubArray(12, 1).ToHexString().ToUpper() == "01")
                            {
                                if (count > 0)
                                {
                                    //更新数据
                                    string updsql = string.Format(@"update collectionTest set systime='{0}',freq='{1}',rssi='{2}',datr='{3}',lsnr='{4}',hexdata='{5}',strdata='{6}' where moteeui='{7}'", package.app.gwrx[0].time.ToString(),
                                                    package.app.motetx.freq.ToString(), package.app.gwrx[0].rssi.ToString(), package.app.motetx.datr, package.app.gwrx[0].lsnr.ToString(),
                                                    data.ToHexString(), outdata, package.app.moteeui.ToString());
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

                                    #region 下发时间效准数据协议
                                    string wd = "";
                                    string res = "";

                                    int sumData = 0;
                                    string Validation = "";
                                    #region 时间戳换算
                                    string st = "";
                                    int t1 = Convert.ToInt32(ToTimeStamp(DateTime.Now)) - timestamp > 0 ? Convert.ToInt32(ToTimeStamp(DateTime.Now)) - timestamp : (Convert.ToInt32(ToTimeStamp(DateTime.Now)) - timestamp) * -1;
                                    string stamp = Convert.ToString(t1, 16);
                                    stamp = stamp.Length % 2 == 0 ? stamp : "0" + stamp;
                                    if (stamp.Length < 8)
                                    {
                                        for (int i = 0; i < 7 - stamp.Length; i++)
                                        {
                                            st = "0" + st;
                                        }
                                    }

                                    st = Convert.ToInt32(ToTimeStamp(DateTime.Now)) - timestamp > 0 ? "0" + st + stamp : "8" + st + stamp;
                                    #endregion

                                    #region 下发数据拼接
                                    string WakeupData = Convert.ToString(Convert.ToInt32(cycle), 16).Length % 2 == 0 ? Convert.ToString(Convert.ToInt32(cycle), 16) : "0" + Convert.ToString(Convert.ToInt32(cycle), 16);
                                    if (WakeupData.Length < 8)
                                    {
                                        for (int i = 0; i < 8 - WakeupData.Length; i++)
                                        {
                                            wd += "0";
                                        }
                                    }
                                    wd += WakeupData;
                                    Random rd = new Random();
                                    string re = Convert.ToString(rd.Next(1, Convert.ToInt32("FFFF", 16)), 16);
                                    if (re.Length < 4)
                                    {
                                        for (int i = 0; i < 4 - re.Length; i++)
                                        {
                                            res += "0";
                                        }
                                    }
                                    res += re;
                                    string Data = Convert.ToString(30, 16).ToUpper() + data.SubArray(4, 4).ToHexString().ToUpper() + "51" + st +
                                                     wd.ToUpper() + "00000000" + "0000" + res.ToUpper();
                                    for (int i = 0; i < Data.Length / 2; i++)
                                    {
                                        sumData += Convert.ToInt32(Data.Substring(i * 2, 2), 16);
                                    }
                                    Validation = Convert.ToString(sumData, 16);
                                    string vd = Validation.Length % 2 == 0 ? Validation : "0" + Validation;
                                    if (vd.Length < 4)
                                    {
                                        for (int i = 0; i <= 4 - vd.Length; i++)
                                        {
                                            vd = "0" + Validation;
                                        }
                                    }
                                    #endregion

                                    Send(package.app.moteeui, strToHexByte("FD0D0A" + Data + vd.ToUpper() + "0D0ADF"));
                                    SendShow(@"FD0D0A" + Data + Validation.ToUpper() + "0D0ADF");
                                    #endregion
                                }
                            }
                            #endregion


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

        /// <summary>
        /// 将时间转换为时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public long ToTimeStamp(DateTime dateTime)
        {
            var TimeStamps = (dateTime.Ticks - 621355968000000000) / 10000000;
            return TimeStamps;
        }

        private void SendShow(string str)
        {
            listBox1.Items.Insert(0, "[" + DateTime.Now.ToString() + "]提交数据:" + str);
            listBox1.BeginUpdate();
            if (listBox1.Items.Count >= 100)
                listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
            listBox1.EndUpdate();
        }
        #endregion

        #region 发送数据
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="deveui">要发送的数据</param>
        /// <param name="data">是否需要保存到已发送的列表中</param>
        private void Send(string deveui, byte[] data)
        {
            try
            {
                if (UdpServer != null)
                    UdpServer.SendPacket(deveui, data);
            }
            catch (Exception ex) { log.Error(ex); }
        }
        #endregion

        #region 判断码错误类型
        private string errorCode(string errorcode)
        {
            string result = "";
            switch (errorcode)
            {
                case "00000000":
                    result = "无错误";
                    break;
                case "00000001":
                    result = "内置EEPROM操作故障";
                    break;
                case "00000002":
                    result = "外置EEPROM操作故障";
                    break;
                case "00000004":
                    result = "采集电池电压故障";
                    break;
                case "00000008":
                    result = "电池电量拉低报警";
                    break;
                case "00000010":
                    result = "时间设置故障";
                    break;
                case "00000020":
                    result = "唤醒周期配置故障";
                    break;
                case "00000040":
                    result = "标定配置错误";
                    break;
                case "02000000":
                    result = "接收数据ID错误";
                    break;
                case "04000000":
                    result = "接收的数据校验错误 ";
                    break;
                case "08000000":
                    result = "数据解析错误（不在解析范围内）";
                    break;
                case "10000000":
                    result = "程序更新错误";
                    break;
                case "20000000":
                    result = "至少一个传感器故障";
                    break;
                case "40000000":
                    result = "其他错误";
                    break;
                default:
                    result = "";
                    break;
            }
            return result;
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
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Visible = false;
            }
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
            if (UdpServer?.UdpServer != null) UdpServer.Stop();
        }
        #endregion

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(listBox1.SelectedItem.ToString());
        }

        #region 唤醒方式的显示隐藏
        private void Cbox_manual_CheckedChanged(object sender, EventArgs e)
        {
            cbox_CheckedChanged();
        }

        private void Cbox_auto_CheckedChanged(object sender, EventArgs e)
        {
            cbox_CheckedChanged();
        }

        private void cbox_CheckedChanged()
        {
            if ((cbox_manual.Checked && cbox_auto.Checked) || (!cbox_manual.Checked && !cbox_auto.Checked))//同时选择或者都不选——显示全部
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
            else if (cbox_auto.Checked)//只选择自动唤醒——只显示自动唤醒的数据
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells["wakeuptype"].Value.ToString() == "自动唤醒")
                    {
                        dataGridView1.Rows[i].Visible = true;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Visible = false;
                    }
                }
            }
            else if (cbox_manual.Checked)//只选择手动唤醒——只显示手动唤醒的数据
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (dataGridView1.Rows[i].Cells["wakeuptype"].Value.ToString() == "手动唤醒")
                    {
                        dataGridView1.Rows[i].Visible = true;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Visible = false;
                    }
                }
            }
        }
        #endregion

        #region 消息队列

        private static ConcurrentQueue<newAsEquipData> _queues = new ConcurrentQueue<newAsEquipData>();

        /// <summary>
        /// 
        /// </summary>
        public static void Start()
        {
            //Start(() => { Running(); });
            new Thread(() => { Running(); }).Start();
        }

        internal static void Push(newAsEquipData newAs)
        {
            Task.Factory.StartNew((d) => { _queues.Enqueue((newAsEquipData)d); }, newAs);
        }

        private static void Running()
        {
            while (true)
            {
                if (_queues.IsEmpty)
                    Thread.Sleep(5000);
                else
                {
                    newAsEquipData request;
                    if (_queues.TryDequeue(out request))
                    {
                        try
                        {
                            using (var db = new NDatabase())
                            {
                                ApiCall ac = new ApiCall();
                                ac.SaveDataMethod(db, request, Redis);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }
        #endregion

        #region 压力测试udp下发
        /// <summary>
        /// 压力测试udp下发
        /// </summary>
        private void UdpSend()
        {
            UDPMan udp = new UDPMan(null, "", new DnsEndPoint("0.0.0.0", 1701));

            udp.UDPSend(new byte[] { 0 }, new DnsEndPoint("0.0.0.0", 1701));
        }
        #endregion
    }
}
