using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRemover
{
    class Program
    {
        static void Main(string[] args)
        {
            String folderNames = File.ReadAllText("FolderNames.txt");
            List<String> directoryList = new List<String>();
            directoryList.AddRange(folderNames.Split(','));

            String baseDir = Convert.ToString(ConfigurationManager.AppSettings["BaseDirectory"]);
            String[] fileFormats = Convert.ToString(ConfigurationManager.AppSettings["FileType"]).Split(',');
            DateTime lastDate = Convert.ToDateTime(Convert.ToString(ConfigurationManager.AppSettings["LastDate"]));

            UtilityClass utilClass = new UtilityClass();

            utilClass.init(@"D:\FileRemover\Log\Log_DeleteFiles_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".csv");
            //utilClass.WriteLine("------------Proccess started------------");
            //utilClass.WriteLine("Searching: " + folderNames + "\nFileTypes: " + Convert.ToString(ConfigurationManager.AppSettings["FileType"]));
            //utilClass.WriteLine("Deleting files before: " + lastDate.ToString("dd-MMM-yyyy HH:mm:ss"));
            utilClass.WriteLine("Deleted File Name,Path,Size in Bytes,DateCreated,LastModified");
            //String directory = Convert.ToString(ConfigurationManager.AppSettings["DirectoryPath"]);
            //String[] fileFormats = Convert.ToString(ConfigurationManager.AppSettings["FileType"]).Split(',');
            //String[] fileFormats = new string[] { "pdf", "log" };

            for (int i = 0; i < directoryList.Count(); i++)
            {
                directoryList[i] = directoryList[i].Trim();
            }

            for (int i = 0; i < fileFormats.Count(); i++)
            {
                fileFormats[i] = fileFormats[i].Trim();
            }

            foreach (String dir in directoryList)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(baseDir);
                utilClass.DirectoryTree(directoryInfo, fileFormats, lastDate);
            }

            utilClass.WriteLine("------------Proccess Ended------------");

            //foreach (String directory in directoryList)
            //{
            //    DirectoryInfo directoryInfo = new DirectoryInfo(directory);
            //    foreach (String format in fileFormats)
            //    {
            //        FileInfo[] files = directoryInfo.GetFiles("*." + format + "*");

            //        foreach (FileInfo fileInfo in files)
            //        {
            //            if (fileInfo.CreationTime < DateTime.Now.AddMonths(-12) && fileInfo.LastWriteTime < DateTime.Now.AddMonths(-12))
            //                fileInfo.Delete();
            //        }
            //    }
            //}
        }
    }

    class UtilityClass
    {
        static string Logpath;

        public void DirectoryTree(DirectoryInfo root, String[] fileFormats, DateTime lastDate)
        {
            List<FileInfo> files = new List<FileInfo>();
            List<DirectoryInfo> subDir = new List<DirectoryInfo>();

            //WriteLine("CurrentDirectory: " + root.FullName);

            try
            {
                foreach (String format in fileFormats)
                {
                    files.AddRange(root.GetFiles("*." + format + "*"));
                }
            }
            catch (Exception ex)
            {
                WriteLine("FailToRead : " + ex.Message + "\n" + ex.StackTrace);
            }

            if (files.Count != 0)
            {
                try
                {
                    foreach (FileInfo fileInfo in files)
                    {
                        if (fileInfo.CreationTime < lastDate && fileInfo.LastWriteTime < lastDate)
                        {
                            //fileInfo.Delete();
                            Console.WriteLine("Deleted: " + fileInfo.Name);
                            WriteLine(fileInfo.Name + "," + root.FullName + "," + fileInfo.Length + "," + fileInfo.CreationTime + "," + fileInfo.LastWriteTime);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLine("FailToDelete : " + ex.Message + "\n" + ex.StackTrace);
                }

            }
            //else
            //{
            //    WriteLine("No files found!");
            //}
            try
            {
                subDir.AddRange(root.GetDirectories());

                if (subDir.Count != 0)
                {
                    foreach (DirectoryInfo dirInfor in subDir)
                    {
                        DirectoryTree(dirInfor, fileFormats, lastDate);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine("FailToReadDirectory : " + ex.Message + "\n" + ex.StackTrace);
            }

        }

        public void init(string logFilePath)
        {
            Logpath = logFilePath;
        }

        //public void WriteLine(string strLog)
        //{
        //    FileStream fileStream = null;
        //    DirectoryInfo logDirInfo = null;
        //    FileInfo logFileInfo;
        //    logFileInfo = new FileInfo(Logpath);
        //    logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
        //    if (!logDirInfo.Exists) logDirInfo.Create();
        //    if (!logFileInfo.Exists)
        //    {
        //        fileStream = logFileInfo.Create();
        //    }
        //    else
        //    {
        //        fileStream = new FileStream(Logpath, FileMode.Append);
        //    }
        //    StreamWriter log = new StreamWriter(fileStream);
        //    log.WriteLine("Message: " + strLog);
        //    log.WriteLine("Timestamp: " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss"));
        //    log.WriteLine("<--> :");
        //    log.WriteLine("<--> :");
        //    log.Close();
        //    log.Dispose();
        //    fileStream.Close();
        //}

        public void WriteLine(string strLog)
        {
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;
            logFileInfo = new FileInfo(Logpath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(Logpath, FileMode.Append);
            }
            StreamWriter log = new StreamWriter(fileStream);
            log.WriteLine(strLog);
            log.Close();
            log.Dispose();
            fileStream.Close();
        }
    }
}
