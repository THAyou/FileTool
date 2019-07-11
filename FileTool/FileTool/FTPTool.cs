using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileTools
{
    /// <summary>
    /// FTP服务简单工具
    /// Tyou 2019-07-11 13:17:00
    /// </summary>
    public class FTPTool
    {
        /// <summary>
        /// 初始化未设置FTP服务器
        /// </summary>
        public FTPTool()
        {
            FTPInfo = new FTPSeviceInfo();
        }

        /// <summary>
        /// 初始化，并且设置FTP服务器IP
        /// </summary>
        /// <param name="FtpID"></param>
        public FTPTool(string FtpID)
        {
            FTPInfo = new FTPSeviceInfo { FTPServiceIP = FtpID };
        }

        /// <summary>
        /// 初始化，并且设置FTP服务器IP和用户名密码
        /// </summary>
        /// <param name="FtpID"></param>
        public FTPTool(string FtpID,string FtpUserName,string FtpPwd)
        {
            FTPInfo = new FTPSeviceInfo { FTPServiceIP = FtpID, FTPUserName = FtpUserName, FTPUserPwd = FtpPwd };
        }

        /// <summary>
        /// FTP登录信息
        /// </summary>
        private FTPSeviceInfo FTPInfo { get; set; }

        /// <summary>
        /// 文件储存目录
        /// </summary>
        public string Path { get; set; } = AppDomain.CurrentDomain.BaseDirectory.ToString();

        /// <summary>
        /// 设置FTP服务器IP地址
        /// </summary>
        public void SetFtpIP(string FtpIP)
        {
            FTPInfo.FTPServiceIP = FtpIP;
        }

        /// <summary>
        /// 设置FTP信息
        /// </summary>
        /// <param name="FtpUserName"></param>
        /// <param name="FtpUserPwd"></param>
        public void SetFtpInfo(string FtpUserName,string FtpUserPwd=null)
        {
            FTPInfo.FTPUserName = FtpUserName;
            FTPInfo.FTPUserPwd = FtpUserPwd;
        }


        /// <summary>
        /// 上传文件至FTP服务器
        /// </summary>
        /// <param name="file">文件对象</param>
        /// <param name="uri">上传地址</param>
        /// <returns></returns>

        public bool UpFile(FileInfo file, string uri)
        {
            string FileUri = string.Empty;
            return FTPHelp.FtpUploadFile(file, FTPInfo.FTPUserName, FTPInfo.FTPUserPwd, uri, out FileUri);
        }

        /// <summary>
        /// 上传文件至FTP服务器
        /// </summary>
        /// <param name="FilePath">文件详细目录</param>
        /// <param name="uri">上传地址</param>
        /// <returns></returns>

        public bool UpFile(string FilePath, string uri)
        {
            string FileUri = string.Empty;
            return FTPHelp.FtpUploadFile(new FileInfo(FilePath), FTPInfo.FTPUserName, FTPInfo.FTPUserPwd, uri, out FileUri);
        }

        /// <summary>
        /// 下载文件至指定路径
        /// </summary>
        /// <param name="DownPath">下载路径</param>
        /// <param name="uri">下载地址</param>
        /// <returns></returns>
        public bool DownloadFile(string DownPath,string uri)
        {
            return FTPHelp.FtpDownload(DownPath, uri, FTPInfo);
        }

        /// <summary>
        /// FTP创建文件夹
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool MakeDir(string uri)
        {
            return FTPHelp.MakeDir(uri, FTPInfo);
        }

        /// <summary>
        /// FTP文件重命名
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="NewFileName"></param>
        /// <returns></returns>
        public bool FileRename(string uri,string NewFileName)
        {
            return FTPHelp.FileRename(uri, FTPInfo.FTPUserName, FTPInfo.FTPUserPwd, NewFileName);
        }

        /// <summary>
        /// 删除FTP上的文件
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public bool FileDelete(string uri)
        {
            return FTPHelp.FileDelete(uri, FTPInfo);
        }
    }
}
