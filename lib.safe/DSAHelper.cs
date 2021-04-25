using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace lib.safe
{
    class DSAHelper
    {
      
        /// <summary>
        /// 计算数据的签名
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Sign(byte[] bs, string key)
        {
            using (DSACryptoServiceProvider dsa = new DSACryptoServiceProvider())
            {
                dsa.FromXmlString(key);
                byte[] bh = dsa.SignData(bs);
                return Convert.ToBase64String(bh);
            }
        }

        /// <summary>
        /// 验证数据的签名
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="key"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool Verify(byte[] bs, string key, string hash)
        {
            using (DSACryptoServiceProvider dsa = new DSACryptoServiceProvider())
            {
                dsa.FromXmlString(key);
                return dsa.VerifyData(bs, Convert.FromBase64String(hash));
            }
        }

        /// <summary>
        /// 计算文件的签名
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Sign(string filename, string key)
        {
            using (DSACryptoServiceProvider dsa = new DSACryptoServiceProvider())
            {
                dsa.FromXmlString(key);
                if (!File.Exists(filename)) throw new Exception("文件不存在！");
                using (StreamReader sr = new StreamReader(filename))
                {
                    byte[] bh = dsa.SignData(sr.BaseStream);

                    return Convert.ToBase64String(bh);
                }
            }
        }

        /// <summary>
        /// 验证文件的签名
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="key"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool Sign(string filename, string key, string hash)
        {
            using (DSACryptoServiceProvider dsa = new DSACryptoServiceProvider())
            {
                dsa.FromXmlString(key);
                if (!File.Exists(filename)) throw new Exception("文件不存在！");
                byte[] bh = File.ReadAllBytes(filename);
                return dsa.VerifyData(bh, Convert.FromBase64String(hash));
            }
        }

    }

}
