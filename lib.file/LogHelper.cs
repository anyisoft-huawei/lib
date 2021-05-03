using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace lib.file
{
    public class LogHelper
    {


        LogHelper()
        {

        }

        /// <summary>
        /// 向文件中追加内容
        /// </summary>
        /// <param name="file"></param>
        /// <param name="msg"></param>
        /// <param name="trace">调用者信息</param>
        /// <param name="maxlenght">检查文件大小，0不检查</param>
        /// <param name="newfile">完整路径的新文件名，空自动构造文件名</param>
        public static void Write(string file, string msg, string trace = "", int maxlenght = 0, string newfile = null)
        {
            try
            {
                //判断文件大小
                if(maxlenght > 0)
                {
                   if( FileHelper.GetFileLength(file) > maxlenght)
                    {
                        file = string.IsNullOrEmpty(newfile) ? GetNewName(file) : newfile;
                    }
                    File.Move(file, newfile);
                }
                var text = string.Format("{0}\r\n{1}\r\n", trace, msg);
                File.AppendAllText(file, text);
            }
            catch (Exception)
            {
                return;
            }

        }

        /// <summary>
        /// 获取调用者信息
        /// </summary>
        /// <returns></returns>
        public static string GetTrace()
        {
            StackTrace trace = new StackTrace();
            MethodBase method = trace.GetFrame(1).GetMethod();
            return string.Format("[{0}]: \r\n{1}.{2}()", DateTime.Now.ToString(), method.DeclaringType.FullName, method.Name);
        }

        /// <summary>
        /// 使用当前文件名+日期构建一个新文件名
        /// </summary>
        /// <param name="file">文件名称</param>
        /// <param name="format">日期格式</param>
        /// <returns></returns>
        public static string GetNewName(string file, string format = "yyyymmddhhMMss")
        {
            return Path.Combine(
                Path.GetDirectoryName(file),
                string.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(file), DateTime.Now.ToString(format), Path.GetExtension(file))
                );
        }


    }
}
