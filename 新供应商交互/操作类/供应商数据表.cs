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

namespace 新供应商交互.操作类
{
    public class 供应商数据表
    {
        public static JsonData 数据表(JsonData jdata)
        {
            try
            {
                #region 参数
                if (jdata.Keys.Contains("params"))
                {
                    JsonData json参数 = jdata["params"];
                    string 登录名 = "";
                    if (json参数.Keys.Contains("logname"))
                    {
                        登录名 = json参数["logname"].ToString();
                    }
                    string offset = "";
                    if (json参数.Keys.Contains("offset"))
                    {
                        offset = json参数["offset"].ToString();
                    }
                    string rows = "";
                    if (json参数.Keys.Contains("rows"))
                    {
                        rows = json参数["rows"].ToString();
                    }
                }
                #endregion

                // string scmd1 = "SELECT * FROM notbj  where 1 " + s_search + "   LIMIT " + i_num.ToString() + "," + numPerPage + "";

                JsonData jd = new JsonData();
                jd["status"] = "0";
                jd["message"] = "ok";
                return jd;
            }
            catch (Exception ex)
            {
                JsonData jd = new JsonData();
                jd["status"] = "300";
                jd["message"] = ex.Message;
                return jd;
            }
        }



    }
}
