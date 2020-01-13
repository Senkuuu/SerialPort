using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Wima.Log;

namespace Accenture.SerialPort
{
    public class RealMoteState
    {
        public string DevEui { get; set; }
        public string DevAddr { get; set; }

        public long Time { get; set; }

        public float LastTP { get; set; }
        public float LastHm { get; set; }
        public int BT { get; set; }

        public int CR { get; set; }
        public int SR { get; set; }
        public int CKBJ { get; set; }
        public DateTime LastRcvTime { get; set; } = DateTime.Now;
        public int Version { get; set; }
        public ulong TimeInt { get; set;}
        public RealMoteState()
        {

        }
    }

    public class RcvBase
    {
        public string Cmd { get; set; }
        public string MoteId { get; set; }
        public string DataStr { get; set; }
        public DateTime RcvDate { get; set; }

        public RcvBase() { }
        public RcvBase(string _cmd, string _moteid, string _datastr, DateTime _rcvdate)
        {
            Cmd = _cmd;
            MoteId = _moteid;
            DataStr = _datastr;
            RcvDate = _rcvdate;
        }

    }

    public class TempClass
    {
        public string AutoId { get { return Moteid + (RcvDate.ToString("yyyyMMddHHmm")); } }

        public string Moteid { get; set; }

        public float Temp { get; set; }
        public DateTime RcvDate { get; set; } = DateTime.Parse("1970/01/01");
        public TempClass(string _Moteid, float _Temp, DateTime _RcvDate)
        {
            Moteid = _Moteid;
            Temp = _Temp;
            RcvDate = _RcvDate;
        }
    }

    public class HumClass
    {
        public string AutoId { get { return Moteid + (RcvDate.ToString("yyyyMMddHHmm")); } }

        public string Moteid { get; set; }

        public float Hum { get; set; }
        public DateTime RcvDate { get; set; } = DateTime.Parse("1970/01/01");
        public HumClass(string _Moteid, float _Hum, DateTime _RcvDate)
        {
            Moteid = _Moteid;
            Hum = _Hum;
            RcvDate = _RcvDate;
        }
    }

    public class RcvDateSave
    {
        public LogMan log { get { return UdpMan.log; } }
        public string ConnStr { get; set; }
        public RcvDateSave(string _connstr)
        {
            ConnStr = _connstr;
        }
        public void TempInsert(TempClass temp)
        {
            try
            {
                DateTime dt = DateTime.Now;
                using (SqlConnection sql = new SqlConnection(ConnStr))
                {
                    sql.Open();
                    string str = "if not exists (select * from TempTable where AutoId=@a) insert into temptable(autoid,moteid,temp,rcvdate) values(@t1,@t2,@t3,@t4) ";
                    SqlCommand com = new SqlCommand(str, sql);
                    com.Parameters.AddWithValue("@a", temp.AutoId);
                    com.Parameters.AddWithValue("@t1", temp.AutoId);
                    com.Parameters.AddWithValue("@t2", temp.Moteid);
                    com.Parameters.AddWithValue("@t3", temp.Temp);
                    com.Parameters.AddWithValue("@t4", temp.RcvDate);
                    com.ExecuteNonQuery();
                }
                Console.WriteLine("Insert Temp Time(ms)=" + (DateTime.Now - dt).TotalMilliseconds);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public Dictionary<DateTime, float> TempSelect(string moteid,DateTime date)
        {
            try
            {
                Dictionary<DateTime, float> ddf = new Dictionary<DateTime, float>();
                DateTime dt = DateTime.Now;
                using (SqlConnection sql = new SqlConnection(ConnStr))
                {
                    sql.Open();
                    string str = "select temp,rcvdate from temptable where rcvdate>=@a and rcvdate<=@b and moteid=@c order by rcvdate ";
                    SqlCommand com = new SqlCommand(str, sql);
                    com.Parameters.AddWithValue("@a", DateTime.Parse(date.ToString("yyyy-MM-dd")));
                    com.Parameters.AddWithValue("@b", DateTime.Parse(date.AddDays(1).ToString("yyyy-MM-dd")));
                    com.Parameters.AddWithValue("@c", moteid);
                    SqlDataReader dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        ddf.Add(DateTime.Parse(dr[1].ToString()),Convert.ToSingle(dr[0].ToString()));
                    }
                }
                Console.WriteLine("Select Temp Time(ms)=" + (DateTime.Now - dt).TotalMilliseconds);
                return ddf;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Dictionary<DateTime, float>();
            }
        }

        public void HumInsert(HumClass hum)
        {
            try
            {
                DateTime dt = DateTime.Now;
                using (SqlConnection sql = new SqlConnection(ConnStr))
                {
                    sql.Open();
                    string str = "if not exists (select * from humTable where AutoId=@a) insert into humtable(autoid,moteid,hum,rcvdate) values(@t1,@t2,@t3,@t4) ";
                    SqlCommand com = new SqlCommand(str, sql);
                    com.Parameters.AddWithValue("@a", hum.AutoId);
                    com.Parameters.AddWithValue("@t1", hum.AutoId);
                    com.Parameters.AddWithValue("@t2", hum.Moteid);
                    com.Parameters.AddWithValue("@t3", hum.Hum);
                    com.Parameters.AddWithValue("@t4", hum.RcvDate);
                    com.ExecuteNonQuery();
                }
                Console.WriteLine("Insert Hum Time(ms)=" + (DateTime.Now - dt).TotalMilliseconds);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public Dictionary<DateTime, float> HumSelect(string moteid,DateTime date)
        {
            try
            {
              
                Dictionary<DateTime, float> ddf = new Dictionary<DateTime, float>();
                DateTime dt = DateTime.Now;
                using (SqlConnection sql = new SqlConnection(ConnStr))
                {
                    sql.Open();
                    string str = "select hum,rcvdate from humtable where rcvdate>=@a and rcvdate<=@b and moteid=@c order by rcvdate";
                    SqlCommand com = new SqlCommand(str, sql);
                    com.Parameters.AddWithValue("@a", DateTime.Parse(date.ToString("yyyy-MM-dd")));
                    com.Parameters.AddWithValue("@b", DateTime.Parse(date.AddDays(1).ToString("yyyy-MM-dd")));
                    com.Parameters.AddWithValue("@c", moteid);
                    SqlDataReader dr = com.ExecuteReader();
                    while (dr.Read())
                    {
                        ddf.Add(DateTime.Parse(dr[1].ToString()), float.Parse(dr[0].ToString()));
                    }
                }
                Console.WriteLine("Select Hum Time(ms)=" + (DateTime.Now - dt).TotalMilliseconds);
                return ddf;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return new Dictionary<DateTime, float>();
            }
        }

        public bool BaseDataInsert(RcvBase rb)
        {
            try
            {
                DateTime dt = DateTime.Now;
                using (SqlConnection sql = new SqlConnection(ConnStr))
                {
                    sql.Open();
                    string str = "insert into rcvbasetable(cmd,moteid,datastr,rcvdate) values(@c,@m,@d,@r)";
                    SqlCommand com = new SqlCommand(str, sql);
                    com.Parameters.AddWithValue("@c", rb.Cmd);
                    com.Parameters.AddWithValue("@m", rb.MoteId);
                    com.Parameters.AddWithValue("@d", rb.DataStr);
                    com.Parameters.AddWithValue("@r", rb.RcvDate);
                    int i = com.ExecuteNonQuery();
                    Console.WriteLine("Insert BaseData Time(ms)=" + (DateTime.Now - dt).TotalMilliseconds);
                    return i > 0;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public bool RealTUpdateOrInsert(string moteid, float temp, float hum, int bt, int outw, int v)
        {
            try
            {
               
                DateTime dt = DateTime.Now;
                using (SqlConnection sql = new SqlConnection(ConnStr))
                {
                    sql.Open();
                    string str = "if not exists (select * from realtimemote where moteid=@a) insert into realtimemote(moteid,temp,hum,bt,outwarn,version,lastupdatetime) values(@t1,@t2,@t3,@t4,@t5,@t6,@t7) else update realtimemote set temp=@t8,hum=@t9,bt=@t10,outwarn=@t11,version=@t12,lastupdatetime=@t13 where moteid=@t14";
                    SqlCommand com = new SqlCommand(str, sql);
                    com.Parameters.AddWithValue("@a", moteid);
                    com.Parameters.AddWithValue("@t1", moteid);
                    com.Parameters.AddWithValue("@t2", temp);
                    com.Parameters.AddWithValue("@t3", hum);
                    com.Parameters.AddWithValue("@t4", bt);
                    com.Parameters.AddWithValue("@t5", outw);
                    com.Parameters.AddWithValue("@t6", v);
                    com.Parameters.AddWithValue("@t7", DateTime.Now);
                    com.Parameters.AddWithValue("@t8", temp);
                    com.Parameters.AddWithValue("@t9", hum);
                    com.Parameters.AddWithValue("@t10", bt);
                    com.Parameters.AddWithValue("@t11", outw);
                    com.Parameters.AddWithValue("@t12", v);
                    com.Parameters.AddWithValue("@t13", DateTime.Now);
                    com.Parameters.AddWithValue("@t14", moteid);
                    int i = com.ExecuteNonQuery();
                    Console.WriteLine("Insert BaseData1 Time(ms)=" + (DateTime.Now - dt).TotalMilliseconds);
                    return i > 0;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }
        public bool RealTUpdateOrInsert(string moteid, int fsk, int rd, int power)
        {
            try
            {
                DateTime dt = DateTime.Now;
                using (SqlConnection sql = new SqlConnection(ConnStr))
                {
                    sql.Open(); 
                    string str = "if exists (select * from realtimetable where moteid=@a) insert into realtimetable(moteid,fsk,stateradio,sendpower,lastupdatetime) values(@t1,@t2,@t3,@t4,@t5) else update realtimetable set fsk=@t6,stateradio=@t7,sendpower=@t8,lastupdatetime=@t9 where moteid=@t10";
                    SqlCommand com = new SqlCommand(str, sql);
                    com.Parameters.AddWithValue("@a", moteid);
                    com.Parameters.AddWithValue("@t1", moteid);
                    com.Parameters.AddWithValue("@t2", fsk);
                    com.Parameters.AddWithValue("@t3", rd);
                    com.Parameters.AddWithValue("@t4", power);
                    com.Parameters.AddWithValue("@t5", DateTime.Now);
                    com.Parameters.AddWithValue("@t6", fsk);
                    com.Parameters.AddWithValue("@t7", rd);
                    com.Parameters.AddWithValue("@t8", power);
                    com.Parameters.AddWithValue("@t9", DateTime.Now);
                    com.Parameters.AddWithValue("@t10", moteid);
                    int i = com.ExecuteNonQuery();
                    Console.WriteLine("Insert BaseData2 Time(ms)=" + (DateTime.Now - dt).TotalMilliseconds);
                    return i > 0;
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }
    }

    public class CSConfig
    {
        /// <summary>
        /// 发送周期
        /// </summary>
        public int SendCycle { get; set; } = 30;
        /// <summary>
        /// 测温周期
        /// </summary>
        public int CwCycle { get; set; } = 30;

        public int OutKU { get; set; } = 1;

        public int Radio { get; set; } = 1;

        public int Power { get; set; } = 6;

        public bool IsAuto { get; set; } = true;

        public int TimeJzInt { get; set; } = 3;

    }
}
