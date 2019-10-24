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
using http请求服务.dal;
using 新供应商交互.op;
using 新供应商交互.操作类;
namespace 新供应商交互
{
    class Program
    {
        public static object l_ob_log = new object();
        public static MySql_Pool myPool;
        public static MSSql_Pool msPool;
        public static MySql_Pool myPool_supplier;
        public static MySql_Pool myPool_userdata;
        static void Main(string[] args)
        {
            if (CheckExist())
            {
                return;
            }
            #region 初始化参数
            msPool = new MSSql_Pool(@"server=192.168.2.254;uid=sa;pwd=win654321;database=tousu;Max Pool Size=1000;Min Pool Size=0;Pooling=TRUE");
            string s_81 = @"Server=192.168.2.51;Database=gross_profit;Uid=root;Pwd=jxjjwin2019;pooling=true;Min Pool Size=0;Max Pool Size=100;";
            myPool = new MySql_Pool(s_81);
            myPool_supplier = new MySql_Pool(@"Server=192.168.2.51;Database=supplier;Uid=root;Pwd=jxjjwin2019;pooling=true;Min Pool Size=0;Max Pool Size=100;");
            myPool_userdata = new MySql_Pool(@"Server=192.168.2.51;Database=userdata;Uid=root;Pwd=jxjjwin2019;pooling=true;Min Pool Size=0;Max Pool Size=100;");
            #endregion

            #region
            HttpListener listerner = new HttpListener();
            try
            {
                listerner.AuthenticationSchemes = AuthenticationSchemes.Anonymous;//指定身份验证 Anonymous匿名访问
                listerner.Prefixes.Add("http://192.168.2.159:8090/Service/");
                listerner.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务启动失败..." + ex.Message);
                Console.ReadLine();
            }
            Console.WriteLine("服务器启动成功.......");

            //线程池
            int minThreadNum;
            int portThreadNum;
            int maxThreadNum;

            ThreadPool.GetMaxThreads(out maxThreadNum, out portThreadNum);
            ThreadPool.GetMinThreads(out minThreadNum, out portThreadNum);
            Console.WriteLine("最大线程数：{0}", maxThreadNum);
            Console.WriteLine("最小空闲线程数：{0}", minThreadNum);
            Console.WriteLine("\n\n等待客户连接中。。。。");
            while (true)
            {
                //等待请求连接
                //没有请求则GetContext处于阻塞状态
                try
                {
                    HttpListenerContext ctx = listerner.GetContext();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc), ctx);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    my_log(ex.Message + ex.StackTrace);
                    listerner.Close();
                    listerner.Start();
                }

            }
            #endregion
            //Console.ReadLine();
        }

        static void TaskProc(object o)
        {
            try
            {
                HttpListenerContext ctx = (HttpListenerContext)o;
                try
                {
                    ctx.Response.StatusCode = 200;//设置返回给客服端http状态代码
                    ctx.Response.ContentType = "text/plain";

                    Stream stream = ctx.Request.InputStream;
                    System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
                    String body = reader.ReadToEnd();
                    body = HttpUtility.UrlDecode(body);
                    string 输出 = 公共操作.UnicodeToGB(body);
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "收到json数据:" + 输出);

                    JsonData jo = JsonMapper.ToObject(body);
                    if (jo.Keys.Contains("action"))
                    {
                        string action = jo["action"].ToString();
                        action = 公共操作.UnicodeToGB(action);
                        JsonData jd = null;
                        switch (action)
                        {
                            case "mao_ts":
                                jd = 供应商数据表.数据表(jo);
                                break;
                        }
                        if (jd == null)
                        {
                            jd = new JsonData();
                            jd["status"] = "300";
                            jd["message"] = "没有对应的action";
                        }
                        #region
                        string jsonData = JsonMapper.ToJson(jd);
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonData);
                        ctx.Response.ContentLength64 = buffer.Length;
                        System.IO.Stream output = ctx.Response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        //关闭输出流，释放相应资源
                        output.Close();
                        ctx.Response.Close();
                        #endregion
                    }
                    else
                    {
                        #region 错误
                        JsonData jd = new JsonData();
                        jd["status"] = "300";
                        jd["message"] = "缺少必要的参数";
                        string jsonData = JsonMapper.ToJson(jd);
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "返回数据:" + jsonData);
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonData);
                        ctx.Response.ContentLength64 = buffer.Length;
                        System.IO.Stream output = ctx.Response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        //关闭输出流，释放相应资源
                        output.Close();
                        ctx.Response.Close();
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    #region
                    JsonData jd = new JsonData();
                    jd["status"] = "300";
                    jd["message"] = "缺少必要的参数";
                    string jsonData = JsonMapper.ToJson(jd);
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonData);
                    ctx.Response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = ctx.Response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    //关闭输出流，释放相应资源
                    output.Close();
                    ctx.Response.Close();
                    // qq群通知(ex.Message);
                    Console.WriteLine(ex.Message);
                    my_log(ex.Message + ex.StackTrace);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                my_log(ex.Message + ex.StackTrace);
            }
            

        }


        #region
        public static string UnicodeToString(string unicode)
        {
            string resultStr = "";
            string[] strList = unicode.Split('u');
            for (int i = 1; i < strList.Length; i++)
            {
                resultStr += (char)int.Parse(strList[i], System.Globalization.NumberStyles.HexNumber);
            }
            return resultStr;
        }

        static bool CheckExist()
        {
            Process[] app = Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName);

            if (app.Length > 1)
            {
                Console.WriteLine("程序已启动,请不要重复运行!");
                //PostMessage();
                Console.ReadLine();
                return true;
            }
            else
            {
                Console.WriteLine("程序准备开始..");
                return false;
            }
        }

        public static void qq发送(string message, string name, string type)
        {
            string sql = "insert qq_messsage(type,message,name)values('" + type + "','" + message + "','" + name + "') ";
            MysqlHelper.ExecuteNonQuery(sql, ref Program.myPool);
        }

        public static void qq群通知(string message)
        {
            if (message.Length > 200)
            {
                message = message.Substring(0, 199);
            }
            Program.qq发送(message, "财务技术信息分享群", "0");
        }

        public static void my_log(string content)
        {
            try
            {
                lock (l_ob_log)
                {
                    string sgenPath = System.IO.Directory.GetCurrentDirectory();

                    string savepath = sgenPath + @"\log\";
                    if (!Directory.Exists(savepath))
                    {
                        Directory.CreateDirectory(savepath);
                    }
                    string saveFilename = DateTime.Now.ToString("yyyy-MM-dd");

                    using (System.IO.StreamWriter hfile = new System.IO.StreamWriter(savepath + saveFilename + ".txt", true, Encoding.UTF8))
                    {
                        hfile.WriteLine("----" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ----");
                        hfile.WriteLine(content);
                        hfile.WriteLine("--------");
                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "|" + ex.StackTrace);
            }

        }
        #endregion

    }
}
