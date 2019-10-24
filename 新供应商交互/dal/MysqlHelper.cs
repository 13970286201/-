using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
namespace http请求服务.dal
{
    class MysqlHelper
    {
        public static string error_sql = "";
        public MysqlHelper() { }


        #region 创建command
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandTimeout = 3600;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType; 
            if (cmdParms != null)
            {
                foreach (MySqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                    (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
        #endregion


        #region ExecuteNonQuery

        //执行SQL语句，返回影响的记录数 
        /// <summary> 
        /// 执行SQL语句，返回影响的记录数 
        /// </summary> 
        /// <param name="SQLString">SQL语句</param> 
        /// <returns>影响的记录数</returns> 
        public static int ExecuteNonQuery(string SQLString, ref MySql_Pool mypool)
        {

            MySqlConnection connection = mypool.BorrowDBConnection();
            using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
            {
                try
                {
                    cmd.CommandTimeout = 7600;
                    int rows = cmd.ExecuteNonQuery();
                    mypool.ReturnDBConnection(connection);
                    return rows;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    mypool.ReturnDBConnection(connection);
                    throw e;
                }
            }

        }

        //public static void ExecuteNonQuery(ref  List<mysqlstrut> list, ref MySql_Pool mypool)
        //{

        //    MySqlConnection connection = mypool.BorrowDBConnection();
        //    try
        //    {
        //        foreach (mysqlstrut msql in list)
        //        {
        //            error_sql = msql.sql;
        //            using (MySqlCommand cmd = new MySqlCommand(msql.sql, connection))
        //            {
        //                try
        //                {
        //                    cmd.CommandTimeout = 3600;
        //                    cmd.ExecuteNonQuery();
        //                }
        //                catch
        //                {
        //                    MySqlCommand cmd1 = new MySqlCommand(msql.sql1, connection);
        //                    cmd1.CommandTimeout = 3600;
        //                    cmd1.ExecuteNonQuery();
        //                    Program.my_log(msql.sql);
        //                }


        //                //return rows;


        //            }
        //        }
        //        mypool.ReturnDBConnection(connection);
        //    }
        //    catch (MySql.Data.MySqlClient.MySqlException e)
        //    {
        //        Program.my_log(error_sql);
        //        mypool.ReturnDBConnection(connection);
        //        throw e;
        //    }


        //}

        public static int ExecuteNonQuery(string SQLString, ref MySql_Pool mypool, params MySqlParameter[] cmdParms)
        {
            MySqlConnection connection = mypool.BorrowDBConnection();
            using (MySqlCommand cmd = new MySqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    int rows = cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                    mypool.ReturnDBConnection(connection);
                    return rows;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    mypool.ReturnDBConnection(connection);
                    throw e;
                }
            }

        }

        #endregion


        #region ExecuteScalar
        /// <summary> 
        /// 执行一条计算查询结果语句，返回查询结果（object）。 
        /// </summary> 
        /// <param name="SQLString">计算查询结果语句</param> 
        /// <returns>查询结果（object）</returns> 
        public static object ExecuteScalar(string SQLString, ref MySql_Pool mypool)
        {
            MySqlConnection connection = mypool.BorrowDBConnection();
            using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
            {
                try
                {
                    cmd.CommandTimeout = 3600;


                    object obj = cmd.ExecuteScalar();
                    mypool.ReturnDBConnection(connection);
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    mypool.ReturnDBConnection(connection);
                    throw e;
                }
            }

        }
        /// <summary> 
        /// 执行一条计算查询结果语句，返回查询结果（object）。 
        /// </summary> 
        /// <param name="SQLString">计算查询结果语句</param> 
        /// <returns>查询结果（object）</returns> 
        public static object ExecuteScalar(string SQLString, ref MySql_Pool mypool, params MySqlParameter[] cmdParms)
        {
            MySqlConnection connection = mypool.BorrowDBConnection();
            using (MySqlCommand cmd = new MySqlCommand())
            {
                try
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    object obj = cmd.ExecuteScalar();
                    mypool.ReturnDBConnection(connection);
                    cmd.Parameters.Clear();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    mypool.ReturnDBConnection(connection);
                    throw e;
                }
            }

        }
        #endregion



        #region ExecuteDataTable
        public static DataTable ExecuteDataTable(string SQLString, ref MySql_Pool mypool)
        {
            MySqlConnection connection = mypool.BorrowDBConnection();
            DataSet ds = new DataSet();
            try
            {

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = SQLString;
                cmd.CommandType = CommandType.Text;//cmdTyp
                cmd.CommandTimeout = 3600;
                MySqlDataAdapter command = new MySqlDataAdapter(cmd);

                //MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);

                command.Fill(ds, "ds");
                mypool.ReturnDBConnection(connection);
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                mypool.ReturnDBConnection(connection);
                throw new Exception(ex.Message);
            }
            return ds.Tables[0];
        }

        /// <summary> 
        /// 执行查询语句，返回DataSet 
        /// </summary> 
        /// <param name="SQLString">查询语句</param> 
        /// <returns>DataTable</returns> 
        public static DataTable ExecuteDataTable(string SQLString, ref MySql_Pool mypool, params MySqlParameter[] cmdParms)
        {
            MySqlConnection connection = mypool.BorrowDBConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandTimeout = 3600;
            PrepareCommand(cmd, connection, null, SQLString, cmdParms);
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                try
                {
                    da.Fill(ds, "ds");
                    cmd.Parameters.Clear();
                    mypool.ReturnDBConnection(connection);
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    mypool.ReturnDBConnection(connection);
                    throw new Exception(ex.Message);
                }
                return ds.Tables[0];
            }

        }



        #endregion

        #region 事务
        public static void Transaction(List<string> list,ref MySql_Pool mypool)
        {
            MySqlConnection connection = mypool.BorrowDBConnection();
            MySqlTransaction myTrans = connection.BeginTransaction();
            try
            {
               
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.Transaction = myTrans;

                foreach (string SQLString in list)
                {
                    cmd.CommandText = SQLString;
                    int rows = cmd.ExecuteNonQuery();

                }
               // cmd.ExecuteNonQuery();
                myTrans.Commit();
            }
            catch (Exception e)
            {
                myTrans.Rollback();//遇到错误，回滚
                mypool.ReturnDBConnection(connection);
                throw e;
            }

          
               
                


        }

        public static bool ExecuteNoQueryTran(List<String> SQLStringList, ref MySql_Pool mypool)
        {

            MySqlConnection conn = mypool.BorrowDBConnection();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
            try
            {
                for (int n = 0; n < SQLStringList.Count; n++)
                {
                    string strsql = SQLStringList[n];
                    if (strsql.Trim().Length > 1)
                    {
                        cmd.CommandText = strsql;
                        PrepareCommand(cmd, conn, tx, strsql, null);
                        cmd.ExecuteNonQuery();
                    }
                }
                cmd.ExecuteNonQuery();
                tx.Commit();
                return true;
            }
            catch
            {
                tx.Rollback();
                mypool.ReturnDBConnection(conn);
                return false;
            }
           
        }
        #endregion


    }
}
