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
        /// 将不安全文本转换为int，并在文本中不包含数字时返回默认值
        /// </summary>
        /// <param name="text">不安全文本</param>
        /// <param name="defaul">默认值</param>
        /// <returns>转换后的值</returns>
        public static int ToInt(this string text, int defaul = 0)
        {
            if (string.IsNullOrEmpty(text)) return defaul;
            var match = Regex.Match(text, @"\d+");
            return match.Success ? int.Parse(match.Value) : defaul;
        }

        /// <summary>
        /// 将不安全文本转换为long，并在文本中不包含数字时返回默认值
        /// </summary>
        /// <param name="text">不安全文本</param>
        /// <param name="defaul">默认值</param>
        /// <returns>转换后的值</returns>
        public static long ToLong(this string text, long defaul = 0)
        {
            if (string.IsNullOrEmpty(text)) return defaul;
            var match = Regex.Match(text, @"\d+");
            return match.Success ? long.Parse(match.Value) : defaul;
        }

        /// <summary>
        /// 将不安全文本转换为float，并在文本中不包含数字时返回默认值
        /// </summary>
        /// <param name="text">不安全文本</param>
        /// <param name="defaul">默认值</param>
        /// <returns>转换后的值</returns>
        public static float ToDouble(this string text, float defaul = 0)
        {
            if (string.IsNullOrEmpty(text)) return defaul;
            var match = Regex.Match(text, @"\d+\.\d+");
            return match.Success ? float.Parse(match.Value) : defaul;
        }

        /// <summary>
        /// 将不安全文本转换为double，并在文本中不包含数字时返回默认值
        /// </summary>
        /// <param name="text">不安全文本</param>
        /// <param name="defaul">默认值</param>
        /// <returns>转换后的值</returns>
        public static double ToDouble(this string text, double defaul = 0)
        {
            if (string.IsNullOrEmpty(text)) return defaul;
            var match = Regex.Match(text, @"\d+\.\d+");
            return match.Success ? double.Parse(match.Value) : defaul;
        }


        /// <summary>
        /// 将文本转为安全正则文本
        /// </summary>
        /// <param name="text">不文本</param>
        /// <returns>转换后的文本</returns>
        public static string ToSafeRegexText(this string text)
        {
            var val = text;           
            foreach (var item in "\\^$*+?{}.()|[]")
            {
                if (val.Contains(item)) val = val.Replace(item.ToString(), "\\" + item);
            }
            return val;
        }

        /// <summary>
        /// 判断文本的IP类型，ipv4返回4，ipv6返回6，不为ip返回0
        /// </summary>
        /// <param name="text">要检查的字符串</param>
        /// <returns>类型数字</returns>
        public static int IsIpVersion(string text)
        {
            return Regex.IsMatch(text, @"^\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}$") ? 4 :
                (Regex.IsMatch(text, @"^\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}$") ? 6 : 0);
        }


        /// <summary>
        /// 文本是否包含另一文本(symbol)中的所有字符
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <param name="symbol">字符集合</param>
        /// <returns></returns>
        public static bool HasAllChar(this string value, string symbol)
        {
            return !Regex.IsMatch(value, string.Format("[^{0}]", symbol.ToSafeRegexText()));
        }

        /// <summary>
        /// 文本是否存在另一文本(symbol)中的字符
        /// </summary>
        /// <param name="value">要检查的字符串</param>
        /// <param name="symbol">字符集合</param>
        /// <returns></returns>
        public static bool HasChar(string value, string symbol)
        {
            return Regex.IsMatch(value, string.Format("[{0}]", symbol.ToSafeRegexText()));
        }

        /// <summary>
        /// 反转(包含32位编码的)字符串顺序
        /// </summary>
        /// <param name="text">字符串</param>
        /// <returns>反转结果</returns>
        public static string Reverse32(this string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            byte[] bs = Encoding.UTF32.GetBytes(text);
            byte[] rbs = new byte[bs.Length];
            int i = 0;//开始的字符
            int j = bs.Length - 4;//最后的字符
            while (i < bs.Length)
            {
                Buffer.BlockCopy(bs, i, rbs, j, 4);
                i += 4;//bs下一个字符
                j -= 4;//rbs上一个字符
            }
            return Encoding.UTF32.GetString(rbs);
        }

        /// <summary>
        /// 将(包含32位编码的)字符串转为字符集合
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string[] ToChar32Array(this string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                byte[] bs = Encoding.UTF32.GetBytes(text);
                byte[] tmp = new byte[4];//一个字
                int len = bs.Length / 4;
                string[] vs = new string[len];
                for (int p = 0; p < len; p++)
                {
                    Buffer.BlockCopy(bs, p * 4, tmp, 0, 4);
                    vs[p] = Encoding.UTF32.GetString(tmp);
                }
                return vs;
            }
            return null;
        }

        /// <summary>
        /// 获取指定字数的字符串,并从源字符串中删除获取的字符串
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string GetSubString(ref string text, int len)
        {
            byte[] bs = Encoding.UTF32.GetBytes(text);//全部字
            int home = len * 4;
            if (home < bs.Length)
            {
                text = Encoding.UTF32.GetString(bs, home, bs.Length - home);
                return Encoding.UTF32.GetString(bs, 0, home);
            }
            else
            {
                var val = text;
                text = "";
                return val;
            }
        }

        /// <summary>
        /// 获取(包含32位编码的)字符串的长度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetLength32(this string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            return Encoding.UTF32.GetBytes(value).Length / 4;
        }

        /// <summary>
        /// 截切指定字数的字符串，并返回截切的字符串
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CutEndString(ref string text, int len)
        {
            byte[] bs = Encoding.UTF32.GetBytes(text);//全部字
            int home = len * 4;
            if (home >= bs.Length) return "";
            text = Encoding.UTF32.GetString(bs, 0, home);
            return Encoding.UTF32.GetString(bs, home, bs.Length - home);
        }

        /// <summary>
        /// 从指定字符位置截切字符串，并返回截切的字符串
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="symbol">标记字符</param>
        /// <returns></returns>
        public static string CutEndString(ref string text, string symbol)
        {
            if (string.IsNullOrEmpty(text)) return "";
            int p = text.IndexOf(symbol);
            if (p < 0) return "";
            int ep = p + symbol.Length;//尾部字符串的起点
            string end = (ep < text.Length) ? text.Substring(ep) : "";
            text = text.Substring(0, p);
            return end;
        }

        /// <summary>
        /// 反向查找截切字符串，并返回截切的字符串
        /// </summary>
        /// <param name="text"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static string CutEndStringForLast(ref string text, string symbol)
        {
            if (string.IsNullOrEmpty(text)) return "";
            int p = text.LastIndexOf(symbol);
            if (p < 0) return "";
            int ep = p + symbol.Length;
            string end = (ep < text.Length ? text.Substring(ep) : "");
            text = text.Substring(0, p);
            return end;
        }

        /// <summary>
        /// 剪切字符串，返回剪切的字符串
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="len">长度</param>
        /// <returns></returns>
        public static string CutHomeString(ref string text, int len)
        {
            byte[] bs = Encoding.UTF32.GetBytes(text);//全部字
            int home = len * 4;
            if (home >= bs.Length) return "";
            var val = Encoding.UTF32.GetString(bs, 0, home);
            text = Encoding.UTF32.GetString(bs, home, bs.Length - home);
            return val;
        }

        /// <summary>
        /// 剪切字符串，返回剪切的字符串
        /// </summary>
        /// <param name="text">字符串</param>
        /// <param name="symbol">标记字符</param>
        /// <returns></returns>
        public static string CutHomeString(ref string text, string symbol)
        {
            if (string.IsNullOrEmpty(text)) return "";
            int p = text.IndexOf(symbol);
            if (p < 0) return "";
            string home = text.Substring(0, p);
            int ep = p + symbol.Length;
            text = ep < text.Length ? text.Substring(ep) : "";
            return home;
        }



    }
}