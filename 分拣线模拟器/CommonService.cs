using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMH.WCS.Service
{
    class CommonService
    {
        /// <summary>
        /// 头长度：头:3A、帧长
        /// </summary>
        public const int HEAD_SIZE = 5;
        /// <summary>
        /// 尾长度：校验位、结尾1: 0D、 结尾2:0A

        /// </summary>
        public const int TAIL_SIZE = 3;

        public static byte[] MakePLCMessage(byte[] args)
        {
            int resultLength = args.Length + HEAD_SIZE + TAIL_SIZE;
            byte[] result = new byte[resultLength];
            result[0] = 0x3a;//帧头
            result[1] = (byte)(args.Length +4);//加上校验位

            result[2] = 0x11;
            result[3] = 0x76;
            result[4] = 0x01;


            //result[1] = (byte)(args.Length );//不包括校验位

            for (int i = 0; i < args.Length; i++)
            {
                result[5 + i] = args[i];
            }
            //校检位 帧长度至校验位 不包括帧长度 不包括检验位
            result[resultLength - 3] = CalulateVerificationCode(result, 2, resultLength - 4);
            result[resultLength - 2] = 0x0D;//结尾1
            result[resultLength - 1] = 0x0A;//结尾2

            return result;
        }
        //计算校验位
        public static byte CalulateVerificationCode(byte[] args,int startIndex,int endIndex)
        {
            if(args.Length==0)
            {
                return 0;
            }
            if (startIndex < 0)
                startIndex = 0;
            if (endIndex >= args.Length)
                endIndex = args.Length - 1;
            int temp = 0;
            for(int i=startIndex;i<=endIndex;i++)
            {
                temp += Convert.ToInt16(args[i]);
            }
            return (byte)(temp % 256);
        }

        public static string getRequestTime()
        {
            DateTime now = DateTime.Now;
            DateTime temp = DateTime.Parse("1970-1-01 00:00:00.000");
            TimeSpan ts = now - temp;
            return Convert.ToInt32(ts.TotalSeconds).ToString();
        }
    }
}
