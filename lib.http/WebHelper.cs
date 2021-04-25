using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace lib.http
{
    public class WebHelper
    {
        private CookieContainer _Cookie = new CookieContainer();

        public void AddCookie(string host, string key, string val)
        {
            _Cookie.Add(new Uri(host), new Cookie(key, val));
        }
        public void SetCookies(string host, string cookies)
        {
            _Cookie.SetCookies(new Uri(host), cookies);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string Get(string uri)
        {
            string responseString;
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.CookieContainer = _Cookie;
            request.Timeout = 6000;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                if (stream == null)
                    throw new Exception("stream = null");
                if (response.CharacterSet == null)
                    throw new Exception("response.CharacterSet = null");
                using (var sr = new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet)))
                    responseString = sr.ReadToEnd();
            }
            return responseString;
        }

        public byte[] GetBytes(string uri)
        {
            var bytes = new List<byte>();

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.CookieContainer = _Cookie;
            request.UserAgent = "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.43 Safari/537.31";

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                var buffer = new byte[512];
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    bytes.AddRange(buffer.ToList().GetRange(0, read));
            }

            return bytes.ToArray();
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Post(string uri, string data)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.CookieContainer = _Cookie;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36 TheWorld 7";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Timeout = 6000;
            //发送
            var bs = Encoding.UTF8.GetBytes(data);
            request.ContentLength = bs.Length;
            using (var stream = request.GetRequestStream())
                stream.Write(bs, 0, bs.Length);
            //接收
            string res_val;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null) throw new Exception("stream = null");
                    if (response.CharacterSet == null) throw new Exception("response.CharacterSet = null");
                    using (var sr = new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet)))
                        res_val = sr.ReadToEnd();
                }
            }
            return res_val;

        }

        /// <summary>
        /// 表单方式提交数据
        /// </summary>
        /// <param name="uri">post地址</param>
        /// <param name="data">表单参数</param>
        /// <param name="files">表单文件</param>
        /// <param name="head">额外头</param>
        /// <returns></returns>
        public string Post(string uri, List<KeyValuePair<string, string>> data = null, List<KeyValuePair<string, string>> files = null, List<KeyValuePair<string, string>> head = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.CookieContainer = _Cookie;
            request.Method = "POST";
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36 TheWorld 7";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            if (head != null)
            {
                foreach (var item in head)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }
            request.Timeout = 6000;
            //发送
            var ms = new MemoryStream();
            // 边界符
            var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
            var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            if (null != data)
            {
                string dataformat = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
                foreach (var kv in data)
                {
                    ms.Write(beginBoundary, 0, beginBoundary.Length);
                    var line = string.Format(dataformat, kv.Key, kv.Value);
                    var linebytes = Encoding.UTF8.GetBytes(line);
                    ms.Write(linebytes, 0, linebytes.Length);
                }
            }
            if (null != files)
            {
                string fileformat = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                var endfile = Encoding.UTF8.GetBytes("\r\n");
                foreach (var kv in data)
                {
                    if (File.Exists(kv.Value))
                    {
                        ms.Write(beginBoundary, 0, beginBoundary.Length);
                        var line = string.Format(fileformat, kv.Key, kv.Value);
                        var linebytes = Encoding.UTF8.GetBytes(line);
                        ms.Write(linebytes, 0, linebytes.Length);
                        var file = File.ReadAllBytes(kv.Value);
                        ms.Write(file, 0, file.Length);
                        ms.Write(endfile, 0, endfile.Length);
                    }
                }
            }
            // 结束符
            var end = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");
            ms.Write(end, 0, end.Length);
            var bs = ms.ToArray();
            ms.Close();
            request.ContentLength = bs.Length;
            using (var stream = request.GetRequestStream())
                stream.Write(bs, 0, bs.Length);
            //接收
            string res_val;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null) throw new Exception("stream = null");
                    if (response.CharacterSet == null) throw new Exception("response.CharacterSet = null");
                    using (var sr = new StreamReader(stream, Encoding.GetEncoding(response.CharacterSet)))
                        res_val = sr.ReadToEnd();
                }
            }
            return res_val;

        }

        public void Dispose()
        {
            _Cookie = null;
        }
    }
}
