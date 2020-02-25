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
        /// Redis缓存
        /// </summary>
        /// <param name="dtb"></param>
        /// <returns></returns>
        public bool DtToRedis(DataTable dtb, string type)
        {
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
                //WMS_PB_Equipment value = null;
                try
                {
                    Redis.Set(dr[""+key+""].ToString(), drow);
                    //value = Redis.Get<WMS_PB_Equipment>("key");//读取Redis
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }

            return true;
        }
    }
}
