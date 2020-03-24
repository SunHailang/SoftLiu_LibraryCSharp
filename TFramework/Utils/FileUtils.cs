/*
    _create: sun hai lang
    _time: 2020-03-23
 */

using System.IO;

namespace TFramework.Utils
{
    public static class FileUtils
    {
        public static void CopyFile()
        {

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
    }
}
