using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace FileTools
{
    /// <summary>
    /// 文件式同步工具
    /// Dgs 2019-07-10 17:01:00
    /// </summary>
    public class FileUpLoadHelpDemo<T>
    {
        public FileUpLoadHelpDemo()
        {
            FTPInfo = new FTPSeviceInfo();
        }

        public FileUpLoadHelpDemo(string FTPServiceIP)
        {
            FTPInfo = new FTPSeviceInfo();
            FTPInfo.FTPServiceIP = FTPServiceIP;
            FTPInfo.FTPUserName = string.Empty;
            FTPInfo.FTPUserPwd = string.Empty;
        }

        public FileUpLoadHelpDemo(string FTPServiceIP, string FTPUserName, string FTPUserPwd)
        {
            FTPInfo = new FTPSeviceInfo();
            FTPInfo.FTPServiceIP = FTPServiceIP;
            FTPInfo.FTPUserName = FTPUserName;
            FTPInfo.FTPUserPwd = FTPUserPwd;
        }

        /// <summary>
        /// FTP登录信息
        /// </summary>
        public FTPSeviceInfo FTPInfo { get; set; }

        /// <summary>
        /// 文件暂时储存目录
        /// </summary>
        public string Path { get; set; } = AppDomain.CurrentDomain.BaseDirectory.ToString() + "UploadFiles";



        /// <summary>
        /// 将数据转换为文件上传至FTP服务器
        /// </summary>
        /// <param name="Info"></param>
        /// <param name="ActionName"></param>
        /// <returns>返回文件所在地址</returns>
        public string UpLoadData(T Info, List<string> ActionName, out string outFileName, string InFileName = null)
        {
            string resultUri = string.Empty;
            //生成文件名
            var FileName = InFileName == null ? DateTime.Now.Ticks.ToString() + ".txt" : InFileName;
            //文件完整目录
            var ParhAndFileName = Path + "\\" + FileName;
            //创建工具实体
            var fileTool = new FileTool(Path);
            //创建工具
            fileTool.CreateFile(Path, FileName);
            //数据序列化
            var DataStr = JsonConvert.SerializeObject(Info);
            //写文件
            var result = fileTool.AppendNewFile(DataStr);
            if (result == "success")
            {
                //生成上传路径
                List<string> UpLoadPaths = ActionName;
                UpLoadPaths.Add(DateTime.Now.ToString("yyyyMMdd"));
                //上传文件
                var IsSuccess = FTPHelp.FtpUploadFile(ParhAndFileName, FTPInfo, UpLoadPaths, out resultUri,true);
                //上传成功，删除临时文件
                if (IsSuccess)
                {
                    fileTool.DeleteFile(ParhAndFileName);
                }
            }
            outFileName = FileName;
            return resultUri;
        }

        /// <summary>
        /// 将数据转换为文件上传至FTP服务器
        /// </summary>
        /// <param name="Info"></param>
        /// <param name="ActionName"></param>
        /// <param name="outFileName"></param>
        /// <returns></returns>
        public string UpLoadData(T Info, string ActionName, out string outFileName, string InFileName = null)
        {
            return this.UpLoadData(Info, new List<string> { ActionName }, out outFileName, InFileName); ;
        }

        /// <summary>
        /// 将数据转换为文件上传至FTP服务器
        /// </summary>
        /// <param name="Info"></param>
        /// <param name="ActionName"></param>
        /// <returns>返回文件所在地址</returns>
        public string UpLoadData(T Info, string ActionName, string InFileName = null)
        {
            string outFileName = string.Empty;
            return this.UpLoadData(Info, new List<string> { ActionName }, out outFileName, InFileName); ;
        }

        /// <summary>
        /// 从FTP下载文件，并转换为实体类
        /// </summary>
        /// <param name="FTPUri"></param>
        /// <returns></returns>
        public T DownloadToData(string FTPUri)
        {
            //生成文件名
            var FileName = DateTime.Now.Ticks.ToString() + ".txt";
            //创建工具实体
            var fileTool = new FileTool(Path);
            //文件完整目录
            var ParhAndFileName = Path + "\\" + FileName;
            //下载失败则返回空
            var IsSuccess = FTPHelp.FtpDownload(FTPUri, ParhAndFileName, FTPInfo,true,false);
            if (IsSuccess)
            {
                //读取文件内容
                var DataStr = fileTool.GetFileContent(Path, FileName);
                if (DataStr != null && DataStr != string.Empty)
                {
                    T result = JsonConvert.DeserializeObject<T>(DataStr);
                    fileTool.DeleteFile(ParhAndFileName);
                    return result;
                }
            }
            return Activator.CreateInstance<T>();
        }
    }


}
