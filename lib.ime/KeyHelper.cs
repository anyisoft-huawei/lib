using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace lib.ime
{
 
    /// <summary> 模拟键盘按键 </summary>
    public static class KeyHelper
    {
        #region 导入系统定义

        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(
        byte bVk, //虚拟键值  
        byte bScan,// 一般为0  
        int dwFlags, //这里是整数类型 0 为按下，2为释放  
        int dwExtraInfo //这里是整数类型 一般情况下设成为0  
        );

        [DllImport("user32.dll")]
        public static extern int GetFocus();
        #endregion

        #region 模拟按键
        public static void Play()
        {
            keybd_event(179, 0, 0, 0);
            keybd_event(179, 0, 2, 0);
        }

        public static void Stop()
        {
            keybd_event(178, 0, 0, 0);
            keybd_event(178, 0, 2, 0);
        }

        public static void Last()
        {
            keybd_event(177, 0, 0, 0);
            keybd_event(177, 0, 2, 0);
        }

        public static void Next()
        {
            keybd_event(176, 0, 0, 0);
            keybd_event(176, 0, 2, 0);
        }
        #endregion

        /// <summary>
        /// 模拟按下指定建
        /// </summary>
        /// <param name="_code"></param>
        public static void SendKeyPress(byte _code)
        {
            keybd_event(_code, 0, 0, 0);
            keybd_event(_code, 0, 2, 0);
        }

        /// <summary>
        /// 按下键
        /// </summary>
        /// <param name="_code"></param>
        public static void SendKeyDown(byte _code)
        {
            keybd_event(_code, 0, 0, 0);
        }

        /// <summary>
        /// 松开键
        /// </summary>
        /// <param name="_code"></param>
        public static void SendKeyUp(byte _code)
        {
            keybd_event(_code, 0, 2, 0);
        }

        /// <summary>
        /// 键码表
        /// </summary>
        public static readonly string[,] _keycodes = {
{"32","(空格)(␠)"},
{"33","!"},
{"34","双引号"},
{"35","#"},
{"36","$"},
{"37","%"},
{"38","&"},
{"39","'"},
{"40","("},
{"41",")"},
{"42","*"},
{"43","+"},
{"44",","},
{"45","-"},
{"46","."},
{"47","/"},
{"48","0"},
{"49","1"},
{"50","2"},
{"51","3"},
{"52","4"},
{"53","5"},
{"54","6"},
{"55","7"},
{"56","8"},
{"57","9"},
{"58",":"},
{"59",";"},
{"60","<"},
{"61","="},
{"62",">"},
{"63","?"},
{"64",""},
{"65","A"},
{"66","B"},
{"67","C"},
{"68","D"},
{"69","E"},
{"70","F"},
{"71","G"},
{"72","H"},
{"73","I"},
{"74","J"},
{"75","K"},
{"76","L"},
{"77","M"},
{"78","N"},
{"79","O"},
{"80","P"},
{"81","Q"},
{"82","R"},
{"83","S"},
{"84","T"},
{"85","U"},
{"86","V"},
{"87","W"},
{"88","X"},
{"89","Y"},
{"90","Z"},
{"91","["},
{"92","\""},
{"93","]"},
{"94","^"},
{"95","_"},
{"96","`"},
{"97","a"},
{"98","b"},
{"99","c"},
{"100","d"},
{"101","e"},
{"102","f"},
{"103","g"},
{"104","h"},
{"105","i"},
{"106","j"},
{"107","k"},
{"108","l"},
{"109","m"},
{"110","n"},
{"111","o"},
{"112","p"},
{"113","q"},
{"114","r"},
{"115","s"},
{"116","t"},
{"117","u"},
{"118","v"},
{"119","w"},
{"120","x"},
{"121","y"},
{"122","z"},
{"123","{"},
{"124","|"},
{"125","}"},
{"126","~"}
            };

        private static Dictionary<string, string> _dic = null;

        /// <summary>
        /// 键码字典 32-空格
        /// </summary>
        public static Dictionary<string, string> Code_Name
        {
            get
            {
                if (null == _dic)
                {
                    _dic = new Dictionary<string, string>();
                    if (_keycodes.GetLength(1) > 1)
                    {
                        for (int i = 0; i < _keycodes.GetLength(0); i++)
                        {
                            _dic.Add(_keycodes[i, 0], _keycodes[i, 1]);
                        }
                    }
                }
                return _dic;
            }
        }

    }
}
