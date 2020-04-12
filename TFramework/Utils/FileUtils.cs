/*
    _create: sun hai lang
    _time: 2020-03-23
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TFramework.Utils
{
    public static class FileUtils
    {
        /// <summary>
        /// 拷贝一个文件夹内容到另一个文件夹下， 并覆盖
        /// </summary>
        /// <param name="source">源文件夹</param>
        /// <param name="target">目标文件夹</param>
        /// <param name="child">是否包含子文件夹</param>
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
                    file.CopyTo(Path.Combine(target.FullName, file.Name), overwrite);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine("CopyFileRecursively Error: " + error.Message);
            }
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
        /// <returns></returns>
        public static bool DeleteEmptyDirs(string directory)
        {
            bool didDelete = false;
            string[] directoriesdirectories = Directory.GetDirectories(directory);
            for (int i = 0; i < directoriesdirectories.Length; i++)
            {
                string dir = directoriesdirectories[i];
                int filecount = Directory.GetFiles(dir).Length + Directory.GetDirectories(dir).Length;
                if (filecount > 0)
                {
                    if (DeleteEmptyDirs(dir))
                    {
                        i--;
                    }
                }
                else
                {
                    Directory.Delete(dir);
                    didDelete = true;
                }
            }
            return didDelete;
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
    }
}
