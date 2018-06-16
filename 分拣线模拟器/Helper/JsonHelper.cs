using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Reflection;
namespace OMH.WCS.Helper
{
    /// <summary>
    /// 转换Json格式帮助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 序列化成json字符串
        /// </summary>
        /// <param name="obj">object对象</param>
        /// <returns>json字符串</returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 反序列化成Datatable
        /// </summary>
        /// <param name="strJson">json字符串</param>
        /// <returns>Datatable</returns>
        public static DataTable JsonToDataTable(this string strJson)
        {
            #region
            DataTable tb = null;
            //获取数据  
            Regex rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split(',');
                //创建表  
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = "Table";
                    foreach (string str in strRows)
                    {
                        DataColumn dc = new DataColumn();
                        string[] strCell = str.Split(':');
                        dc.DataType = typeof(String);
                        dc.ColumnName = strCell[0].ToString().Replace("\"", "").Trim();
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }
                //增加内容  
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    object strText = strRows[r].Split(':')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("/", "").Replace("\"", "").Trim();
                    if (strText.ToString().Length >= 5)
                    {
                        if (strText.ToString().Substring(0, 5) == "Date(")//判断是否JSON日期格式
                        {
                            strText = "";// CommonHelper.JsonToDateTime(strText.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                    dr[r] = strText;
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }
            return tb;
            #endregion
        }

        /// <summary>
        /// 反序列化成指定列表对象
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="str">json字符串</param>
        /// <returns>列表对象</returns>
        public static List<T> JsonToList<T>(this string str)
        {
            return JsonConvert.DeserializeObject<List<T>>(str);
        }

        /// <summary>
        /// 反序列化成指定对象
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="str">json字符串</param>
        /// <returns>泛型对象</returns>
        public static T JsonToEntity<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        /// <summary>
        /// json格式字符串反序列化成指定对象
        /// </summary>
        /// <typeparam name="TResult">指定对象类型</typeparam>
        /// <param name="strJson">json格式字符串</param>
        /// <returns>指定对象</returns>
        public static TResult FromJson<TResult>(this string strJson)
        {
            //转换json格式时间为标准时间表示法
            //替换Json的Date字符串    
            strJson = Regex.Replace(strJson, @"\\/Date\((\d+)\)\\/", match =>
            {
                var dt = new DateTime(1970, 1, 1);
                dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                dt = dt.ToLocalTime();
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            });

            return JsonConvert.DeserializeObject<TResult>(strJson);
        }

        /// <summary>
        /// 反序列化json字符串为object对象
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns>object</returns>
        public static object FromJson(this string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        /// <summary>
        /// 从指定的Json中取指定Key的值
        /// </summary>
        /// <param name="json">Json串</param>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public static string GetJsonValue(string json, string key)
        {
            try
            {
                ////先在项目中添加System.Web.Extensions引用
                ////using System.Web.Script.Serialization;
                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                //Dictionary<string, object> jsons = (Dictionary<string, object>)serializer.DeserializeObject(json);

                //object value;
                //jsons.TryGetValue(key, out value);

                //if (value != null && value.ToString().Length > 0)
                //    return value.ToString().Trim();
                //else
                    return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 将对象2中非Null的值传给对象1
        /// </summary>
        /// <param name="o1">对象1</param>
        /// <param name="o2">对象2</param>
        /// <param name="pkName">主键</param>
        public static void Copy(object o1, object o2,string pkName)
        {
            Type t = o1.GetType();

            Type t2 = o2.GetType();

            foreach (PropertyInfo pi in t2.GetProperties())
            {
                object value2 = pi.GetValue(o2, null);

                string name = pi.Name;

                if (value2 == null) continue;
                if (name.ToLower().Equals(pkName.ToLower())) continue;

                foreach (PropertyInfo item in t.GetProperties())
                {
                    if (item.Name.Equals(name))
                    {
                        item.SetValue(o1, value2, null);
                    }
                }
            }
        }
    
    }
}
