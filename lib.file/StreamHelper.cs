using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.file
{

    public static class StreamHelper
    {

        /// <summary>
        /// 读取二进制
        /// </summary>
        /// <param name="_fs">文件流</param>
        /// <param name="index">起始位置</param>
        /// <param name="len">读取长度</param>
        /// <returns></returns>
        public static byte[] ReadBytes(this FileStream _fs, long index, long len)
        {
            byte[] bs = new byte[len];
            _fs.Position = index;
            var p = _fs.Read(bs, 0, bs.Length);
            if (p < 1) return null;
            if (p < len)
            {
                var vs = new byte[p];
                Buffer.BlockCopy(bs, 0, vs, 0, p);
                return vs;
            }
            return bs;
        }

        /// <summary>
        /// 读取字符串
        /// </summary>
        /// <param name="_fs">文件流</param>
        /// <param name="index">起始位置</param>
        /// <param name="len">读取长度</param>
        /// <param name="cn">字符编码</param>
        /// <returns></returns>
        public static string ReadString(this FileStream _fs, long index, long len, Encoding cn)
        {
            byte[] bs = new byte[len];
            _fs.Position = index;
            var p = _fs.Read(bs, 0, bs.Length);
            return p < 1 ? "" : cn.GetString(bs, 0, p);
        }

        /// <summary>
        /// 查找内容
        /// </summary>
        /// <param name="_bs">字节数组</param>
        /// <param name="bt">查找内容</param>
        /// <param name="_start">开始位置</param>
        /// <returns>返回查找到的索引</returns>
        public static int IndexOf(this byte[] _bs, byte bt, int _start = 0)
        {
            for (int i = _start; i < _bs.Length; i++)
            {
                //判断首字符是否相同
                if (_bs[i] == bt)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查找内容
        /// </summary>
        /// <param name="_bs">字节数组</param>
        /// <param name="bt">查找内容</param>
        /// <param name="_end">开始位置</param>
        /// <returns>返回查找到的索引</returns>
        public static int LastIndexOf(this byte[] _bs, byte bt, int _end = 0)
        {
            for (int i = _end; i >= 0; i--)
            {
                //判断首字符是否相同
                if (_bs[i] == bt)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 判断是否被包含另一数组
        /// </summary>
        /// <param name="_bs">数组a</param>
        /// <param name="_bt">数组b</param>
        /// <param name="_start">数组a的开始位置</param>
        /// <returns></returns>
        public static bool Contains(this byte[] _bs, byte[] _bt, int _start = 0)
        {
            if (_bs[_start] != _bt[0]) return false; //判断首字符是否相同
            for (int i = _bt.Length - 1; i > 0; i--)
            {
                if (_bs[_start + i] != _bt[i]) return false;
            }
            return true;
        }

        /// <summary>
        /// 查找内容
        /// </summary>
        /// <param name="_fs">文件流</param>
        /// <param name="_bv">内容</param>
        /// <param name="_start">开始位置</param>
        /// <returns></returns>
        public static long IndexOf(this FileStream _fs, byte _bv, long _start = 0)
        {
            int size = 2048;
            long kuai = _start;
            byte[] buffer = new byte[size];
            _fs.Position = kuai;
            //开始查找
            while (kuai >= 0)
            {
                //读取一块
                int count = _fs.Read(buffer, 0, size);
                if (count < 1) return -1;
                //开始查找
                for (int i = 1; i < count; i++)
                {
                    //判断首字符是否相同
                    if (buffer[i] == _bv) return kuai + i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查找内容
        /// </summary>
        /// <param name="_fs">文件流</param>
        /// <param name="bs">内容数组</param>
        /// <param name="size">缓存大小</param>
        /// <returns></returns>
        public static long IndexOf(this FileStream _fs, byte[] bs, int size = 4096)
        {
            byte home = bs[0];
            long kuai = 0;
            byte[] buffer = new byte[size + bs.Length];
            byte[] bcopy = new byte[bs.Length];
            //开始查找
            while (kuai >= 0)
            {
                //读取一块
                _fs.Position = kuai;
                int count = _fs.Read(buffer, bs.Length, size) + bs.Length;
                Buffer.BlockCopy(bcopy, 0, buffer, 0, bs.Length);
                Buffer.BlockCopy(buffer, size, bcopy, 0, bs.Length);
                //开始查找
                for (int i = 1; i < count; i++)
                {
                    //判断首字符是否相同
                    if (buffer[i] == home)
                    {
                        if (i + bs.Length >= count) break;//进入下组循环
                        //最后一个字符相同
                        bool b = true;
                        for (int j = bs.Length - 1; j > 0; j--)
                        {
                            if (bs[j] != buffer[i + j]) { b = false; break; }
                        }
                        if (b) return i;
                    }
                }
                kuai += size;
            }
            return -1;
        }


        /// <summary>
        /// 反向查找内容
        /// </summary>
        /// <param name="_fs">文件流</param>
        /// <param name="_bv">内容</param>
        /// <param name="_start">开始位置</param>
        /// <returns></returns>
        public static long LastIndexOf(this FileStream _fs, byte _bv, long _start = 0)
        {
            int size = 2048;
            if (_start < 1) _start = _fs.Length - 1;
            long kuai = _start;
            byte[] buffer = new byte[size];
            //开始查找
            while (kuai >= 0)
            {
                kuai -= size;
                if(kuai < 0)
                {
                    size += (int)kuai;
                    kuai = 0;
                }
                _fs.Position = kuai;
                //读取一块
                int count = _fs.Read(buffer, 0, size);
                if (count < 1) return -1;
                //开始查找
                for (int i = count - 1; i >= 0; i--)
                {
                    //判断首字符是否相同
                    if (buffer[i] == _bv) return kuai + i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查找内容
        /// </summary>
        /// <param name="_fs">文件流</param>
        /// <param name="bs">内容数组</param>
        /// <param name="size">缓存大小</param>
        /// <returns></returns>
        public static long LastIndexOf(this FileStream _fs, byte[] bs, int size = 4096)
        {
            byte end = bs[bs.Length - 1];
            long kuai = _fs.Length % size;
            byte[] buffer = new byte[size + bs.Length];
            byte[] bcopy = new byte[bs.Length];
            //最后块
            kuai = _fs.Length - kuai;
            //开始查找
            while (kuai >= 0)
            {
                //读取一块
                _fs.Position = kuai;
                int count =  _fs.Read(buffer, 0, size) + bs.Length;
                Buffer.BlockCopy(bcopy, 0, buffer, size, bs.Length);
                Buffer.BlockCopy(buffer, 0, bcopy, 0, bs.Length);
                //开始查找
                for (int i = count - 2; i >= 0; i--)
                {
                    //最后一个字符相同
                    if(buffer[i] == end)
                    {
                        int ji = i - bs.Length + 1;//首个字符
                        if (ji < 0) break;//进入下组循环
                        bool b = true;
                        for (int j = 0; j < bs.Length - 1; j++)
                        {
                            if (bs[j] != buffer[ji + j]) { b = false; break; }
                        }
                        if (b) return ji;
                    }
                }
                kuai-=size;
            }
            return -1;
        }





    }
}
