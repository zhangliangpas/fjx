using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using log4net;

namespace GMS.Common.Log
{
    /// <summary>
    /// Log4Net日志 工厂
    /// 版本：2.0
    /// </summary>
    public class LogFactory
    {
        static LogFactory()
        {
            //取代之前的HttpContext获取路径方式，兼容WCF等其他应用 
            var configFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "/XmlConfig/log4net.config");
            //FileInfo configFile = new FileInfo(HttpContext.Current.Server.MapPath("/XmlConfig/log4net.config"));

            log4net.Config.XmlConfigurator.Configure(configFile);
        }

        public static LogHelper GetLogger(Type type)
        {
            return new LogHelper(LogManager.GetLogger(type));
        }

        public static LogHelper GetLogger(string str)
        {
            return new LogHelper(LogManager.GetLogger(str));
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="caption">标题</param>
        /// <param name="msg">内容</param>
        public static void GetLogger(string caption, string msg)
        {
            LogHelper log = LogFactory.GetLogger(caption);
            log.Error(msg);

            StackTrace st = new StackTrace(true);
            string methodName = st.GetFrame(1).GetMethod().Name.ToString();
            log.Error(methodName + "->" + msg);
        }

        public static void GetLogger(Type type, string msg)
        {
            LogHelper log = LogFactory.GetLogger(type);
            StackTrace st = new StackTrace(true);

            string methodName = st.GetFrame(1).GetMethod().Name.ToString();
            log.Error(methodName + "->" + msg);
        }
    }
}
