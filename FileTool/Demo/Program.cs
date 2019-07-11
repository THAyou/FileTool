using FileTools;
using System;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            FTPTool serviceOne = new FTPTool("192.168.131.170");
            serviceOne.FileRename("ftp://192.168.131.170/ApplyCashe/20190711/636984372880238219.txt", "NEW.TXT");
        }
    }
}
