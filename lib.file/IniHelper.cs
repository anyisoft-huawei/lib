using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace lib.file
{
    class IniHelper
    {
        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 读取配置信息
        /// </summary>
        /// <param name="ini">ini文件地址</param>
        /// <param name="section">段</param>
        /// <param name="key">键名</param>
        /// <param name="_default">默认值</param>
        /// <returns></returns>
        public static string GetValue(string ini, string section, string key, string _default)
        {
            if (File.Exists(ini))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(section, key, _default, temp, 1024, ini);
                return temp.ToString(); 
            }
            else
            {
                return _default;
            }
        }

        /// <summary>
        /// 写入配置信息
        /// </summary>
        /// <param name="ini">ini文件地址</param>
        /// <param name="section">名称</param>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool SetValue(string ini, string section, string key, string value)
        {
            if (!File.Exists(ini)) return false;
            long i = WritePrivateProfileString(section, key, value, ini);
            return 0 == i ? false : true;
        }

    }
}
