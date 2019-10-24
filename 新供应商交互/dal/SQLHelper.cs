using System;
using System.Data;
using System.Data.SqlClient;

namespace http请求服务.dal
{
    class SQLHelper
    {

        #region 执行sql字符串
        /// <summary> 
        /// 执行不带参数的SQL语句 
        /// </summary> 
        /// <param name="Sqlstr"></param> 
        /// <returns></returns> 
        public static int ExecuteSql(String Sqlstr, ref MSSql_Pool mypool)
        {
            SqlConnection conn = mypool.BorrowDBConnection();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 720;
            cmd.Connection = conn;
            cmd.CommandText = Sqlstr;

            cmd.ExecuteNonQuery();


            mypool.ReturnDBConnection(conn);
            return 1;

        }
        /// <summary> 
        /// 执行带参数的SQL语句 
        /// </summary> 
        /// <param name="Sqlstr">SQL语句</param> 
        /// <param name="param">参数对象数组</param> 
        /// <returns></returns> 
        public static int ExecuteSql(String Sqlstr, SqlParameter[] param, ref MSSql_Pool mypool)
        {

            SqlConnection conn = mypool.BorrowDBConnection();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 720;
            cmd.Connection = conn;
            cmd.CommandText = Sqlstr;
            cmd.Parameters.AddRange(param);
            cmd.ExecuteNonQuery();


            mypool.ReturnDBConnection(conn);
            return 1;

        }


        /// <summary> 
        /// 执行SQL语句并返回数据表 
        /// </summary> 
        /// <param name="Sqlstr">SQL语句</param> 
        /// <returns></returns> 
        public static DataTable ExecuteDt(String Sqlstr, ref MSSql_Pool mypool)
        {
            SqlConnection conn = mypool.BorrowDBConnection();

            System.Data.SqlClient.SqlCommand comm = new System.Data.SqlClient.SqlCommand(Sqlstr, conn);
            comm.CommandTimeout = 720;


            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = comm;

            DataTable dt = new DataTable();
            da.Fill(dt);


            mypool.ReturnDBConnection(conn);
            return dt;

        }
        /// <summary> 
        /// 执行SQL语句并返回数据表 
        /// </summary> 
        /// <param name="Sqlstr">SQL语句</param> 
        /// <param name="param">参数对象数组</param>
        /// <returns></returns> 
        public static DataTable ExecuteDt(String Sqlstr, SqlParameter[] param, ref MSSql_Pool mypool)
        {
            SqlConnection conn = mypool.BorrowDBConnection();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = 720;
            cmd.Connection = conn;
            foreach (SqlParameter p in param)
            {
                cmd.Parameters.Add(p);
            }
            cmd.CommandText = Sqlstr;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            mypool.ReturnDBConnection(conn);
            return dt;

        }
        /// <summary>
        /// 返回object 返回结果第一行第一列
        /// </summary>
        /// <param name="Sqlstr"></param>
        /// <returns></returns>
        public static object ExecuteScalar(String Sqlstr, ref MSSql_Pool mypool)
        {
            SqlConnection conn = mypool.BorrowDBConnection();

            SqlCommand cmd = new SqlCommand(Sqlstr, conn);
            object obj = cmd.ExecuteScalar();

            mypool.ReturnDBConnection(conn);
            return obj;
        }
        /// <summary> 
        /// 执行SQL语句并返回DataSet 
        /// </summary> 
        /// <param name="Sqlstr">SQL语句</param> 
        /// <returns></returns> 
        public static DataSet ExecuteDs(String Sqlstr, ref MSSql_Pool mypool)
        {
            SqlConnection conn = mypool.BorrowDBConnection();
            SqlDataAdapter da = new SqlDataAdapter(Sqlstr, conn);
            DataSet ds = new DataSet();
            conn.Open();
            da.Fill(ds);

            mypool.ReturnDBConnection(conn);
            return ds;

        }
        #endregion


    }
}
