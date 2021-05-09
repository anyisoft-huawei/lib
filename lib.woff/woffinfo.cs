using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace lib.woff
{
    /// <summary>
    /// woff头，包含44字节
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct woffheader
    {
        /// <summary>
        /// woff标识 0x774F4646
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public char[] 标识;
        /// <summary>
        /// sfnt 版本
        /// </summary>
        public uint 字体版本;
        /// <summary>
        /// 总长度
        /// </summary>
        public uint 长度;
        /// <summary>
        /// 目录条目数
        /// </summary>
        public ushort 条目数;
        /// <summary>
        /// 保留，设为0
        /// </summary>
        public ushort 保留;
        /// <summary>
        /// Sfnt总大小
        /// </summary>
        public uint 原始总大小;
        /// <summary>
        /// 主要版本
        /// </summary>
        public ushort 主要版本;
        /// <summary>
        /// 次要版本
        /// </summary>
        public ushort 次要版本;
        /// <summary>
        /// 压缩数据开始位置
        /// </summary>
        public uint 数据起始;
        /// <summary>
        /// 压缩数据长度
        /// </summary>
        public uint 数据长度;
        /// <summary>
        /// 压缩数据原始长度
        /// </summary>
        public uint 原数据长度;
        /// <summary>
        /// 私用数据开始位置
        /// </summary>
        public uint 私用起始;
        /// <summary>
        /// 私用数据长度
        /// </summary>
        public uint 私用长度;

    }


    /// <summary>
    /// woff表结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct wofftable
    {
        /// <summary>
        /// 标识
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.U1)]
        public char[] 标识;
        /// <summary>
        /// 开始位置
        /// </summary>
        public uint 起始;
        /// <summary>
        /// 长度
        /// </summary>
        public uint 长度;
        /// <summary>
        /// 原始长度
        /// </summary>
        public uint 原长度;
        /// <summary>
        /// 原始校验和
        /// </summary>
        public uint 原校验和;

    }

 


}
