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
    /// IV包含在数据中的椭圆曲线算法对象（密匙交换算法）
    /// </summary>
    public class ECDHIVInData
    {
        CngKey CK;
        byte[] key;

        /// <summary>
        /// 创建密匙并返回公匙
        /// </summary>
        /// <returns></returns>
        public string CreateKey()
        {
            CK = CngKey.Create(CngAlgorithm.ECDiffieHellmanP256);//以ECDsaP256创建私钥
            return Convert.ToBase64String(CK.Export(CngKeyBlobFormat.EccPublicBlob));
        }

        public byte[] GetKey()
        {
            return key;
        }

        /// <summary>
        /// 用远程公匙初始化对称密匙
        /// </summary>
        /// <param name="pk"></param>
        public void SetRometoPublicKey(string pk)
        {
            byte[] bt = Convert.FromBase64String(pk);
            using (ECDiffieHellmanCng cng = new ECDiffieHellmanCng(CK))
            {
                key = cng.DeriveKeyMaterial(CngKey.Import(bt, CngKeyBlobFormat.EccPublicBlob));
            }
        }


        /// <summary>
        /// 加密数据
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="puk">对方的公匙</param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] bts)
        {
            using (ECDiffieHellmanCng cng = new ECDiffieHellmanCng(CK))
            {
                using (var aes = new AesCryptoServiceProvider())
                {
                    aes.Key = key; //设置对称加密密钥
                    aes.GenerateIV();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
                        ms.Write(aes.IV, 0, aes.IV.Length); //写入IV
                        cs.Write(bts, 0, bts.Length);//写入数据
                        cs.Close();
                        return ms.ToArray();
                    }
                }
            }
        }
        public string Encrypt(string message)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(message)));
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] data)
        {
            using (var aes = new AesCryptoServiceProvider())
            {
                var ivlength = aes.BlockSize >> 3;//获取IV长度
                byte[] ivdata = new byte[ivlength];
                Array.Copy(data, ivdata, ivlength);//获取IV
                aes.Key = key; //设置对称加密密钥
                aes.IV = ivdata;
                using (MemoryStream me = new MemoryStream())
                {
                    var cs = new CryptoStream(me, aes.CreateDecryptor(), CryptoStreamMode.Write);
                    cs.Write(data, ivlength, data.Length - ivlength);
                    cs.Close();
                    return me.ToArray();
                }
            }
        }
        public string Decrypt(string message)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(message)));
        }


    }
}
