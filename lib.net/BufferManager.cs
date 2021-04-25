using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.net
{

    /// <summary>
    /// 缓存池
    /// </summary>
    public sealed class BufferManager : IDisposable
    {
        /// <summary>
        /// 缓存池
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// 单元大小
        /// </summary>
        private int blockSize;
        /// <summary>
        /// 数量
        /// </summary>
        private int blockCount;
        /// <summary>
        /// 大小
        /// </summary>
        private int bufferSize;
        /// <summary>
        /// 当前可用索引
        /// </summary>
        private int Index;
        /// <summary>
        /// 闲置栈
        /// </summary>
        private Stack<ArraySegment<byte>> freeIndexPool;

        /// <summary>
        /// 构造缓存池
        /// </summary>
        /// <param name="BlockSize">块大小</param>
        /// <param name="Count">块数量</param>
        public BufferManager(int BlockSize = 4096, int Count = 100)
        {
            if (BlockSize < 1 || Count < 1) throw new Exception();
            blockCount = Count;
            blockSize = BlockSize;
            Index = 0;
            bufferSize = blockCount * blockSize;
            buffer = new byte[bufferSize];
            freeIndexPool = new Stack<ArraySegment<byte>>();
        }

        /// <summary>
        /// 分配缓存
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool GetBuffer(ref ArraySegment<byte> e)
        {
            if (freeIndexPool.Count > 0)
            {
                e = freeIndexPool.Pop();
                return true;
            }
            else
            {
                if (Index < bufferSize)
                {
                    e = new ArraySegment<byte>(buffer, Index, blockSize);
                    Index += blockSize;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 释放缓存
        /// </summary>
        /// <param name="e"></param>
        public void FreeBuffer(ArraySegment<byte> e)
        {
            freeIndexPool.Push(e);
            for (int i = e.Offset; i < e.Offset + blockSize; i++)
            {
                if (buffer[i] == 0) break;
                buffer[i] = 0;
            }
        }

        /// <summary>
        /// 复制缓存
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="offset"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public static byte[] newBytes(byte[] buff, int offset, int lenght)
        {
            byte[] bs = new byte[lenght];
            Buffer.BlockCopy(buff, offset, bs, 0, lenght);
            return bs;
        }


        public void Dispose()
        {
            freeIndexPool = null;
            buffer = null;
        }
    }
}
