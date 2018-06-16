using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace OMH.WCS.Helper
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ServiceBaiShi : IServiceBaiShi
    {
        public string sort_mode(int pipeline)
        {
            //编写业务逻辑
            return string.Format("get 方法传入参数: {0}", pipeline);
        }

        public string complement_info(complement_info complement_info)
        {
            //编写业务逻辑
            return complement_info.ToJson();
        }

        public string sorting_info(sorting_info sorting_info)
        {
            //编写业务逻辑
            //return sorting_info.ToJson();

            //"{ \"status\":\"1\",\"errorCode\":[],\"errorInfo\":[],\"attachInfo\":\"成功请求分拣结果！\",\"result\":{\"sortingId\":\"SdsSim-20160111153404317\",\"trayCode\":1,\"billCode\":\"71130220364148\",\"pipeline\":\"200000-001\",\"sortPortCode\":[\"200000-001093\"],\"sortSource\":\"暴力分拣\",\"sortCode\":\"A05\"},\"listResult\":null}";

            string apiResult, chute;
            chute = sorting_info.billCodes[0].Substring(sorting_info.billCodes[0].ToString().Length - 1, 1);
            int temp = 0;
            if (int.TryParse(chute, out temp))
            {
                chute = (temp + 1).ToString("000");
            }
            else
            {
                chute = "11";
            }

             apiResult = "{ \"status\":\"1\",\"errorCode\":[],\"errorInfo\":[],\"attachInfo\":\"成功请求分拣结果！\",\"result\":{\"sortingId\":\"SdsSim-20160111153404317\",\"trayCode\":1,\"billCode\":\"71130220364148\",\"pipeline\":\"200000-001\",\"sortPortCode\":[\"200000-001" + chute + "\"],\"sortSource\":\"暴力分拣\",\"sortCode\":\"A05\"},\"listResult\":null}";
            return apiResult;
        }

        public string sorting_result(sorting_result sorting_result)
        {

            string apiResult = "{ \"status\":1,\"errorCode\":[],\"errorInfo\":[],\"attachInfo\":\"成功创建分拣结果\",\"result\":null,\"listResult\":null}";
            return apiResult;
        }

        

        public static bool ServiceStart()
        {
            try
            {
                ServiceHost serviceHost = new ServiceHost(typeof(ServiceBaiShi));
                if (serviceHost.State != CommunicationState.Opened)
                {
                    serviceHost.Open();
                    return true;
                }
            }
            catch
            {
                throw new Exception();
                return false;
            }
            return false;
        }

    }
}
