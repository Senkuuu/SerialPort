using System;
using System.Collections.Generic;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Configuration;

namespace Accenture.SerialPort
{
    /// <summary>
    /// 
    /// </summary>
    public class DBHelper
    {
        private static string conStr = "server=.;database=WSD.CS;uid=sa;pwd=cstr.1997";
        //private static string conStr = "server=192.168.1.126;database=WSD.CS;uid=sa;pwd=sa";

        /// <summary>
        /// 增删改 返回int类型
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int MyExecuteNonQuery(string sql)
        {
            using (SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                SqlCommand sqlcomm = new SqlCommand(sql, sqlcon);
                int result = sqlcomm.ExecuteNonQuery();
                return result;
            }
        }
        /// <summary>
        /// 查询单值 返回Object类型
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static object MyExecuteScalar(string sql)
        {
            using (

               SqlConnection sqlcon = new SqlConnection(conStr))
            {
                sqlcon.Open();
                SqlCommand sqlcomm = new SqlCommand(sql, sqlcon);
                object result = sqlcomm.ExecuteScalar();
                return result;
            }
        }
        /// <summary>
        /// 查询 返回SqlDataReader对象
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static SqlDataReader MyExecuteReader(string sql)
        {
            SqlConnection sqlcon = new SqlConnection(conStr);
            sqlcon.Open();
            SqlCommand sqlcomm = new SqlCommand(sql, sqlcon);
            SqlDataReader dr = sqlcomm.ExecuteReader(CommandBehavior.CloseConnection);
            return dr;
        }

        /// <summary>
        /// 导出数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql)
        {
            DataTable dt = null;
            try
            {
                SqlConnection sqlcon = new SqlConnection(conStr);
                sqlcon.Open();
                SqlCommand cmd = new SqlCommand(sql, sqlcon);

                SqlDataAdapter da = new SqlDataAdapter(sql, sqlcon);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }
                sqlcon.Close();
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///这个暂时没用到
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="ParametersList"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, Dictionary<string, object> ParametersList)
        {

            try
            {
                SqlConnection sqlcon = new SqlConnection(conStr);
                sqlcon.Open();
                SqlCommand cmd = new SqlCommand(sql, sqlcon);

                if (ParametersList != null && ParametersList.Count > 0)
                {
                    foreach (var item in ParametersList)
                    {
                        var dp = cmd.CreateParameter();
                        dp.ParameterName = item.Key;
                        dp.Value = item.Value;
                        cmd.Parameters.Add(dp);
                    }
                }

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


    }

}