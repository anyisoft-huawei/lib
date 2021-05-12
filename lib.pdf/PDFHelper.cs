using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using lib.file;

namespace lib.pdf
{
    /// <summary>
    /// pdf帮助类
    /// </summary>
    public class PDFHelper : IDisposable
    {

        /// <summary>
        /// 创建一个新的pdf对象
        /// </summary>
        public PDFHelper() { }

        /// <summary>
        /// 使用指定文件地址打开一个pdf
        /// </summary>
        /// <param name="file">pdf文件名</param>
        /// <param name="_edit">编辑模式打开</param>
        public PDFHelper(string file, bool _edit = false)
        {
            _Edit = false;
            Read(file);
        }

        /// <summary>
        /// 释放PDF
        /// </summary>
        public void Dispose()
        {
            _objs.Clear();
            _Xrefs.Clear();
            _pdf?.Close();
        }

        #region 字段 属性

        private bool _Edit = false;
        /// <summary>
        /// 文件名称
        /// </summary>
        public string _Name;
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get { return _Name; } }

        /// <summary>
        /// 版本
        /// </summary>
        public string _Version = "%PDF-1.5";
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get { return _Version; } }

        public byte[] End_Falg;

        /// <summary>
        /// PDF文件流
        /// </summary>
        private FileStream _pdf = null;
        /// <summary>
        /// 对象引用表
        /// </summary>
        private List<PDFXref> _Xrefs = new List<PDFXref>();
        /// <summary>
        /// 对象字典
        /// </summary>
        private Dictionary<int, PDFObjcet> _objs = new Dictionary<int, PDFObjcet>();



        private PDFCatalog _Catalog = new PDFCatalog();
        /// <summary>
        /// 目录结构
        /// </summary>
        public PDFCatalog Catalog { get { return _Catalog; } }

        private PDFPageTree _PageTree = new PDFPageTree();
        /// <summary>
        /// 页面树
        /// </summary>
        public PDFPageTree PageTree { get { return _PageTree; } }

        private PDFFontCollection _Fonts = new PDFFontCollection();
        /// <summary>
        /// 字体集合
        /// </summary>
        public PDFFontCollection Fonts { get { return _Fonts; } }
        #endregion

        #region 常量

        /// <summary>
        /// 文件编码
        /// </summary>
        Encoding _cn = Encoding.UTF8;
        /// <summary>
        /// 回车符
        /// </summary>
        const byte _r = 13;
        /// <summary>
        /// 换行符
        /// </summary>
        const byte _n = 10;
        #endregion

        #region 函数

        public static PDFHelper ReadFile(string file)
        {
            var pdf = new PDFHelper();
            pdf.Read(file);
            return pdf;
        }
        /// <summary>
        /// 读取pdf文件
        /// </summary>
        /// <param name="file">文件名</param>
        public void Read(string file)
        {
            _Xrefs.Clear();
            _objs.Clear();
            if (!File.Exists(file)) throw new Exception("文件不存在！");
            _Name = file;
            _pdf = new FileStream(file, FileMode.Open, FileAccess.Read);
            //文件头
            var p = _pdf.IndexOf(13);
            if (p < 0) return;//空内容
            var vert = _pdf.ReadString(0, p, _cn);
            if ("%PDF-" != vert.Substring(0, 5)) throw new Exception("未能识别的格式！");//是否PDF文件  
            _Version = vert; //设置版本号
            //文件结构
            GetXrefInfo();
            var v = _objs.Values.ToList();
            //读取引用表
            foreach (var item in _Xrefs)
            {
                var xref = item;
                var start = xref.home + PDFXref._start.Length + 1;
                if (start < xref.end)
                {
                    var tmp = _pdf.ReadString(start, xref.end - start, _cn);
                    xref.Load(tmp);
                }
            }
            //获取目录
            var catas = GetObject("/catalog");
            //添加目录
            if (catas.Count > 0)
            {
                var cata = catas[0];
                if (cata.typeend < cata.end)
                {
                    var bs = _pdf.ReadBytes(cata.typeend, cata.end - cata.typeend);
                    var text = _cn.GetString(bs);
                }
            }

        }

        public List<byte[]> GetLines(byte[] bs)
        {
            var list = new List<byte[]>();
            return list;
        }


        /// <summary>
        /// 获取文档结构信息
        /// </summary>
        /// <returns></returns>
        private void GetXrefInfo()
        {
            int size = 4096;//缓存大小
            var buffer = new byte[size];//缓存块
            List<byte> tmp = new List<byte>();//暂存块
            PDFObjcet a_obj = null;//当前对象
            PDFXref a_xref = null;//当前引用表
            //正则参数
            var reg_obj = new Regex(@"^(\d+) (\d+) obj$", RegexOptions.Compiled);
            var reg_type = new Regex(@"<< /Type /[\w]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            //开始工作
            int objfalg = -1;//对象标志
            _pdf.Position = 0;
            long kuai = 0;
            while (true)
            {
                kuai = _pdf.Position;//记住当前位置
                int home = -1;//开始位置
                int end = 0;//结束位置
                int count = _pdf.Read(buffer, 0, size);
                if (count > 0)
                {
                    //检查块内容
                    var lbs = buffer.ToList();
                    for (int i = 0; i < count; i++)
                    {
                        if (buffer[i] == _r || buffer[i] == _n)
                        {
                            //转移标记
                            home = end;
                            end = i;
                            //判断是否要读取标记
                            if (home >= 0)
                            {
                                home++;
                                int v_count = end - home;
                                //是否需要进行处理
                                if (v_count + tmp.Count < 50 || objfalg == 1)//-----------------------
                                {
                                    if (home < end) tmp.AddRange(lbs.GetRange(home, end - home));//拼接
                                    var val = _cn.GetString(tmp.ToArray()).Trim();
                                    //存类型结束地址
                                    if (objfalg == 1)
                                    {
                                        a_obj.typeend = kuai + i;
                                        a_obj.PramasTree.SetValue(val);
                                        objfalg = 21;
                                    }
                                    //检查对象头
                                    var match = reg_obj.Match(val);
                                    if (match.Success)
                                    {
                                        objfalg = 1;
                                        a_obj = new PDFObjcet(int.Parse(match.Groups[1].Value));
                                        a_obj.eg = int.Parse(match.Groups[2].Value);
                                        a_obj.home = kuai + home;
                                        _objs.Add(a_obj.id, a_obj);
                                    }
                                    //判断标记类型
                                    switch (val)
                                    {
                                        case "endobj":
                                            objfalg = -1;
                                            a_obj.end = kuai + home;
                                            break;
                                        case "xref":
                                        case "startxref":
                                            if (objfalg != 3) { a_xref = new PDFXref(); _Xrefs.Add(a_xref); }
                                            objfalg = 2;
                                            a_xref.home = kuai + home;
                                            break;
                                        case "%%EOF":
                                            objfalg = -1;
                                            a_xref.end = kuai + home;
                                            break;
                                        case "trailer":
                                            objfalg = 3;
                                            a_xref = new PDFXref();
                                            _Xrefs.Add(a_xref);
                                            a_xref.home = kuai + home;
                                            break;
                                        default:
                                            //对类型进行处理
                                            break;
                                    }
                                }
                                tmp.Clear();
                            }
                        }
                    }
                    end++;
                    if (end < lbs.Count) tmp.AddRange(lbs.GetRange(end, lbs.Count - end));
                    if (count < size) break;
                }
                else
                {
                    break;
                }
            }
            if (tmp.Count > 0)
            {
                End_Falg = tmp.ToArray();
                tmp.Clear();
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="_type">类型</param>
        /// <returns></returns>
        private List<PDFObjcet> GetObject(string _type)
        {
            var list = new List<PDFObjcet>();
            foreach (var item in _objs.Values)
            {
                if (item._Type.ToLower() == _type)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public void Save(string file)
        {
            if (!File.Exists(file)) File.Create(file).Close();
        }

        #endregion

        #region 子类

        /// <summary>
        /// PDF引用表
        /// </summary>
        public class PDFXref
        {
            public long trailer;
            /// <summary>
            /// 对象的起始标记地址
            /// </summary>
            public long home;
            /// <summary>
            /// 对象的结束标记地址
            /// </summary>
            public long end;
            public List<XrefGroup> Groups = new List<XrefGroup>();
            /// <summary>
            /// 头标记
            /// </summary>
            public const string _start = "startxref";
            /// <summary>
            /// 尾标记
            /// </summary>
            public const string _stop = "%%EOF";
            public void Load(string data)
            {
                int i = 1;
            }

            /// <summary>
            /// 引用组
            /// </summary>
            public class XrefGroup
            {
                public int homeid;
                public int count;
                public List<Xref> Xrefs = new List<Xref>();
            }

            /// <summary>
            /// 引用信息
            /// </summary>
            public struct Xref
            {
                /// <summary>
                /// 地址
                /// </summary>
                public int address;
                /// <summary>
                /// 编号
                /// </summary>
                public int id;
                /// <summary>
                /// 标志
                /// </summary>
                public string flag;
            }
        }

        /// <summary>
        /// 对象类型
        /// </summary>
        public enum PDFObjcetType
        {
            /// <summary>
            /// 字体
            /// </summary>
            Font
        }

        /// <summary>
        /// PDF对象
        /// </summary>
        public class PDFObjcet
        {

            /// <summary>
            /// 对象的起始标记地址
            /// </summary>
            public long home = -1;
            /// <summary>
            /// 对象的结束标记地址
            /// </summary>
            public long end = -1;
            /// <summary>
            /// 类型结束标记
            /// </summary>
            public long typeend = -1;

            /// <summary>
            /// 对象id
            /// </summary>
            public int id;
            /// <summary>
            /// 标记
            /// </summary>
            public int eg;
            /// <summary>
            /// 参数树
            /// </summary>
            public PDFPramas PramasTree = new PDFPramas();
            /// <summary>
            /// 对象头
            /// </summary>
            public string Header { get { return string.Format("{0} {1} obj", id, eg); } }
            /// <summary>
            /// 对象类型
            /// </summary>
            public string _Type { get { return PramasTree._Type; } }

            public PDFObjcet() { }
            public PDFObjcet(int num) { id = num; }

            /// <summary>
            /// 定义虚函数，由继承类实现
            /// </summary>
            /// <returns></returns>
            public virtual byte[] ToBytes()
            {
                return null;
            }

            public override string ToString()
            {
                return string.Format("{0} {1} obj {2}", id, eg, _Type);
            }


        }

        /// <summary>
        /// PDF参数树
        /// </summary>
        public class PDFPramas
        {
            /// <summary>
            /// 隐藏值
            /// </summary>
            private string _Value;
            /// <summary>
            /// 隐藏节点
            /// </summary>
            private List<PDFPramas> _Nodes = null;
            /// <summary>
            /// 隐藏对象类型
            /// </summary>
            private PDFPramas _type;

            /// <summary>
            /// 对象类型
            /// </summary>
            public string _Type { get { return null == _type ? "" : _type.Value; } }
            /// <summary>
            /// 键名
            /// </summary>
            public string Key;
            /// <summary>
            /// 键值
            /// </summary>
            public string Value { get { return _Value; } }
            /// <summary>
            /// 键节点
            /// </summary>
            public List<PDFPramas> Nodes { get { return _Nodes.ToList(); } }
            /// <summary>
            /// 备注
            /// </summary>
            public string _end;

            /// <summary>
            /// 添加新节点
            /// </summary>
            /// <param name="key">节点名称</param>
            /// <returns></returns>
            public PDFPramas AddNode(string key)
            {
                var pm = new PDFPramas();
                pm.Key = key.Trim();
                if ("/type" == pm.Key.ToLower()) _type = pm;
                _Nodes.Add(pm);
                return pm;
            }

            /// <summary>
            /// 设置值
            /// </summary>
            /// <param name="_v">值</param>
            public void SetValue(string _v)
            {
                _Value = null;//初始化值
                _Nodes = null;//初始化子节点
                int falg = 0;
                int start = -1;
                PDFPramas pm = null;
                bool key = false;
                _v = _v.Trim();
                if (_v.IndexOf("<<") == 0)
                {
                    _v = _v.Substring(2);
                    _Nodes = new List<PDFPramas>();
                    int p = _v.LastIndexOf(">>");
                    if (p >= 0)
                    {
                        _end = _v.Substring(p);
                        _end = _end.Length > 2 ? _end.Substring(2) : "";
                        _v = _v.Substring(0, p);
                    }
                    for (int i = 0; i < _v.Length; i++)
                    {
                        switch (_v[i])
                        {
                            case '/'://名称
                                if (0 == falg)
                                {
                                    if (key)
                                    {
                                        pm = AddNode(_v.Substring(start, i - start));
                                    }
                                    else
                                    {
                                        if (null != pm) pm.SetValue(_v.Substring(start, i - start));
                                    }
                                    start = i;//标记起点
                                    key = !key;
                                }
                                break;
                            case ' ':
                                if (0 == falg && key)
                                {
                                    pm = AddNode(_v.Substring(start, i - start));
                                    key = false;
                                    start = i;//标记起点
                                }
                                break;
                            case '<':
                            case '[':
                                if (0 == falg && key)
                                {
                                    pm = AddNode(_v.Substring(start, i - start));
                                    key = false;
                                    start = i;//标记起点
                                }
                                falg++;
                                if (_v[1] == '<') i++;
                                break;
                            case '>':
                            case ']':
                                falg--;
                                if (_v[1] == '>') i++;
                                break;
                            default:
                                break;
                        }
                    }
                    if (start > 0)
                    {
                        pm.SetValue(_v.Substring(start));
                    }
                }
                else _Value = _v;
            }

            /// <summary>
            /// 移除节点
            /// </summary>
            /// <param name="key">节点名称</param>
            public void Remove(string key)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (key == Nodes[i].Key)
                    {
                        if (_type == Nodes[i]) _type = null;
                        Nodes.RemoveAt(i);
                    }
                }
            }

            /// <summary>
            /// 节点值获取
            /// </summary>
            /// <returns></returns>
            private string GetNodesValue()
            {
                if (null == _Nodes) return "";
                var sb = new StringBuilder();
                sb.Append("<<");
                foreach (var kv in _Nodes)
                {
                    sb.Append(kv.ToString());
                }
                sb.Append(">>");
                return sb.ToString();
            }

            /// <summary>
            /// 键内容
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("{0}{1}{2}", Key, (null == _Nodes ? string.Format(" {0}", _Value) : GetNodesValue()), _end);
            }

        }



        /// <summary>
        /// PDF目录结构
        /// </summary>
        public class PDFCatalog : PDFObjcet
        {

        }

        /// <summary>
        /// PDF页面树
        /// </summary>
        public class PDFPageTree : PDFObjcet
        {

        }

        /// <summary>
        /// PDF字体
        /// </summary>
        public class PDFFontCollection
        {

        }

        /// <summary>
        /// PDF字体
        /// </summary>
        public class PDFFont : PDFObjcet
        {
            /// <summary>
            /// 字体名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 基字体
            /// </summary>
            public string BaseFont;

            public string SubType;
            public string _Encoding;

            public override string ToString()
            {
                var name = Enum.GetName(typeof(PDFObjcetType), PDFObjcetType.Font);
                return base.ToString();
            }
        }

        #endregion




        /// <summary>
        /// 颜色（RGB 0.0-1.0）
        /// </summary>
        public struct ColorSpec
        {
            private double red1;
            private double green1;
            private double blue1;
            public string red;
            public string green;
            public string blue;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="R"></param>
            /// <param name="G"></param>
            /// <param name="B"></param>
            public ColorSpec(uint R, uint G, uint B)
            {
                //转换为0.0-1.0表示
                red1 = R; green1 = G; blue1 = B;
                red1 = Math.Round((red1 / 255), 3);
                green1 = Math.Round((green1 / 255), 3);
                blue1 = Math.Round((blue1 / 255), 3);
                red = red1.ToString();
                green = green1.ToString();
                blue = blue1.ToString();
            }
        }

        /// <summary>
        /// 页面尺寸（1/72英寸）.
        /// </summary>
        public struct PageSize
        {
            /// <summary>
            /// 宽度
            /// </summary>
            public uint xWidth;
            /// <summary>
            /// 高度
            /// </summary>
            public uint yHeight;
            /// <summary>
            /// 左边距
            /// </summary>
            public uint leftMargin;
            /// <summary>
            /// 右边距
            /// </summary>
            public uint rightMargin;
            /// <summary>
            /// 上边距
            /// </summary>
            public uint topMargin;
            /// <summary>
            /// 下边距
            /// </summary>
            public uint bottomMargin;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="width">宽度</param>
            /// <param name="height">高度</param>
            public PageSize(uint width, uint height)
            {
                xWidth = width;
                yHeight = height;
                leftMargin = 0;
                rightMargin = 0;
                topMargin = 0;
                bottomMargin = 0;
            }

            /// <summary>
            /// 页边距
            /// </summary>
            /// <param name="L">左边距</param>
            /// <param name="T">上边距</param>
            /// <param name="R">右边距</param>
            /// <param name="B">下边距</param>
            public void SetMargins(uint L, uint T, uint R, uint B)
            {
                leftMargin = L;
                rightMargin = R;
                topMargin = T;
                bottomMargin = B;
            }
        }//end struct PageSize

        /// <summary>
        /// 对齐
        /// </summary>
        public enum Align
        {
            /// <summary>
            /// 左对齐
            /// </summary>
            LeftAlign,
            /// <summary>
            /// 居中的对齐
            /// </summary>
            CenterAlign,
            /// <summary>
            /// 右对齐
            /// </summary>
            RightAlign
        }

        /// <summary>
        /// 表格
        /// </summary>
        public struct TableParams
        {
            /// <summary>
            /// X 坐标
            /// </summary>
            public uint xPos;
            /// <summary>
            /// Y 坐标
            /// </summary>
            public uint yPos;
            /// <summary>
            /// 行数
            /// </summary>
            public uint numRow;
            /// <summary>
            /// 列宽
            /// </summary>
            public uint columnWidth;
            /// <summary>
            /// 列数
            /// </summary>
            public uint numColumn;
            /// <summary>
            /// 行高
            /// </summary>
            public uint rowHeight;
            /// <summary>
            /// 表格宽度
            /// </summary>
            public uint tableWidth;
            /// <summary>
            /// 表格高度
            /// </summary>
            public uint tableHeight;
            /// <summary>
            /// 列宽度集合
            /// </summary>
            public uint[] columnWidths;


            /// <summary>
            /// 创建可变宽表格
            /// </summary>
            /// <param name="numColumns">列数</param>
            /// <param name="widths">宽度集合</param>
            public TableParams(uint numColumns, params uint[] widths)
            {
                xPos = yPos = numRow = columnWidth = rowHeight = tableHeight = 0;
                tableWidth = 0;
                numColumn = numColumns;
                columnWidths = new uint[numColumn];
                columnWidths = widths;
                SetTableWidth();
            }


            /// <summary>
            /// 创建表格
            /// </summary>
            /// <param name="numColumns">列数</param>
            public TableParams(uint numColumns)
            {
                xPos = yPos = numRow = columnWidth = rowHeight = tableHeight = 0;
                tableWidth = 0;
                numColumn = numColumns;
                columnWidths = null;
                columnWidth = 0;
            }

            /// <summary>
            /// 初始化表格宽度
            /// </summary>
            private void SetTableWidth()
            {
                for (uint i = 0; i < numColumn; i++)
                    tableWidth += columnWidths[i];
            }
        }//end struct TableParams




    }
}
