using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.file
{
    public class FileHelper
    {

        /// <summary>
        /// 获取文件下的目录
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="kz">扩展名</param>
        /// <returns></returns>
        public static List<string> GetFilesName(string path, string kz = null)
        {
            List<string> ls = new List<string>();
            if (kz != null) kz = kz.ToLower();
            foreach (FileInfo i in new DirectoryInfo(path).GetFiles())
            {
                if (kz == null || i.Extension.ToLower() == kz) ls.Add(i.Name);
            }
            return ls;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static long GetFileLength(string file)
        {
            return File.Exists(file) ? new FileInfo(file).Length : 0;
        }


    }
}
