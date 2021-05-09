using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace lib.file
{
    public class MemoryHelper
    {
        public class MemoryFile
        {

            private string _Name;
            private int _Size;
            private MemoryMappedFile _file;
            private MemoryMappedViewAccessor _rw;
            /// <summary>
            /// 共享名
            /// </summary>
            private string Name { get { return _Name; } }
            /// <summary>
            /// 大小
            /// </summary>
            private int Size { get { return _Size; } }
            /// <summary>
            /// 共享文件
            /// </summary>
            public MemoryMappedFile MappedFile { get { return _file; } }
            /// <summary>
            /// 访问控制器
            /// </summary>
            public MemoryMappedViewAccessor ViewAccessor { get { return _rw; } }

            /// <summary>
            /// 创建内存映射文件
            /// </summary>
            /// <param name="MemoryName"></param>
            /// <param name="size"></param>
            public MemoryFile(string MemoryName, int size)
            {
                _Name = MemoryName;
                _Size = size;
                //创建映射文件
                _file = MemoryMappedFile.CreateOrOpen(MemoryName, _Size, MemoryMappedFileAccess.ReadWrite);
                _rw = _file.CreateViewAccessor(0, _Size, MemoryMappedFileAccess.ReadWrite);
            }


            /// <summary>
            /// 读取结构
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="position">位置</param>
            /// <returns></returns>
            public T Read<T>(long position)
            {
                byte[] data = new byte[Marshal.SizeOf(typeof(T))];
                _rw.ReadArray(position, data, 0, data.Length);//读数据
                return data.ToStructure<T>();//字节数组解析到结构体  
            }
            /// <summary>
            /// 写入结构
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="position"></param>
            /// <param name="t"></param>
            public void Write<T>(long position, ref T t)
            {
                byte[] buffer = t.ConventToBytes();
                _rw.WriteArray(position, buffer, 0, buffer.Length);  //写数据
            }

            public void Dispose()
            {
                _rw?.Dispose();
                _file?.Dispose();
            }
        }





    }
}
