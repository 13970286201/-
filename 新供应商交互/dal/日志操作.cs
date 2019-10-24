using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LitJson;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using 新供应商交互;
namespace http请求服务.dal
{
    class 日志操作
    {
        public static string CreatlogTable_客户()
        {
            string sdt = DateTime.Now.ToString("yyyy");
            string tablename = "log_" + sdt;

                string s_sql = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='" + tablename + "' and `table_schema`='userdata' ";
                object ob = MysqlHelper.ExecuteScalar(s_sql, ref Program.myPool_userdata);
                if (ob == null)
                {
                    string sql = "CREATE TABLE  IF NOT EXISTS `" + tablename + "`  LIKE  `log_mb`";
                    MysqlHelper.ExecuteNonQuery(sql, ref Program.myPool_userdata);

                }

            return tablename;
        }
        public static void 客户_日志操作(string 用户名, string 内容)
        {
            string tablename = CreatlogTable_客户();
            string log_sql = "insert `" + tablename + "`(username,time,text)values(?username,?time,?text)";

            MySqlParameter[] log_param = new MySqlParameter[]
                {
                new MySqlParameter("?username", 用户名),
                new MySqlParameter("?time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new MySqlParameter("?text", 内容)
                };
            MysqlHelper.ExecuteNonQuery(log_sql, ref Program.myPool_userdata, log_param);
        }


        public static string CreatlogTable_供应商()
        {
            string sdt = DateTime.Now.ToString("yyyy");
            string tablename = "log_" + sdt;

            string s_sql = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='" + tablename + "' and `table_schema`='supplier' ";
            object ob = MysqlHelper.ExecuteScalar(s_sql, ref Program.myPool_supplier);
            if (ob == null)
            {
                string sql = "CREATE TABLE  IF NOT EXISTS `" + tablename + "`  LIKE  `log_mb`";
                MysqlHelper.ExecuteNonQuery(sql, ref Program.myPool_supplier);

            }

            return tablename;
        }
        public static void 供应商_日志操作(string 用户名, string 内容)
        {
            string tablename = CreatlogTable_供应商();
            string log_sql = "insert `" + tablename + "`(username,time,text)values(?username,?time,?text)";

            MySqlParameter[] log_param = new MySqlParameter[]
                {
                new MySqlParameter("?username", 用户名),
                new MySqlParameter("?time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new MySqlParameter("?text", 内容)
                };
            MysqlHelper.ExecuteNonQuery(log_sql, ref Program.myPool_supplier, log_param);
        }

    }
}
