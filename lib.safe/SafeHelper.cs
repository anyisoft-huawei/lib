using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace lib.safe
{
    public static class SafeHelper
    {

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="_v"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] EasyEncrypt(this string _v, string key)
        {
            byte[] b = Encoding.UTF8.GetBytes(_v);
            byte[] k = Encoding.UTF8.GetBytes(key);
            int j = 0;
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)(b[i] ^ k[j]);
                j++;
                if (j >= k.Length) j = 0;
            }
            return b;
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="b"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string EasyDecrypt(this byte[] b, string key)
        {
            byte[] k = Encoding.UTF8.GetBytes(key);
            int j = 0;
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = (byte)(b[i] ^ k[j]);
                j++;
                if (j >= k.Length) j = 0;
            }
            return Encoding.UTF8.GetString(b);
        }


        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="_v">要加密的字符串</param>
        /// <returns></returns>
        public static string GetMD5(this string _v)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] bt = (new ASCIIEncoding()).GetBytes(_v);
                byte[] _vs = md5.ComputeHash(bt);
                string val = BitConverter.ToString(_vs);
                val = val.Replace("-", "");
                val = val.ToLower();
                return val;
            }
        }

        /// <summary>
        ///  计算指定文件的MD5值
        /// </summary>
        /// <param name="filename">指定文件的完全限定名称</param>
        /// <returns>返回值的字符串形式</returns>
        public static string GetFileMd5(string filename)
        {
            //检查文件是否存在
            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        byte[] buffer = md5.ComputeHash(fs);
                        //将字节数组转换成十六进制的字符串形式
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            sb.Append(buffer[i].ToString("x2"));
                        }
                        return sb.ToString();
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 返回文件的哈希值
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <returns></returns>
        public static string GetFileSHA1(string filename)
        {
            //检查文件是否存在
            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (SHA1 sha1 = SHA1.Create())
                    {
                        byte[] buffer = sha1.ComputeHash(fs);
                        //将字节数组转换成十六进制的字符串形式
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            sb.Append(buffer[i].ToString("x2"));
                        }
                        return sb.ToString();
                    }
                }
            }
            return string.Empty;
        }


        /// <summary>
        ///  计算指定文件的CRC32值
        /// </summary>
        /// <param name="filename">指定文件的完全限定名称</param>
        /// <returns>返回值的字符串形式</returns>
        public static string GetCRC32(string filename)
        {
            //检查文件是否存在
            if (File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (CRC32 crc = new CRC32())
                    {
                        byte[] buffer = crc.ComputeHash(fs);
                        //将字节数组转换成十六进制的字符串形式
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            sb.Append(buffer[i].ToString("x2"));
                        }
                        return sb.ToString();
                    }
                }
            }
            return string.Empty;
        }


        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="utf8">UTF8</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns></returns>
        public static byte[] DESEncrypt(this byte[] utf8, string key, string iv)
        {
            byte[] btKey = Encoding.UTF8.GetBytes(key);
            byte[] btIV = Encoding.UTF8.GetBytes(iv);

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
            {
                cs.Write(utf8, 0, utf8.Length);
                cs.FlushFinalBlock();
                return ms.ToArray();
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="_bs"></param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns></returns>
        public static byte[] DESDecrypt(this byte[] _bs, string key, string iv)
        {
            byte[] btKey = Encoding.UTF8.GetBytes(key);
            byte[] btIV = Encoding.UTF8.GetBytes(iv);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                {
                    cs.Write(_bs, 0, _bs.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="utf8">UTF8</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>Base64String</returns>
        public static string DESEncrypt(this string utf8, string key, string iv)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(utf8).DESEncrypt(key, iv));
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="base64">Base64String</param>
        /// <param name="key">密钥</param>
        /// <param name="iv">向量</param>
        /// <returns>UTF8</returns>
        public static string DESDecrypt(this string base64, string key, string iv)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64).DESDecrypt(key, iv));
        }


    }
}
