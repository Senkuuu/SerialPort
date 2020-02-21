using Nancy.Json;
using NPOI.SS.Formula.Functions;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WiYun.Entities;

namespace Accenture.SerialPort
{
    public class RedisHelper
    {

        //获取Redis服务器地址
        public static string path = ConfigurationManager.AppSettings["RedisPath"];
        public static string Port = ConfigurationManager.AppSettings["Port"];
        public static string Password = ConfigurationManager.AppSettings["Password"];
        //连接Redis服务器,path:服务器地址，Port:端口，Password：密码，访问的数据库
        public RedisClient Redis = new RedisClient(path, int.Parse(Port), Password, 0);
        //缓存池
        PooledRedisClientManager prcm = new PooledRedisClientManager();

        /// <summary>
        /// DataTable转换json
        /// </summary>
        /// <param name="dtb"></param>
        /// <returns></returns>
        public bool DtToJson(DataTable dtb, string type)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //System.Collections.ArrayList dic = new System.Collections.ArrayList();
            string key = "";
            if (type.Equals("WMS_PB_Equipment"))
                key = "SN";
            else if (type.Equals("WMS_BT_AlarmRule"))
                key = "NO";
            foreach (DataRow dr in dtb.Rows)
            {
                Dictionary<string, object> drow = new Dictionary<string, object>();
                foreach (DataColumn dc in dtb.Columns)
                {
                    drow.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                //dic.Add(drow);
                var str = jss.Serialize(drow);//序列化
                try
                {
                    Redis.Set(dr[""+key+""].ToString(), str);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
                //WMS_PB_Equipment  data= jss.Deserialize<WMS_PB_Equipment>(str);//反序列化
            }

            return true;
        }
    }
}
