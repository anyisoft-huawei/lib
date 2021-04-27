using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.convert
{

    /// <summary>
    /// int帮助类
    /// </summary>
    public static class IntHelper
    {


        /// <summary>
        /// 获取数字的对应汉字形式,123:一二三
        /// </summary>
        /// <param name="_int"></param>
        /// <returns></returns>
        public static string ToGBKText(this int _int)
        {
            string _GBN = "-0123456789.";
            string _GBK = "负〇一二三四五六七八九点";
            var val = _int.ToString();
            StringBuilder sb = new StringBuilder();
            foreach (char i in val)
            {
                int index = _GBN.IndexOf(i);
                if (index >= 0)
                {
                    sb.Append(_GBK.Substring(index, 1));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 小写数字
        /// </summary>
        public static readonly string[] _GBK = { "〇", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
        /// <summary>
        /// 小写单位
        /// </summary>
        public static readonly string[] _GBKU = { "个", "十", "百", "千", "万", "十", "百", "千", "亿" };
        /// <summary>
        /// 大写数字
        /// </summary>
        public static readonly string[] _GBT = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
        /// <summary>
        /// 大写单位
        /// </summary>
        public static readonly string[] _GBTU = { "个", "拾", "佰", "仟", "萬", "拾", "佰", "仟", "亿" };

        /// <summary>
        /// 转换数字为汉字,123:一百二十三
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToGBKUText(long val)
        {
            return ToGBText(val, _GBK, _GBKU);
        }

        /// <summary>
        /// 转换数字为汉字,123:壹佰贰拾叁
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToGBKTText(long val)
        {
            return ToGBText(val, _GBT, _GBTU);
        }

        /// <summary>
        /// 转换位汉字形式
        /// </summary>
        /// <param name="_long"></param>
        /// <param name="_GBV">{ "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" }</param>
        /// <param name="_GBU">{ "个", "拾", "佰", "仟", "萬", "拾", "佰", "仟", "亿" }</param>
        /// <returns></returns>
        public static string ToGBText(this long _long,string[] _GBV, string[] _GBU)
        {
            string val = (_long < 0) ? (-_long).ToString() : _long.ToString();
            string _v = "";//存值
            int UnitIndex = 0;
            for (int i = val.Length - 1; i >= 0; i--)
            {
                if (UnitIndex >= _GBU.Length) UnitIndex = 1;
                _v = _GBV[int.Parse(val.Substring(i, 1))] + _GBU[UnitIndex] + _v;
                UnitIndex++;
            }
            _v = _v.Replace(_GBU[0], "");
            _v = _v.Replace(_GBV[0] + _GBU[3], _GBV[0]);
            _v = _v.Replace(_GBV[0] + _GBU[2], _GBV[0]);
            _v = _v.Replace(_GBV[0] + _GBU[1], _GBV[0]);
            if (_v.Length < 4) _v = _v.Replace(_GBV[1] + _GBU[1], _GBU[1]);
            while (true)
            {
                if (_v.IndexOf(_GBV[0] + _GBV[0]) > 0) _v = _v.Replace(_GBV[0] + _GBV[0], _GBV[0]); else break;
            }
            _v = _v.Replace(_GBV[0] + _GBU[8], _GBU[8]);
            _v = _v.Replace(_GBV[0] + _GBU[4], _GBU[4]);
            if (_v.Substring(_v.Length - 1, 1) == _GBV[0]) _v = _v.Substring(0, _v.Length - 1);
            return _long < 0 ? "负" + _v : _v;
        }

        /// <summary>
        /// 将汉字 文本(一百二十一) 转为 数字文本(121)
        /// </summary>
        /// <returns></returns>
        public static string ToGBNText(this string _text)
        {
            if (!string.IsNullOrEmpty(_text)) return "";
            string _GBK = "一二三四五六七八九";
            string _GBN = "123456789";
            StringBuilder sb = new StringBuilder();
            string[] _ys = _text.Split('亿');//分割亿单位内
            foreach (string _yv in _ys)
            {
                //依次处理亿单位
                string _wv = "";//用来存储转换后的文本
                if (!string.IsNullOrEmpty(_yv))
                {
                    string[] _ws = _yv.Split('万');//分割万单位内              
                    for (int iw = _ws.Length - 1; iw >= 0; iw--) //依次处理万单位
                    {
                        if (_wv.Length >= 8) break;//达到8位跳出循环
                        string _nv = "";//用来存储转换后的文本
                        if (!string.IsNullOrEmpty(_ws[iw]))
                        {
                            string _gv = _ws[iw];//当前万单位文本
                            //从个位开始遍历
                            for (int i = _gv.Length - 1; i >= 0; i--)
                            {
                                if (_nv.Length >= 4) break;//达到4位跳出循环                              
                                switch (_gv[i]) //检查单位
                                {
                                    case '十':
                                        _nv = _nv.PadLeft(1, '0');//十位补0
                                        continue;
                                    case '百':
                                        _nv = _nv.PadLeft(2, '0');//百位补0
                                        continue;
                                    case '千':
                                        _nv = _nv.PadLeft(3, '0');//千位补0
                                        continue;
                                }
                                int p = _GBK.IndexOf(_gv[i]);
                                if (p >= 0) _nv = _GBN[p] + _nv;
                            }//字符串处理完成
                        }
                        _nv = _nv.PadLeft(4, '0');//万位补0
                        _wv = _nv + _wv;//拼接万位
                    }//ws
                }//y
                _wv = _wv.PadLeft(8, '0');//亿位补零
                sb.Append(_wv);//拼接亿位
            }//ys
            return sb.ToString();
        }



    }
}
