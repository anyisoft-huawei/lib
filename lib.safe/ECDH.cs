using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace lib.safe
{

    /// <summary>
    /// 椭圆曲线算法对象（密匙交换算法）
    /// </summary>
    public class ECDH
    {
        ECDiffieHellmanCng ecdh;
        public byte[] PublicKey;
        private byte[] key;
        /// <summary>
        /// 创建一个ECDH对象，并初始化公匙
        /// </summary>
        public ECDH()
        {
            ecdh = new ECDiffieHellmanCng();
            ecdh.HashAlgorithm = CngAlgorithm.ECDiffieHellmanP256;//指定算法
            PublicKey = ecdh.PublicKey.ToByteArray();
        }
        /// <summary>
        /// 创建一个ECDH对象，并利用远程公匙创建对称密匙
        /// </summary>
        /// <param name="RometoPulicKey"></param>
        public ECDH(byte[] RometoPulicKey)
        {
            using (ECDiffieHellmanCng ecd = new ECDiffieHellmanCng())
            {
                ecd.HashAlgorithm = CngAlgorithm.ECDiffieHellmanP256;
                PublicKey = ecd.PublicKey.ToByteArray();
                key = ecd.DeriveKeyMaterial(CngKey.Import(RometoPulicKey, CngKeyBlobFormat.EccPublicBlob));

            }
        }

        ~ECDH()
        {
            ecdh = null;
            PublicKey = null;
            key = null;
        }
        /// <summary>
        /// 利用远程公匙创建对称密匙
        /// </summary>
        /// <param name="RometoPulicKey"></param>
        public void SetKey(byte[] RometoPulicKey)
        {
            key = ecdh.DeriveKeyMaterial(CngKey.Import(RometoPulicKey, CngKeyBlobFormat.EccPublicBlob));
            ecdh = null;
        }

        public byte[] Encrypt(byte[] bt, out string iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                iv = Encoding.UTF8.GetString(aes.IV);

                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bt, 0, bt.Length);
                    cs.Close();
                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// 加密消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public string Encrypt(string msg, out string iv)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(msg), out iv));
        }

        /// <summary>
        /// 解密消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public string Decrypt(byte[] bt, string iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.IV = Encoding.UTF8.GetBytes(iv);
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(bt, 0, bt.Length);
                    cs.Close();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }


        /// <summary>
        /// 解密消息
        /// </summary>
        /// <param name="message">Base64编码字符串</param>
        /// <param name="iv">初始化向量</param>
        /// <returns></returns>
        public string Decrypt(string message, string iv)
        {
            return Decrypt(Convert.FromBase64String(message), iv);
        }
    }
}
