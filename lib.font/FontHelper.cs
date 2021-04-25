using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace lib.font
{

    /// <summary>
    /// 字体帮助类
    /// </summary>
    public class FontHelper
    {

        private static List<string> _SystemFonts;
        /// <summary>
        /// App字体集合
        /// </summary>
        private static PrivateFontCollection AppFonts = new PrivateFontCollection();
        /// <summary>
        /// 系统字体名称集合
        /// </summary>
        public static List<string> FontNames
        {
            get
            {
                if (_SystemFonts == null)
                {
                    var vs = new List<string>();
                    using (var fonts = new InstalledFontCollection())
                    {
                        foreach (FontFamily family in fonts.Families)
                        {
                            vs.Add(family.Name);
                        }
                        _SystemFonts = vs;
                    } 
                }
                return _SystemFonts;
            }
        }
        /// <summary>
        /// 应用字体名称集合
        /// </summary>
        public static List<string> AppFontNames
        {
            get
            {
                var vs = new List<string>();
                foreach (FontFamily family in AppFonts.Families)
                {
                    vs.Add(family.Name);
                }
                return vs;
            }
        }
        /// <summary>
        /// 所有字体名称集合
        /// </summary>
        public static List<string> AllFontNames
        {
            get
            {
                var vs = new List<string>();
                vs.AddRange(FontNames);
                vs.AddRange(AppFontNames);
                return vs;
            }
        }

        /// <summary>
        /// 添加资源字体
        /// </summary>
        /// <param name="bytes"></param>
        public static void AddFont(byte[] bytes)
        {
            IntPtr MeAdd = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, MeAdd, bytes.Length);
            AppFonts.AddMemoryFont(MeAdd, bytes.Length);
        }

        /// <summary>
        /// 添加文件字体
        /// </summary>
        /// <param name="path"></param>
        public static void AddFont(string path)
        {
            AppFonts.AddFontFile(path);
        }

        /// <summary>
        /// 获取资源字体
        /// </summary>
        /// <param name="bytes">字体数据</param>
        /// <param name="size">字体大小</param>
        /// <param name="fontstyle">字体风格</param>
        /// <returns></returns>
        public static Font GetFont(string name, int size = 12, FontStyle fontstyle = FontStyle.Regular)
        {
            foreach (FontFamily family in AppFonts.Families)
            {
                if(family.Name == name)
                {
                    return new Font(family, size, fontstyle);
                }
            }
            return new Font(name, size, fontstyle);
        }


    }
}
