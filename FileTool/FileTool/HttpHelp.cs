using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BT.Preservation.Bll.HTTPHelp
{
    /// <summary>
    /// Http请求帮助类
    /// </summary>
    public class HttpHelp
    {
        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="Jsondata">参数对象</param>
        /// <returns></returns>
        public static string SendPostJsonData(string url, object Jsondata)
        {
            return SendHttp(url, JsonConvert.SerializeObject(Jsondata), HttpMethod.Post);
        }
        /// <summary>
        /// 发送Post请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <param name="data">请求参数</param>
        /// <returns></returns>
        public static string SendPostData(string url,string data)
        {
            return SendHttp(url, data, HttpMethod.Post);
        }

        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <param name="url">请求url</param>
        /// <returns></returns>
        public static string SendGetData(string url)
        {
            return SendHttp(url, "", HttpMethod.Get);
        }
        /// <summary>
        /// 发送Http请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SendHttp(string url, string data, HttpMethod method)
        {
            HttpWebRequest request = null;
            request = WebRequest.Create(url) as HttpWebRequest;
            switch (method)
            {
                case HttpMethod.Get:
                    request.Method = "GET";
                    break;
                case HttpMethod.Post:
                    {
                        request.Method = "POST";
                        byte[] bdata = Encoding.UTF8.GetBytes(data);
                        request.ContentType = "application/json;charset=UTF-8";
                        request.ContentLength = bdata.Length;
                        Stream streamOut = request.GetRequestStream();
                        streamOut.Write(bdata, 0, bdata.Length);
                        streamOut.Close();
                    }
                    break;
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamIn = response.GetResponseStream();

            StreamReader reader = new StreamReader(streamIn);
            string result = reader.ReadToEnd();
            reader.Close();
            streamIn.Close();
            response.Close();

            return result;
        }

        /// <summary>
        /// 发送http post请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="parameters">查询参数集合</param>
        /// <returns></returns>
        public string CreatePostHttpResponse(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;//创建请求对象
            request.Method = "POST";//请求方式
            request.ContentType = "application/x-www-form-urlencoded";//链接类型
                                                                      //构造查询字符串
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                bool first = true;
                foreach (string key in parameters.Keys)
                {

                    if (!first)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        first = false;
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                //写入请求流
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream streamIn = response.GetResponseStream();

            StreamReader reader = new StreamReader(streamIn);
            string result = reader.ReadToEnd();
            reader.Close();
            streamIn.Close();
            response.Close();
            return result;
        }

        /// <summary>
        /// Http下载
        /// </summary>
        /// <param name="HttpFileUrl"></param>
        /// <param name="FSavePath"></param>
        /// <returns></returns>
        private static string HttpDownLoadFile(string HttpFileUrl, string FSavePath,string SaveFileName)
        {
            HttpFileUrl = HttpFileUrl.Replace("\\", "/");
            var request = WebRequest.Create(HttpFileUrl);
            request.Method = WebRequestMethods.Http.Get;
            request.ContentType = "application/octet-stream";
            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            var directPath = FSavePath;
            if (!Directory.Exists(directPath))
            {
                Directory.CreateDirectory(directPath);
            }

            string filePath = directPath + SaveFileName;
            var fileStream = new FileStream(filePath, FileMode.CreateNew);

            var bytes = new byte[2048];
            var count = stream.Read(bytes, 0, bytes.Length);
            while (count > 0)
            {
                fileStream.Write(bytes, 0, count);
                count = stream.Read(bytes, 0, bytes.Length);
            }
            stream.Close();
            fileStream.Close();

            return filePath;
        }
    }

    /// <summary>
    /// 请求类型
    /// </summary>
    public enum HttpMethod
    {
        Get=1,
        Post=2
    }
}
