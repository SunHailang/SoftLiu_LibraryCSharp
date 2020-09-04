/*
    _create: sun hai lang
    _time: 2020-03-23
 */

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TFramework.Utils
{
    public static class FileUtils
    {
        /// <summary>
        /// 获得项目的根路径
        /// </summary>
        /// <returns></returns>
        public static string GetProjectRootPath()
        {
            // exe 运行文件夹目录
            //string environmentPath = Environment.CurrentDirectory;
            // 获取一个文件夹的父对象文件夹信息
            //string parentPath = Directory.GetParent(environmentPath).Parent.FullName;

            string rootPath = "";
            string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory; // F:\project\WPF\AstroATE-PDR\04. 程序\01. 源代码\AstroATE\AstroATE\bin\Debug
            // 向上回退三级，得到需要的目录
            rootPath = BaseDirectoryPath.Substring(0, BaseDirectoryPath.LastIndexOf("\\")); // 第一个\是转义符，所以要写两个
            rootPath = rootPath.Substring(0, rootPath.LastIndexOf(@"\"));   // 或者写成这种格式
            rootPath = rootPath.Substring(0, rootPath.LastIndexOf("\\")); // @"F:\project\WPF\AstroATE-PDR\04. 程序\01. 源代码\AstroATE\AstroATE
            return rootPath;
        }

        /// <summary>
        /// 拷贝一个文件夹内容到另一个文件夹下， 并覆盖
        /// </summary>
        /// <param name="source">源文件夹</param>
        /// <param name="target">目标文件夹</param>
        /// <param name="child">是否包含子文件夹</param>
        /// <param name="overwrite">是否重写， 默认不重写</param>
        /// <param name="withoutExtensions">不包含的 扩展名 ， eg: .meta </param>
        public static void CopyDirectoryFiles(DirectoryInfo source, DirectoryInfo target, bool child = false, bool overwrite = false, params string[] withoutExtensions)
        {
            try
            {
                if (child)
                {
                    foreach (DirectoryInfo dir in source.GetDirectories())
                    {
                        CopyDirectoryFiles(dir, target.CreateSubdirectory(dir.Name), child, overwrite, withoutExtensions);
                    }
                }
                foreach (FileInfo file in source.GetFiles())
                {
                    if (withoutExtensions != null && withoutExtensions.Contains(file.Extension))
                    {
                        continue;
                    }
                    string newFilepath = Path.Combine(target.FullName, file.Name);
                    if (!overwrite && File.Exists(newFilepath))
                    {
                        continue;
                    }
                    file.CopyTo(newFilepath, overwrite);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("CopyFileRecursively Error: " + error.Message);
            }
        }

        /// <summary>
        /// 获取 一个文件夹下的所有文件  默认包含子文件夹
        /// </summary>
        /// <param name="dir">文件夹信息</param>
        /// <param name="subfile">是否包含子文件夹  默认包含</param>
        /// <returns></returns>
        public static FileInfo[] GetDirectorAllFiles(string directory, bool subfile = true)
        {
            DirectoryInfo dir = new DirectoryInfo(directory);
            if (dir == null)
            {
                return null;
            }
            List<FileInfo> fileList = new List<FileInfo>();
            if (subfile)
            {
                foreach (DirectoryInfo item in dir.GetDirectories())
                {
                    fileList.AddRange(GetDirectorAllFiles(item.FullName, subfile));
                }
            }
            foreach (FileInfo item in dir.GetFiles())
            {
                fileList.Add(item);
            }
            return fileList.ToArray<FileInfo>();
        }

        /// <summary>
        /// 获取文件夹内文件的个数
        /// </summary>
        /// <param name="target">目标文件夹</param>
        /// <param name="count">out 个数 默认：0</param>
        /// <param name="child">是否包含子文件夹， 默认：不包含</param>
        public static void GetDirectoryFilesCount(DirectoryInfo target, out long count, bool child = false)
        {
            count = 0;
            try
            {
                if (target == null)
                {
                    return;
                }
                FileInfo[] infos = target.GetFiles();
                if (infos == null)
                {
                    return;
                }
                count += target.GetFiles().Length;
                if (child)
                {
                    DirectoryInfo[] childDirs = target.GetDirectories();
                    if (childDirs == null)
                    {
                        return;
                    }
                    foreach (DirectoryInfo dir in childDirs)
                    {
                        GetDirectoryFilesCount(dir, out count, child);
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("Get Directory Files Count Error: " + error.Message);
            }
        }

        /// <summary>
        /// 删除一个文件夹下的所有空文件夹
        /// </summary>
        /// <param name="directory">文件夹路径</param>
        /// <param name="self">是否包含自己， 默认不包含</param>
        public static void DeleteEmptyDirs(string directory, bool self = false)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    return;
                }
                string[] directoriesdirectories = Directory.GetDirectories(directory);
                if ((directoriesdirectories == null || directoriesdirectories.Length <= 0) && Directory.GetFiles(directory).Length <= 0)
                {
                    if (self)
                    {
                        Directory.Delete(directory);
                    }
                    return;
                }
                if (directoriesdirectories != null && directoriesdirectories.Length > 0)
                {
                    for (int i = directoriesdirectories.Length - 1; i >= 0; i--)
                    {
                        string dir = directoriesdirectories[i];
                        int filecount = Directory.GetFiles(dir).Length + Directory.GetDirectories(dir).Length;
                        if (filecount > 0)
                        {
                            DeleteEmptyDirs(dir, true);
                        }
                        else
                        {
                            if (Directory.Exists(dir))
                            {
                                Directory.Delete(dir);
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine($"DeleteEmptyDirs Error: {error.Message}");
            }

        }

        /// <summary>
        /// 删除一个文件夹下的所有内容
        /// </summary>
        /// <param name="directory">文件夹路径</param>
        /// <param name="self">是否包含自己， 默认：false</param>
        public static void DeleteDirsAll(string directory, bool self = false)
        {
            try
            {
                if (!Directory.Exists(directory)) return;
                DirectoryInfo dirInfo = new DirectoryInfo(directory);
                foreach (DirectoryInfo info in dirInfo.GetDirectories())
                {
                    DeleteDirsAll(info.FullName, true);
                }
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    if (File.Exists(file.FullName))
                    {
                        File.Delete(file.FullName);
                    }
                }
                if (self)
                {
                    Directory.Delete(directory);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("DeleteDirs: " + directory + " Error: " + error.Message);
            }
        }

        /// <summary>
        /// 设置开机自启动
        /// </summary>
        /// <param name="appName">名字</param>
        /// <param name="appPath">程序的路径</param>
        /// <param name="isCurrentUser">是否设置为当前用户</param>
        private static void AddStartSoft(string appName, string appPath, bool isCurrentUser = false)
        {
            if (isCurrentUser)
            {
                RegistryKey rKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (rKey.GetValue(appName) == null)
                {
                    rKey.SetValue(appName, appPath);
                }
            }
            else
            {
                RegistryKey rKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
                if (rKey.GetValue(appName) == null)
                {
                    rKey.SetValue(appName, appPath);
                }
            }
        }
    }
}
