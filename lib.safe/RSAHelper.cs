using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace lib.safe
{
    public class RSAHelper
    {
        public class RSAExchange
        {
            RSACryptoServiceProvider rsaRemote;
            RSACryptoServiceProvider rsaLocal;

            /// <summary>
            /// 创建交换RSA对象
            /// </summary>
            /// <param name="rKey">解密数据需要的公匙</param>
            /// <param name="lKey">用于加密的密匙对</param>
            public RSAExchange(string rKey, string lKey)
            {
                rsaRemote = new RSACryptoServiceProvider();
                rsaRemote.FromXmlString(rKey);
                rsaLocal = new RSACryptoServiceProvider();
                rsaLocal.FromXmlString(lKey);
            }

            ~RSAExchange()
            {
                rsaRemote = null;
                rsaLocal = null;
            }

            /// <summary>
            /// 加密数据
            /// </summary>
            /// <param name="data">加密数据</param>
            /// <returns></returns>
            public string EncryptForLocal(string data)
            {
                byte[] publicValue = rsaLocal.Encrypt(Encoding.UTF8.GetBytes(data), false);
                return Convert.ToBase64String(publicValue);
            }

            /// <summary>
            /// 解密数据
            /// </summary>
            /// <param name="data">解密数据</param>
            /// <returns></returns>
            public string DecryptForRemote(string data)
            {
                byte[] privateValue = rsaRemote.Decrypt(Convert.FromBase64String(data), false);
                return Encoding.UTF8.GetString(privateValue);
            }
        }

     

        /// <summary>
        /// RAS非对称加密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <returns></returns>
        public static string AsymmetricEncrypt(string data, string key)
        {
            RSACryptoServiceProvider rsaPublic = new RSACryptoServiceProvider();
            rsaPublic.FromXmlString(key);
            byte[] publicValue = rsaPublic.Encrypt(Encoding.UTF8.GetBytes(data), false);
            return Convert.ToBase64String(publicValue);
        }

        /// <summary>
        /// RAS非对称解密
        /// </summary>
        /// <param name="data">解密数据</param>
        /// <returns></returns>
        public static string AsymmetricDecrypt(string data, string key)
        {
            RSACryptoServiceProvider rsaPrivate = new RSACryptoServiceProvider();
            rsaPrivate.FromXmlString(key);
            byte[] privateValue = rsaPrivate.Decrypt(Convert.FromBase64String(data), false);
            return Encoding.UTF8.GetString(privateValue);
        }

        /// <summary>  
        /// RAS单密匙加密。  
        /// </summary>  
        public static string Encrypt(string msg, string pk)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(msg), pk), Base64FormattingOptions.None);
        }
        /// <summary>  
        /// RAS单密匙加密。  
        /// </summary>  
        public static byte[] Encrypt(byte[] bs, string pk)
        {
            CspParameters param = new CspParameters();
            param.KeyContainerName = pk;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                int MaxBlockSize = rsa.KeySize / 8 - 11;
                if (bs.Length <= MaxBlockSize) return rsa.Encrypt(bs, false);
                using (MemoryStream ms = new MemoryStream(bs))
                using (MemoryStream cs = new MemoryStream())
                {
                    byte[] Buffer = new byte[MaxBlockSize];
                    int BlockSize = ms.Read(Buffer, 0, MaxBlockSize);
                    while (BlockSize > 0)
                    {
                        byte[] ToEncrypt = new byte[BlockSize];
                        Array.Copy(Buffer, 0, ToEncrypt, 0, BlockSize);
                        byte[] Cryptograph = rsa.Encrypt(ToEncrypt, false);
                        cs.Write(Cryptograph, 0, Cryptograph.Length);
                        BlockSize = ms.Read(Buffer, 0, MaxBlockSize);
                    }
                    return cs.ToArray();
                }
            }
        }

        /// <summary>  
        /// RAS单密匙解密。  
        /// </summary>  
        public static string Decrypt(string securitylString, string pk)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(securitylString), pk));
        }

        /// <summary>  
        /// RAS单密匙解密。  
        /// </summary>  
        public static byte[] Decrypt(byte[] bs, string pk)
        {
            CspParameters param = new CspParameters();
            param.KeyContainerName = pk;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                int MaxBlockSize = rsa.KeySize / 8;
                if (bs.Length <= MaxBlockSize) return rsa.Decrypt(bs, false);
                using (MemoryStream cs = new MemoryStream(bs))
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] Buffer = new byte[MaxBlockSize];
                    int BlockSize = cs.Read(Buffer, 0, MaxBlockSize);
                    while (BlockSize > 0)
                    {
                        byte[] ToDncrypt = new byte[BlockSize];
                        Array.Copy(Buffer, 0, ToDncrypt, 0, BlockSize);
                        byte[] Plaintext = rsa.Decrypt(ToDncrypt, false);
                        ms.Write(Plaintext, 0, Plaintext.Length);
                        BlockSize = cs.Read(Buffer, 0, MaxBlockSize);

                    }
                    return ms.ToArray();
                }
            }
        }
    }

}
