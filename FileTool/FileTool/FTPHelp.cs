using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace FileTools
{
    /// <summary>
    /// 用于访问FTP服务器，下载，上传，创建文件夹，删除文件。支持断点续传，进度条等功能
    /// Tyou 2019-07-11 10:54:00
    /// </summary>
    public class FTPHelp
    {
        #region 文件下载方法组
        /// <summary>
        /// 从FTP服务器下载文件，指定本地路径和本地文件名（支持断点下载）
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        /// <param name="localFileName">保存本地的文件名（包含路径）</param>
        /// <param name="ifCredential">是否启用身份验证（false：表示允许用户匿名下载）</param>
        /// <param name="size">已下载文件流大小</param>
        /// <param name="updateProgress">报告进度的处理(第一个参数：总大小，第二个参数：当前进度)</param>
        /// <returns>是否下载成功</returns>
        public static bool FtpBrokenDownload(string remoteFileName, string localFileName, FTPSeviceInfo Info, bool ifCredential, long size, Action<int, int> updateProgress = null)
        {
            FtpWebRequest reqFTP;
            Stream ftpStream = null;
            FtpWebResponse response = null;
            FileStream outputStream = null;
            try
            {

                outputStream = new FileStream(localFileName, FileMode.Append);
                Uri uri = new Uri(remoteFileName);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.Timeout = 15000;
                reqFTP.UseBinary = true;
                reqFTP.KeepAlive = false;
                reqFTP.ContentOffset = size;
                reqFTP.ReadWriteTimeout = 60000;
                if (ifCredential)//使用用户身份认证
                {
                    reqFTP.Credentials = new NetworkCredential(Info.FTPUserName, Info.FTPUserPwd);
                }
                long totalBytes = GetFileSize(remoteFileName, Info);

                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();

                //更新进度  
                if (updateProgress != null)
                {
                    updateProgress((int)totalBytes, 0);//更新进度条   
                }
                long totalDownloadedByte = 0;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    totalDownloadedByte = readCount + totalDownloadedByte;
                    outputStream.Write(buffer, 0, readCount);
                    //更新进度  
                    if (updateProgress != null)
                    {
                        updateProgress((int)totalBytes, (int)totalDownloadedByte);//更新进度条   
                    }
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
            finally
            {
                if (ftpStream != null)
                {
                    ftpStream.Close();
                }
                if (outputStream != null)
                {
                    outputStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
        }


        /// <summary>
        /// 从FTP服务器下载文件，指定本地路径和本地文件名（支持断点下载）
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        /// <param name="localFileName">保存本地的文件名（包含路径）</param>
        /// <param name="ifCredential">是否启用身份验证（false：表示允许用户匿名下载）</param>
        /// <param name="updateProgress">报告进度的处理(第一个参数：总大小，第二个参数：当前进度)</param>
        /// <returns>是否下载成功</returns>
        public static bool FtpDownload(string remoteFileName, string localFileName, bool ifCredential, FTPSeviceInfo Info, Action<int, int> updateProgress = null)
        {
            FtpWebRequest reqFTP;
            Stream ftpStream = null;
            FtpWebResponse response = null;
            FileStream outputStream = null;
            try
            {
                outputStream = new FileStream(localFileName, FileMode.Create);
                Uri uri = new Uri(remoteFileName);

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.UseBinary = true;
                reqFTP.KeepAlive = false;
                if (ifCredential)//使用用户身份认证
                {
                    reqFTP.Credentials = new NetworkCredential(Info.FTPUserName, Info.FTPUserPwd);
                }

                //获取文件大小
                long totalBytes = GetFileSize(remoteFileName, Info);

                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                response = (FtpWebResponse)reqFTP.GetResponse();
                ftpStream = response.GetResponseStream();

                //更新进度  
                if (updateProgress != null)
                {
                    updateProgress((int)totalBytes, 0);//更新进度条   
                }
                long totalDownloadedByte = 0;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    totalDownloadedByte = readCount + totalDownloadedByte;
                    outputStream.Write(buffer, 0, readCount);
                    //更新进度  
                    if (updateProgress != null)
                    {
                        updateProgress((int)totalBytes, (int)totalDownloadedByte);//更新进度条   
                    }
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
            finally
            {
                if (ftpStream != null)
                {
                    ftpStream.Close();
                }
                if (outputStream != null)
                {
                    outputStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
        }


        /// <summary>
        /// FTP服务器下载文件，指定本地路径和本地文件名
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        /// <param name="localFileName">保存本地的文件名（包含路径）</param>
        /// <param name="Info">服务器信息</param>
        /// <param name="ifCredential">是否启用身份验证（false：表示允许用户匿名下载）</param>
        /// <param name="brokenOpen">是否支持断点下载</param>
        /// <param name="updateProgress">>报告进度的处理(第一个参数：总大小，第二个参数：当前进度)</param>
        /// <returns></returns>
        public static bool FtpDownload(string remoteFileName, string localFileName, FTPSeviceInfo Info, bool ifCredential, bool brokenOpen, Action<int, int> updateProgress = null)
        {
            if (brokenOpen)
            {
                try
                {
                    long size = 0;
                    if (File.Exists(localFileName))
                    {
                        using (FileStream outputStream = new FileStream(localFileName, FileMode.Open))
                        {
                            size = outputStream.Length;
                        }
                    }
                    return FtpBrokenDownload(remoteFileName, localFileName, Info, ifCredential, size, updateProgress);
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                return FtpDownload(remoteFileName, localFileName, ifCredential, Info, updateProgress);
            }
        }

        /// <summary>
        /// 从FTP服务器下载文件，指定本地路径和本地文件名
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        /// <param name="localFileName">保存本地的文件名（包含路径）</param>
        /// <param name="Info">服务器信息</param>
        /// <returns></returns>
        public static bool FtpDownload(string remoteFileName, string localFileName, FTPSeviceInfo Info)
        {
            return FtpDownload(remoteFileName, localFileName, Info, true, false);
        }

        /// <summary>
        /// 从FTP服务器下载文件，指定本地路径和本地文件名
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        /// <param name="localFileName">保存本地的文件名（包含路径）</param>
        /// <param name="FtpUserName">用户名</param>
        /// <param name="FtpUserPwd">密码</param>
        /// <returns></returns>
        public static bool FtpDownload(string remoteFileName, string localFileName, string FtpUserName, string FtpUserPwd)
        {
            FTPSeviceInfo Info = new FTPSeviceInfo { FTPUserName = FtpUserName, FTPUserPwd = FtpUserPwd };
            return FtpDownload(remoteFileName, localFileName, Info);
        }

        /// <summary>
        /// 从FTP服务器下载文件，指定本地路径和本地文件名(不需要验证)
        /// </summary>
        /// <param name="remoteFileName">远程文件名</param>
        /// <param name="localFileName">保存本地的文件名（包含路径）</param>
        /// <returns></returns>
        public static bool FtpDownload(string remoteFileName, string localFileName)
        {
            return FtpDownload(remoteFileName, localFileName, new FTPSeviceInfo(), false, false);
        }
        #endregion

        #region 文件上传方法组
        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="file">文件对象</param>
        /// <param name="Info">登录信息以及服务器IP</param>
        /// <param name="Paths">上传服务器路径</param>
        /// <param name="FileUri">上传成功后，输出文件FTP完整路径</param>
        /// <param name="ifCredential">是否启用身份验证</param>
        /// <param name="updateProgress">进度条方法</param>
        /// <returns></returns>
        public static bool FtpUploadFile(FileInfo file, string FtpUserName, string FtpPwd, string Uri, out string FileUri, bool ifCredential = true, Action<int, int> updateProgress = null)
        {
            FtpWebRequest reqFTP;
            Stream stream = null;
            FtpWebResponse response = null;
            FileStream fs = null;
            try
            {
                if (Uri == null || Uri.Trim().Length == 0)
                {
                    throw new Exception("ftp上传目标服务器地址未设置！");
                }
                FileInfo finfo = file;
                var DateStr = DateTime.Now.ToString("yyyyMMdd");
                var PathStr = string.Empty;
                var uriStr = Uri;
                uriStr += finfo.Name;
                Uri uri = new Uri(uriStr);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.KeepAlive = false;
                reqFTP.UseBinary = true;
                if (ifCredential)
                    reqFTP.Credentials = new NetworkCredential(FtpUserName, FtpPwd);//用户，密码
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;//向服务器发出下载请求命令
                reqFTP.ContentLength = finfo.Length;//为request指定上传文件的大小
                response = reqFTP.GetResponse() as FtpWebResponse;
                reqFTP.ContentLength = finfo.Length;
                int buffLength = 1024;
                byte[] buff = new byte[buffLength];
                int contentLen;
                fs = finfo.OpenRead();
                stream = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                int allbye = (int)finfo.Length;
                //更新进度  
                if (updateProgress != null)
                {
                    updateProgress((int)allbye, 0);//更新进度条   
                }
                int startbye = 0;
                while (contentLen != 0)
                {
                    startbye = contentLen + startbye;
                    stream.Write(buff, 0, contentLen);
                    //更新进度  
                    if (updateProgress != null)
                    {
                        updateProgress((int)allbye, (int)startbye);//更新进度条   
                    }
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                stream.Close();
                fs.Close();
                response.Close();
                FileUri = uriStr;
                return true;

            }
            catch (Exception ex)
            {
                FileUri = null;
                return false;
                throw;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
        }

        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="ServiceIP">文件名以及详细路径</param>
        /// <param name="Info"></param>
        /// <param name="Paths"></param>
        /// <param name="FileUri"></param>
        /// <returns></returns>
        public static bool FtpUploadFile(string ServiceIP, FTPSeviceInfo Info, List<string> Paths, out string FileUri, Action<int, int> updateProgress = null)
        {
            string uri = "ftp://" + Info.FTPServiceIP + "/";
            if (Paths != null && Paths.Count > 1)
            {
                Paths.ForEach(m =>
                {
                    if (m != null && m != string.Empty)
                    {
                        uri += m;
                        uri += "/";
                    }
                });
            }
            return FtpUploadFile(new FileInfo(ServiceIP), Info.FTPUserName, Info.FTPUserPwd, uri, out FileUri, true);
        }

        /// <summary>
        /// 上传文件至FTP服务器(不需要验证)
        /// </summary>
        /// <param name="file">文件对象</param>
        /// <param name="uri">上传地址</param>
        /// <returns></returns>
        public static bool FtpUploadFile(FileInfo file, string uri)
        {
            string FileUri = string.Empty;
            return FtpUploadFile(file, "", "", uri, out FileUri, false);
        }

        /// <summary>
        /// 上传文件至FTP服务器(不需要验证)
        /// </summary>
        /// <param name="localFullPathName">本地文件路径</param>
        /// <param name="uri">上传地址</param>
        /// <returns></returns>
        public static bool FtpUploadFile(string localFullPathName, string uri)
        {
            string FileUri = string.Empty;
            return FtpUploadFile(new FileInfo(localFullPathName), uri);
        }



        #endregion

        #region 获取已经上传的文件大小
        /// <summary>
        /// 获取已上传文件大小
        /// </summary>
        /// <param name="FileName">远程文件路径以及名称</param>
        /// <param name="path">服务器文件路径</param>
        /// <returns></returns>
        public static long GetFileSize(string Uri, FTPSeviceInfo Info)
        {
            long filesize = 0;
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(Uri);
                reqFTP.KeepAlive = false;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(Info.FTPUserName, Info.FTPUserPwd);//用户，密码
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                filesize = response.ContentLength;
                return filesize;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region FTP服务器文件操作(删除,重命名,创建文件夹)

        /// <summary>
        /// 在ftp服务器上创建文件目录
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="FTPUserName">用户名</param>
        /// <param name="FTPUserName">密码</param>
        /// <returns></returns>
        public static bool MakeDir(string uri, string FTPUserName, string FTPUserPwd)
        {
            try
            {
                FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFtp.UseBinary = true;
                // reqFtp.KeepAlive = false;
                reqFtp.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFtp.Credentials = new NetworkCredential(FTPUserName, FTPUserPwd);
                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        ///在ftp服务器上创建文件目录
        /// </summary>
        /// <param name="uri">文件目录</param>
        /// <param name="Info">服务器信息</param>
        /// <returns></returns>
        public static bool MakeDir(string uri, FTPSeviceInfo Info)
        {
            string url = string.Empty;
            var uriStr = "ftp://" + Info.FTPServiceIP + "/";
            uriStr += uri;
            bool b = RemoteFtpDirExists(uriStr, Info);
            if (b)
            {
                return true;
            }
            MakeDir(uriStr, Info.FTPUserName, Info.FTPUserPwd);
            return true;
        }

        /// <summary>
        /// 在ftp服务器上创建文件目录(批量)
        /// </summary>
        /// <param name="Paths">路径集合</param>
        /// <param name="Info">服务器信息</param>
        /// <returns></returns>
        public static bool MakeDir(List<string> Paths, FTPSeviceInfo Info)
        {
            var dirName = string.Empty;
            if (Paths != null && Paths.Count > 1)
            {
                Paths.ForEach(m =>
                {
                    if (m != null && m != string.Empty)
                    {
                        dirName += m;
                        MakeDir(dirName, Info);
                        dirName += "/";
                    }
                });
            }
            else if (Paths.Count == 1)
            {
                MakeDir(Paths[0], Info);
            }
            return true;
        }




        /// <summary>
        /// 判断ftp上的文件目录是否存在
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="FtpUserName"></param>
        /// <param name="FtpUserPwd"></param>
        /// <returns></returns>
        public static bool RemoteFtpDirExists(string uri, string FtpUserName, string FtpUserPwd)
        {
            string url = string.Empty;
            url = uri;
            FtpWebRequest reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            reqFtp.UseBinary = true;
            reqFtp.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);
            reqFtp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            FtpWebResponse resFtp = null;
            try
            {
                resFtp = (FtpWebResponse)reqFtp.GetResponse();
                FtpStatusCode code = resFtp.StatusCode;//OpeningData
                resFtp.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (resFtp != null)
                {
                    resFtp.Close();
                }
                return false;
            }
        }

        /// <summary>
        /// 判断ftp上的文件目录是否存在
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="Info">服务器信息</param>
        /// <returns></returns>
        public static bool RemoteFtpDirExists(string uri, FTPSeviceInfo Info)
        {
            return RemoteFtpDirExists(uri, Info.FTPUserName, Info.FTPUserPwd);
        }


        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="uri">文件Uri</param>
        /// <param name="FTPUserName">服务器登录名</param>
        /// <param name="FTPUserPwd">密码</param>
        /// <param name="newFileName">新文件名</param>
        /// <returns></returns>
        public static bool FileRename(string uri, string FTPUserName, string FTPUserPwd, string newFileName)
        {
            bool success = false;
            FtpWebRequest ftpWebRequest = null;
            FtpWebResponse ftpWebResponse = null;
            Stream ftpResponseStream = null;
            try
            {
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                ftpWebRequest.Credentials = new NetworkCredential(FTPUserName, FTPUserPwd);
                ftpWebRequest.UseBinary = true;
                ftpWebRequest.Method = WebRequestMethods.Ftp.Rename;
                ftpWebRequest.RenameTo = newFileName;
                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                ftpResponseStream = ftpWebResponse.GetResponseStream();
            }
            catch (Exception ex)
            {
                success = false;
                throw;
            }
            finally
            {
                if (ftpResponseStream != null)
                {
                    ftpResponseStream.Close();
                }
                if (ftpWebResponse != null)
                {
                    ftpWebResponse.Close();
                }
            }
            return success;
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="uri">文件Uri</param>
        /// <param name="newFileName">新文件名</param>
        /// <param name="Info">服务器信息</param>
        /// <returns></returns>
        public static bool FileRename(string uri, string newFileName, FTPSeviceInfo Info)
        {
            return FileRename(uri, Info.FTPUserName, Info.FTPUserPwd, newFileName);
        }


        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="Info">FTP服务器信息</param>
        /// <param name="newFileName">新文件名</param>
        /// <param name="paths">路径集合</param>
        /// <returns></returns>
        public static bool FileRename(FTPSeviceInfo Info, string newFileName, List<string> paths)
        {
            string uri = string.Empty;
            var uriStr = "ftp://" + Info.FTPServiceIP + "/";
            if (paths != null && paths.Count > 1)
            {
                paths.ForEach(m =>
                {
                    if (m != null && m != string.Empty)
                    {
                        uri += m;
                        uri += "/";
                    }
                });
            }
            else if (paths.Count == 1)
            {
                uri = paths[0];
            }
            uriStr += uri;
            return FileRename(uriStr, Info.FTPUserName, Info.FTPUserPwd, newFileName);
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="Info">FTP服务器信息</param>
        /// <param name="newFileName">新文件名</param>
        /// <param name="path">文件在服务器上的路径</param>
        /// <returns></returns>
        public static bool FileRename(FTPSeviceInfo Info, string newFileName, string path)
        {
            return FileRename(Info, newFileName, new List<string> { path });
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="Uri">文件Uri</param>
        /// <param name="FTPUserName">服务器登录名</param>
        /// <param name="FTPUserPwd">服务器密码</param>
        /// <returns></returns>
        public static bool FileDelete(string Uri, string FTPUserName, string FTPUserPwd)
        {
            bool success = false;
            FtpWebRequest ftpWebRequest = null;
            FtpWebResponse ftpWebResponse = null;
            Stream ftpResponseStream = null;
            StreamReader streamReader = null;
            try
            {
                ftpWebRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(Uri));
                ftpWebRequest.Credentials = new NetworkCredential(FTPUserName, FTPUserPwd);
                ftpWebRequest.KeepAlive = false;
                ftpWebRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                ftpWebResponse = (FtpWebResponse)ftpWebRequest.GetResponse();
                long size = ftpWebResponse.ContentLength;
                ftpResponseStream = ftpWebResponse.GetResponseStream();
                streamReader = new StreamReader(ftpResponseStream);
                string result = String.Empty;
                result = streamReader.ReadToEnd();
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }
                if (ftpResponseStream != null)
                {
                    ftpResponseStream.Close();
                }
                if (ftpWebResponse != null)
                {
                    ftpWebResponse.Close();
                }
            }
            return success;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="Uri">文件Uri</param>
        /// <param name="Info">服务器信息</param>
        /// <returns></returns>
        public static bool FileDelete(string Uri, FTPSeviceInfo Info)
        {
            return FileDelete(Uri, Info.FTPUserName, Info.FTPUserPwd);
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="Info"></param>
        /// <param name="paths"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static bool FileDelete(FTPSeviceInfo Info, List<string> paths, string FileName)
        {
            string uri = string.Empty;
            var uriStr = "ftp://" + Info.FTPServiceIP + "/";
            if (paths != null && paths.Count > 1)
            {
                paths.ForEach(m =>
                {
                    if (m != null && m != string.Empty)
                    {
                        uri += m;
                        uri += "/";
                    }
                });
            }
            else if (paths.Count == 1)
            {
                uri = paths[0];
            }
            uriStr += uri;
            uriStr += FileName;
            return FileDelete(uriStr, Info.FTPUserName, Info.FTPUserPwd);
        }
        #endregion
    }

}
