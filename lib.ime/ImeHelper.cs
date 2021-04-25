using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace lib.ime
{
    public class ImeHelper
    {
        #region 事件  
        /// <summary>  
        /// 输入文本事件  
        /// </summary>  
        public delegate void EventInputText(char ch);
        /// <summary>  
        /// 输入文本事件  
        /// </summary>  
        public event EventInputText InputText;
        #endregion

        #region 私有字段  
        IntPtr hIMC;
        IntPtr handle;
        private const int WM_IME_SETCONTEXT = 0x0281;
        private const int WM_CHAR = 0x0102;
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct POINTAPI
        {
            public int x;
            public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct COMPOSITIONFORM
        {
            public uint dwStyle;
            public POINTAPI ptCurrentPos;
            public RECT rcArea;
        }
        #endregion

        #region 构造方法  
        public ImeHelper(IntPtr handle)
        {
            hIMC = ImmGetContext(handle);
            this.handle = handle;
        }
        #endregion

        #region 方法  
        /// <summary>  
        /// 输入事件  
        /// </summary>  
        /// <param name="m"></param>  
        public void ImmOperation(object m)
        {
            //if (m.Msg == WM_IME_SETCONTEXT && m.WParam.ToInt32() == 1)
            //{
            //    this.ImmAssociateContext(handle);
            //}
            //if (m.Msg == WM_CHAR)
            //{
            //    KeyEventArgs e = new KeyEventArgs(((Keys)((int)((long)m.WParam))) | Control.ModifierKeys);
            //    InputText((char)e.KeyData);//触发输入事件  
            //}
        }
        /// <summary>
        /// 设置输入法位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void setImePosition(int x, int y)
        {

            COMPOSITIONFORM cf = new COMPOSITIONFORM();
            cf.dwStyle = 2;
            cf.ptCurrentPos.x = x;
            cf.ptCurrentPos.y = y;
            bool setcom = ImmSetCompositionWindow(hIMC, ref cf);

            //ImmReleaseContext(hIMC, handle);
        }


        #endregion

        #region Win Api  
        /// <summary>  
        /// 建立指定输入环境与窗口之间的关联  
        /// </summary>  
        /// <param name="hWnd"></param>  
        /// <returns></returns>  
        private IntPtr ImmAssociateContext(IntPtr hWnd)
        {
            return ImmAssociateContext(hWnd, hIMC);
        }

        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("Imm32.dll")]
        private static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern int ImmCreateContext();
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern bool ImmDestroyContext(int hImc);
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        private static extern int ImmGetCompositionStringW(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);

        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        public static extern int ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("imm32.dll", CharSet = CharSet.Auto)]
        public static extern bool ImmSetCompositionWindow(IntPtr hIMC, ref COMPOSITIONFORM lpCompForm);
        #endregion
    }
}
