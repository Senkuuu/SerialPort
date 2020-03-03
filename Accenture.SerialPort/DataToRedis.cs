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
        /// <summary>
        /// Redis缓存
        /// </summary>
        /// <param name="dtb"></param>
        /// <returns></returns>
        public bool DtToRedis(DataTable dtb, string type, RedisClient Redis)
        {
            string key = "";
            if (type.Equals("WMS_PB_Equipment"))
                key = "SN";
            else if (type.Equals("WMS_BT_AlarmRule"))
                key = "NO";
            else if (type.Equals("WMS_BT_EquipmentBind"))
                key = "EquipmentSNs";
            foreach (DataRow dr in dtb.Rows)
            {
                Dictionary<string, object> drow = new Dictionary<string, object>();
                foreach (DataColumn dc in dtb.Columns)
                {
                    drow.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                //WMS_BT_EquipmentBind value = null;
                try
                {
                    Redis.Set(dr[""+key+""].ToString(), drow);
                    //value = Redis.Get<WMS_BT_EquipmentBind>(dr["" + key + ""].ToString());//读取Redis
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
