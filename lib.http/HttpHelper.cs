using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace lib.http
{
    public static class HttpHelper
    {

        /// <summary>
        /// URL字符编码
        /// </summary>
        /// <param name="_h"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlEncode(this HttpContextBase _h, string value)
        {
            return string.IsNullOrEmpty(value) ? "" : _h.Server.UrlEncode(value);
        }

        /// <summary>
        /// URL字符解码
        /// </summary>
        /// <param name="_h"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string UrlDecode(this HttpContextBase _h, string value)
        {
            return string.IsNullOrEmpty(value) ? "" : _h.Server.UrlDecode(value);
        }

        /// <summary>
        /// 读取Cookie
        /// </summary>
        /// <param name="_h"></param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static string GetCookie(this HttpContextBase _h, string name)
        {
            if (_h.Request.Cookies != null && _h.Request.Cookies[name] != null)
                return _h.UrlDecode(_h.Request.Cookies[name].Value);
            return "";
        }

        /// <summary>
        /// 读取Cookie
        /// </summary>
        /// <param name="_h"></param>
        /// <param name="name">名称</param>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public static string GetCookie(this HttpContextBase _h, string name, string key)
        {
            if (_h.Request.Cookies != null && _h.Request.Cookies[name] != null && _h.Request.Cookies[name][key] != null)
                return _h.UrlDecode(_h.Request.Cookies[name][key]);
            return "";
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="_h"></param>
        /// <param name="name">Cookie名称</param>
        /// <param name="value">Cookie值</param>
        public static void SetCookie(this HttpContextBase _h, string name, string value)
        {
            var cookie = _h.Request.Cookies[name];
            if (cookie == null)
            {
                cookie = new HttpCookie(name);
            }
            cookie.Value = _h.UrlEncode(value);
            _h.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="_h"></param>
        /// <param name="name">Cookie名称</param>
        /// <param name="key">Cookie键名</param>
        /// <param name="value">Cookie值</param>
        public static void SetCookie(this HttpContextBase _h, string name, string key, string value)
        {
            var cookie = _h.Request.Cookies[name];
            if (cookie == null)
            {
                cookie = new HttpCookie(name);
            }
            cookie[key] = _h.UrlEncode(value);
            _h.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="_h"></param>
        /// <param name="name">Cookie名称</param>
        /// <param name="value">Cookie值</param>
        /// <param name="expires">生存分钟</param>
        public static void SetCookie(this HttpContextBase _h, string name, string value, int expires)
        {
            var cookie = _h.Request.Cookies[name];
            if (cookie == null)
            {
                cookie = new HttpCookie(name);
            }
            cookie.Value = _h.UrlEncode(value);
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            _h.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="_h"></param>
        /// <param name="name">Cookie名称</param>
        /// <param name="key">Cookie键名</param>
        /// <param name="value">Cookie值</param>
        /// <param name="expires">生存分钟</param>
        public static void SetCookie(this HttpContextBase _h, string name, string key, string value, int expires)
        {
            var cookie = _h.Request.Cookies[name];
            if (cookie == null)
            {
                cookie = new HttpCookie(name);
            }
            cookie[key] = _h.UrlEncode(value);
            cookie.Expires = DateTime.Now.AddMinutes(expires);
            _h.Response.AppendCookie(cookie);
        }

        public static bool IsPost(this HttpContextBase _h)
        {
            return _h.Request.HttpMethod.Equals("POST");
        }

        public static bool IsGet(this HttpContextBase _h)
        {
            return _h.Request.HttpMethod.Equals("GET");
        }

        /// <summary>
        /// 获取服务器变量信息
        /// </summary>
        /// <param name="_h"></param>
        /// <param name="key">服务器变量名</param>
        /// <returns></returns>
        public static string GetServerVariable(this HttpContextBase _h, string key)
        {
            return _h.Request.ServerVariables[key] ?? "";
        }

        /// <summary>
        /// 返回上一个页面的地址
        /// </summary>
        /// <param name="_h"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static string GetUrlReferrer(this HttpContextBase _h)
        {
            return null == _h.Request.UrlReferrer ? "" : _h.Request.UrlReferrer.ToString();
        }

        /// <summary>
        /// 得到当前完整主机头
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentFullHost(this HttpContextBase _h)
        {
            return _h.Request.Url.IsDefaultPort ? _h.Request.Url.Host : string.Format("{0}:{1}", _h.Request.Url.Host, _h.Request.Url.Port);
        }

        /// <summary>
        /// 判断当前访问是否来自浏览器软件
        /// </summary>
        /// <returns>当前访问是否来自浏览器软件</returns>
        public static bool IsBrowserGet(this HttpContextBase _h)
        {
            return Regex.IsMatch(_h.Request.Browser.Type.ToLower(), @"ie|opera|netscape|mozilla|konqueror|firefox");
        }

        /// <summary>
        /// 判断是否来自搜索引擎链接
        /// </summary>
        /// <returns>是否来自搜索引擎链接</returns>
        public static bool IsSearchEnginesGet(this HttpContextBase _h)
        {
            if (_h.Request.UrlReferrer == null) return false;
            return Regex.IsMatch(_h.Request.UrlReferrer.ToString().ToLower(), @"google|yahoo|msn|baidu|sogou|sohu|sina|163|lycos|tom|yisou|iask|soso|gougou|zhongsou");
        }


        /// <summary>
        /// 获得指定Url参数的值
        /// </summary> 
        /// <param name="key">Url参数</param>
        /// <returns>Url参数的值</returns>
        public static string GetQueryString(this HttpContextBase _h, string key)
        {
            return _h.Request.QueryString[key] ?? "";
        }

        /// <summary>
        /// 获得指定表单参数的值
        /// </summary>
        /// <param name="key">表单参数</param>
        /// <returns>表单参数的值</returns>
        public static string GetForm(this HttpContextBase _h, string key)
        {
            return _h.Request.Form[key] ?? "";
        }

        /// <summary>
        /// 获得Url或表单参数的值, 先判断Url参数是否为空字符串, 如为True则返回表单参数的值
        /// </summary>
        /// <param name="key">参数</param>
        /// <returns>Url或表单参数的值</returns>
        public static string GetParam(this HttpContextBase _h, string key)
        {
            var val = _h.GetQueryString(key);
            return "".Equals(val) ? _h.GetForm(key) : val;
        }

        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetIP(this HttpContextBase _h)
        {
            string result = _h.Request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(result))
                result = _h.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(result))
                result = _h.Request.UserHostAddress;
            if (string.IsNullOrEmpty(result))
                return "127.0.0.1";
            return result;
        }



    }
}