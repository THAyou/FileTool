using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileTools
{
    /// <summary>
    /// FileTool 文件工具
    /// 作者:Tyou 2019-03-19
    /// 版本:v1.0
    /// 用于快捷对文件进行[读取,复制,剪切，删除,修改，重命名]操作
    /// </summary>
    public class FileTool
    {
        private DirectoryInfo root = null;

        private string _path = Directory.GetCurrentDirectory();

        private List<FileInfo> files =  null;

        private List<DirectoryInfo> Directorys = null;

        private FileStream FileStream = null;

        private string _FileName = null;

        /// <summary>
        /// 该目录下读取到文件列表
        /// </summary>
        public List<FileDetails> FileDetailsList = new List<FileDetails>();
        
        /// <summary>
        /// 目录
        /// </summary>
        public FileTool()
        {
            LoadDirectoryInfo();
        }

        /// <summary>
        /// 读取该目录
        /// </summary>
        /// <param name="path">目录</param>
        public FileTool(string path)
        {
            _path = path;
            LoadDirectoryInfo();
        }

        /// <summary>
        /// 读取该目录下的文件
        /// </summary>
        /// <param name="path">目录</param>
        /// <param name="FileName">文件名</param>
        public FileTool(string path,string FileName)
        {
            _path = path;
            _FileName = FileName;
            LoadDirectoryInfo();
        }

        private void LoadDirectoryInfo()
        {
            root = new DirectoryInfo(_path);
            files = root.GetFiles().ToList();
            Directorys = root.GetDirectories().ToList();
            foreach (var d in Directorys)
            {
                FileDetails fileDetails = new FileDetails();
                fileDetails.FileType = FileType.Folder;
                fileDetails.FileName = d.Name;
                fileDetails.FileSize = 0;
                FileDetailsList.Add(fileDetails);
            }
            foreach (var F in files)
            {
                FileDetails fileDetails = new FileDetails();
                fileDetails.FileType = FileType.File;
                fileDetails.FileName = F.Name;
                fileDetails.FileSize = System.Math.Ceiling(F.Length / 1024.0);
                FileDetailsList.Add(fileDetails);
            }
        }

        private void Write(string value, FileMode mode)
        {
            byte[] myByte = System.Text.Encoding.UTF8.GetBytes(value); 
            FileStream = new FileStream(_path + "/" + _FileName, mode, FileAccess.Write);
            FileStream.Write(myByte, 0, myByte.Length);
            FileStream.Flush();
            FileStream.Close();
        }

        private string Read()
        {
            FileStream = new FileStream(_path + "/" + _FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            int fsLen = (int)FileStream.Length;
            byte[] Byte = new byte[fsLen];
            int r = FileStream.Read(Byte, 0, Byte.Length);
            FileStream.Flush();
            FileStream.Close();
            return Encoding.UTF8.GetString(Byte);
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="PathOrFileName"></param>
        /// <param name="IsPathFileName"></param>
        public string ReadFile(string PathOrFileName, bool IsPathFileName = false)
        {
            try
            {
                if (IsPathFileName)
                {
                    _FileName = new FileInfo(PathOrFileName).Name;
                    FileStream = new FileStream(PathOrFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                }
                else
                {
                    _FileName = PathOrFileName;
                    FileStream = new FileStream(_path + "/" + _FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                }
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        /// <summary>
        /// 获取此文件目录下的所有文件信息
        /// </summary>
        /// <param name="path">路径,可无</param>
        /// <returns></returns>
        public List<FileDetails> GetFileeach(string path=null)
        {
            if (path != null && path != "")
            {
                _path = path;
                FileDetailsList = new List<FileDetails>();
                LoadDirectoryInfo();
            }
            return FileDetailsList;
        }

        /// <summary>
        /// 获取此目录下或者完整目录下的文件流
        /// </summary>
        /// <param name="PathOrFileName">完整路径或者文件名</param>
        /// <param name="IsPathFileName">是否是完整路径(默认为否)</param>
        /// <returns></returns>
        public FileStream GetFileStream(string PathOrFileName, bool IsPathFileName = false)
        {
            var result=ReadFile(PathOrFileName, IsPathFileName);
            if (result == "success")
            {
                return FileStream;
            }
            return null;
        }

        /// <summary>
        /// 获取此目录下或者完整目录下的文件流
        /// </summary>
        /// <returns></returns>
        public FileStream GetFileStream()
        {
            return FileStream;
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <param name="Path">路径目录</param>
        /// <param name="FileName">文件</param>
        /// <returns>文件内容</returns>
        public string GetFileContent(string Path, string FileName)
        {
            _FileName = FileName;
            _path = Path;
            return Read();
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <returns>文件内容</returns>
        public string GetFileContent()
        {
            return Read();
        }

        /// <summary>
        /// 对文件追加内容
        /// </summary>
        /// <param name="value">内容</param>
        /// <returns></returns>
        public string AppendFile(string value)
        {
            try
            {
                Write(value,FileMode.Append);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 对文件重写内容
        /// </summary>
        /// <param name="value">内容</param>
        /// <returns></returns>
        public string AppendNewFile(string value)
        {
            try
            {
                Write(value, FileMode.Open);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        /// <summary>
        /// 对文件追加内容
        /// </summary>
        /// <param name="value">内容</param>
        /// <param name="Path">路径目录</param>
        /// <param name="FileName">文件名</param>
        /// <returns></returns>
        public string AppendFile(string value, string Path,string FileName)
        {
            try
            {
                _FileName = FileName;
                _path = Path;
                Write(value,FileMode.Append);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 对文件重写内容
        /// </summary>
        /// <param name="value">内容</param>
        /// <param name="Path">路径目录</param>
        /// <param name="FileName">文件名</param>
        /// <returns></returns>
        public string AppendNewFile(string value, string Path, string FileName)
        {
            try
            {
                _FileName = FileName;
                _path = Path;
                Write(value, FileMode.Open);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <returns></returns>
        public string CreateFile()
        {
            try
            {
                FileStream = File.Create(_path + "/" + _FileName);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="Path">路径目录</param>
        /// <param name="FileName">文件</param>
        /// <returns></returns>
        public string CreateFile(string Path, string FileName)
        {
            try
            {
                _FileName = FileName;
                _path = Path;
                Read();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        public string DeleteFile(string PathAndFileName)
        {
            try
            {
                File.Delete(PathAndFileName);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <returns></returns>
        public string DeletePath(string Path)
        {
            try
            {
                Directory.Delete(Path);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="NewFileName">新文件名</param>
        /// <param name="OldFileName">旧文件名（可为空）</param>
        /// <returns></returns>
        public string ReFileName(string NewFileName,string OldFileName = null)
        {
            try
            {
                var FileName = string.Empty;
                if (OldFileName!=null)
                {
                    FileName = _path + "/" + OldFileName;
                }
                else
                {
                    FileName = _path + "/" + _FileName;
                }
                FileInfo fi = new FileInfo(FileName);
                _FileName = NewFileName;
                fi.MoveTo(_path + "/" + _FileName);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 复制文件到新目录
        /// </summary>
        /// <param name="NewPath">新目录</param>
        /// <param name="OldPath">旧目录(可为空)</param>
        /// <param name="FileName">文件名(可为空)</param>
        /// <returns></returns>
        public string MoveFile(string NewPath,string OldPath=null,string FileName=null)
        {
            try
            {
                
                if (OldPath != null)
                {
                    _path = OldPath;
                }
                if (FileName != null)
                {
                    _FileName = FileName;
                }
                var OldPathAndFileName = _path + "/" + _FileName;
                FileInfo fi = new FileInfo(OldPathAndFileName);
                _path = NewPath;
                fi.MoveTo(_path + "/" + _FileName);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 复制文件到新目录
        /// </summary>
        /// <param name="NewPath">新目录</param>
        /// <param name="OldPath">旧目录(可为空)</param>
        /// <param name="FileName">文件名(可为空)</param>
        /// <returns></returns>
        public string CopyFile(string NewPath, string OldPath = null, string FileName = null)
        {
            try
            {

                if (OldPath != null)
                {
                    _path = OldPath;
                }
                if (FileName != null)
                {
                    _FileName = FileName;
                }
                var OldPathAndFileName = _path + "/" + _FileName;
                FileInfo fi = new FileInfo(OldPathAndFileName);
                _path = NewPath;
                fi.CopyTo(_path + "/" + _FileName);
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }

    public enum FileType
    {
        /// <summary>
        /// 文件
        /// </summary>
        File=1,
        /// <summary>
        /// 文件夹
        /// </summary>
        Folder=2,
        /// <summary>
        /// 其他
        /// </summary>
        other=3,
    }

    public class FileDetails
    {
        public string FileName { get; set; }

        public double FileSize { get; set; }

        public FileType FileType {get;set;}
    }

}
