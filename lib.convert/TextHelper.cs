using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace lib.convert
{
    /// <summary>
    /// 文本处理扩展方法
    /// </summary>
    public static class TextHelper
    {

        /// <summary>
        /// 转换为int，并在空值时返回默认值
        /// </summary>
        /// <param name="_input"></param>
        /// <param name="defaul"></param>
        /// <returns></returns>
        public static int ToInt(this string _input, int defaul = 0)
        {
            if (string.IsNullOrEmpty(_input)) return defaul;
            var match = Regex.Match(_input, @"\d+");
            return match.Success ? int.Parse(_input) : defaul;
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="_input"></param>
        /// <returns></returns>
        public static bool IsIP(this string _input)
        {
            return Regex.IsMatch(_input, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }


        /// <summary>
        /// 文本是否包含不在另一文本的字
        /// </summary>
        /// <param name="value"></param>
        /// <param name="symbol"></param>
        public static bool NotHasChar(string value, string symbol)
        {
            if (!string.IsNullOrEmpty(value))
            {
                foreach (var item in value)
                {
                    if (symbol.IndexOf(item) < 0) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 文本是否包含另一文本的字
        /// </summary>
        /// <param name="value"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static bool HasChar(string value, string symbol)
        {
            if (!string.IsNullOrEmpty(value))
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (symbol.IndexOf(value[i]) >= 0) return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 文本是否包含另一文本的字
        /// </summary>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static bool HasChar(string value, int count, string symbol)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (count > value.Length) count = value.Length;
                for (int i = 0; i < count; i++)
                {
                    if (symbol.IndexOf(value[i]) >= 0) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 反转字符串顺序（包括32位编码字符）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Reverse(string str)
        {
            if (string.IsNullOrEmpty(str)) return "";
            string[] ss = GetCharsArray(str);
            StringBuilder sb = new StringBuilder();
            for (int i = ss.Length - 1; i >= 0; i--)
            {
                sb.Append(ss[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取一个单个字的集合
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] GetCharsArray(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                byte[] bstr = Encoding.UTF32.GetBytes(str);//全部字
                byte[] btmp = new byte[4];//一个字
                int strCount = bstr.Length / 4;
                string[] strs = new string[strCount];
                for (int pstr = 0; pstr < bstr.Length / 4; pstr++)
                {
                    Buffer.BlockCopy(bstr, pstr * 4, btmp, 0, 4);
                    strs[pstr] = Encoding.UTF32.GetString(btmp);
                }
                return strs;
            }
            return null;
        }
        /// <summary>
        /// 获取一个单个字的集合
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> GetChars(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            byte[] bs = Encoding.UTF32.GetBytes(value);//全部字
            byte[] btmp = new byte[4];//一个字
            var vs = new List<string>();
            for (int i = 0; i < bs.Length; i += 4)
            {
                Buffer.BlockCopy(bs, i, btmp, 0, 4);
                vs.Add(Encoding.UTF32.GetString(btmp));
            }
            return vs;
        }

        /// <summary>
        /// 获取并删除指定字数的字符
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static string GetSubString(ref string value, int Count)
        {
            byte[] bs = Encoding.UTF32.GetBytes(value);//全部字
            int home = Count * 4;
            if (home < bs.Length)
            {
                value = Encoding.UTF32.GetString(bs, home, bs.Length - home);
                return Encoding.UTF32.GetString(bs, 0, home);
            }
            else
            {
                var val = value;
                value = "";
                return val;
            }
        }
        /// <summary>
        /// 获取并删除指定字数的字符，重设删除的字符数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static string GetSubString(ref string value, ref int Count)
        {
            byte[] bs = Encoding.UTF32.GetBytes(value);//全部字
            int home = Count * 4;
            if (home < bs.Length)
            {
                value = Encoding.UTF32.GetString(bs, home, bs.Length - home);
                return Encoding.UTF32.GetString(bs, 0, home);
            }
            else
            {
                var val = value;
                value = "";
                Count = bs.Length / 4;
                return val;
            }
        }

        /// <summary>
        /// 截切指定字数的字符，并返回多余的字符
        /// </summary>
        /// <param name="value"></param>
        /// <param name="HomeCount"></param>
        /// <returns></returns>
        public static string GetEndString(ref string value, int HomeCount)
        {
            byte[] bs = Encoding.UTF32.GetBytes(value);//全部字
            int home = HomeCount * 4;
            if (home < bs.Length)
            {
                value = Encoding.UTF32.GetString(bs, 0, home);
                return Encoding.UTF32.GetString(bs, home, bs.Length - home);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取字符串的长度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetCount(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            return Encoding.UTF32.GetBytes(value).Length / 4;
        }


        /// <summary>
        /// 切断字符串，并返回多余的字符
        /// </summary>
        /// <param name="info"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string GetEndString(ref string info, string symbol)
        {
            if (string.IsNullOrEmpty(info)) return "";
            int p = info.IndexOf(symbol);
            if (p >= 0)
            {
                int ep = p + symbol.Length;//尾部字符串的起点
                string end = (ep < info.Length) ? info.Substring(ep) : "";
                info = info.Substring(0, p);
                return end;
            }
            return "";
        }
        /// <summary>
        /// 切断字符串，返回之前的字符串
        /// </summary>
        /// <param name="info"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string GetHomeString(ref string info, string symbol)
        {
            if (string.IsNullOrEmpty(info)) return "";
            int p = info.IndexOf(symbol);
            if (p >= 0)
            {
                string home = info.Substring(0, p);
                int ep = p + symbol.Length;
                info = ep < info.Length ? info.Substring(ep) : "";
                return home;
            }
            return "";
        }

        /// <summary>
        /// 反向查找并切断字符串
        /// </summary>
        /// <param name="info"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string GetEndStringForLast(ref string info, string symbol)
        {
            if (!string.IsNullOrEmpty(info))
            {
                int p = info.LastIndexOf(symbol);
                if (p >= 0)
                {
                    int ep = p + symbol.Length;
                    string end = (ep < info.Length ? info.Substring(ep) : "");
                    info = info.Substring(0, p);
                    return end;
                }
            }
            return "";
        }


    }
}