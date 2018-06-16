using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace OMH.WCS.Helper
{
    /// <summary>
    /// 百世回调接口2个
    /// </summary>
    [ServiceContract]
    public interface IServiceBaiShi
    {
        /// <summary>
        /// get方式 获取流水线分拣模式获取接口
        /// </summary>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "GET",UriTemplate = "/pipeline/v2/sort_mode?pipeline={pipeline}", ResponseFormat = WebMessageFormat.Json)]
        string sort_mode(int pipeline);

        /// <summary>
        /// post方式 补码结果推送接口
        /// </summary>
        /// <param name="complement_info"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, 
            UriTemplate = "/task/v2/complement_info", 
            RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Bare)]
        string complement_info(complement_info complement_info);


        /// <summary>
        /// post方式 分拣信息请求接口
        /// </summary>
        /// <param name="sorting_info"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/wcs/v2/sorting_info",
            RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Bare)]
        string sorting_info(sorting_info sorting_info);


        /// <summary>
        /// post方式 分拣结果推送接口
        /// </summary>
        /// <param name="sorting_result"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/wcs/v2/sorting_result",
            RequestFormat = WebMessageFormat.Json, Method = "POST", BodyStyle = WebMessageBodyStyle.Bare)]
        string sorting_result(sorting_result sorting_result);

        
    }

    /// <summary>
    /// 补码结果推送接口
    /// </summary>
    [DataContract]
    public class complement_info
    {
        String _sortingId = "";
        String _trayCode = "";
        String _billCode = "";
        String _pipeline = "";
        List<String> _sortPortCode = new List<String>();
        String _sortSource = "";
        String _sortCode = "";

        /// <summary>
        /// 一次分拣操作唯一 ID
        /// </summary>
        [DataMember]
        public String sortingId
        {
            get { return _sortingId; }
            set { _sortingId = value; }
        }

        /// <summary>
        /// 小车编码
        /// </summary>
        [DataMember]
        public String trayCode
        {
            get { return _trayCode; }
            set { _trayCode = value; }
        }

        /// <summary>
        /// 运单单号
        /// </summary>
        [DataMember]
        public String billCode
        {
            get { return _billCode; }
            set { _billCode = value; }
        }

        /// <summary>
        /// 流水线号
        /// </summary>
        [DataMember]
        public String pipeline
        {
            get { return _pipeline; }
            set { _pipeline = value; }
        }

        /// <summary>
        /// 若有分拣口信息，则返回分拣口，可能有多个
        /// </summary>
        [DataMember]
        public List<String> sortPortCode
        {
            get { return _sortPortCode; }
            set { _sortPortCode = value; }
        }

        /// <summary>
        /// 如人工补码
        /// </summary>
        [DataMember]
        public String sortSource
        {
            get { return _sortSource; }
            set { _sortSource = value; }
        }

        /// <summary>
        /// 分拣编码
        /// </summary>
        [DataMember]
        public String sortCode
        {
            get { return _sortCode; }
            set { _sortCode = value; }
        }
    }


    /// <summary>
    /// 分拣信息请求接口
    /// </summary>
    [DataContract]
    public class sorting_info
    {
        String _sortingId = "";
        String _trayCode = "";
        String _requestTime = "";
        String _pipeline = "";

        String _sortMode = "";

        /// <summary>
        /// 一次分拣操作唯一 ID
        /// </summary>
        [DataMember]
        public String sortingId
        {
            get { return _sortingId; }
            set { _sortingId = value; }
        }

        /// <summary>
        /// 小车编码
        /// </summary>
        [DataMember]
        public String trayCode
        {
            get { return _trayCode; }
            set { _trayCode = value; }
        }

        String _trayStatus = "";
        /// <summary>
        /// 小车状态
        /// </summary>
        [DataMember]
        public String trayStatus
        {
            get { return _trayStatus; }
            set { _trayStatus = value; }
        }

        List<String> _billCodes = new List<String>();
        /// <summary>
        /// 当能够识别单号则为单号，否则为字符串 NOREAD
        /// </summary>
        [DataMember]
        public List<String> billCodes
        {
            get { return _billCodes; }
            set { _billCodes = value; }
        }

        /// <summary>
        /// 流水线号
        /// </summary>
        [DataMember]
        public String pipeline
        {
            get { return _pipeline; }
            set { _pipeline = value; }
        }

        String _turnNumber = "";
        /// <summary>
        /// 货物已转圈数
        /// </summary>
        [DataMember]
        public String turnNumber
        {
            get { return _turnNumber; }
            set { _turnNumber = value; }
        }

        /// <summary>
        /// 拍照扫码时间
        /// </summary>
        [DataMember]
        public String requestTime
        {
            get { return _requestTime; }
            set { _requestTime = value; }
        }

        /// <summary>
        /// 分拣模式
        /// </summary>
        [DataMember]
        public String sortMode
        {
            get { return _sortMode; }
            set { _sortMode = value; }
        }
    }

    /// <summary>
    /// 补码结果推送接口
    /// </summary>
    [DataContract]
    public class sorting_result
    {
        String _sortingId = "";
        String _trayCode = "";
        String _billCode = "";
        String _pipeline = "";
        String _sortPortCode = "";
        String _sortSource = "";
        String _sortCode = "";

        /// <summary>
        /// 一次分拣操作唯一 ID
        /// </summary>
        [DataMember]
        public String sortingId
        {
            get { return _sortingId; }
            set { _sortingId = value; }
        }

        /// <summary>
        /// 小车编码
        /// </summary>
        [DataMember]
        public String trayCode
        {
            get { return _trayCode; }
            set { _trayCode = value; }
        }

        /// <summary>
        /// 运单单号
        /// </summary>
        [DataMember]
        public String billCode
        {
            get { return _billCode; }
            set { _billCode = value; }
        }

        /// <summary>
        /// 流水线号
        /// </summary>
        [DataMember]
        public String pipeline
        {
            get { return _pipeline; }
            set { _pipeline = value; }
        }

        String _turnNumber = "";
        /// <summary>
        /// 货物已转圈数
        /// </summary>
        [DataMember]
        public String turnNumber
        {
            get { return _turnNumber; }
            set { _turnNumber = value; }
        }

        String _sortTime = "";
        /// <summary>
        /// 拍照扫码时间
        /// </summary>
        [DataMember]
        public String sortTime
        {
            get { return _sortTime; }
            set { _sortTime = value; }
        }



        /// <summary>
        /// 若有分拣口信息，则返回分拣口，可能有多个
        /// </summary>
        [DataMember]
        public String sortPortCode
        {
            get { return _sortPortCode; }
            set { _sortPortCode = value; }
        }

        /// <summary>
        /// 如人工补码
        /// </summary>
        [DataMember]
        public String sortSource
        {
            get { return _sortSource; }
            set { _sortSource = value; }
        }

        /// <summary>
        /// 分拣编码
        /// </summary>
        [DataMember]
        public String sortCode
        {
            get { return _sortCode; }
            set { _sortCode = value; }
        }

        String _sortMode = "";
        /// <summary>
        /// 分拣模式
        /// </summary>
        [DataMember]
        public String sortMode
        {
            get { return _sortMode; }
            set { _sortMode = value; }
        }
    }
}
