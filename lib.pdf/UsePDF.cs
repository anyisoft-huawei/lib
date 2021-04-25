using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace lib.pdf
{
    public class UsePDF
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdfFileName">PDF文件名称</param>
        public UsePDF(string pdfFileName)
        {
            FileName = pdfFileName;
        }
        #region PDF

        private pdfTrailer Trailer;

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; private set; }
        private pdfStream pdfFile;

        public pdfHeader Header;
        public pdfXref Xref;

        public void Read()
        {
            try
            {
                Open();
                if (pdfFile.IndexOf(KeyDict.pdf) != 0) throw new Exception("未能识别的格式！");

                long i = pdfFile.LastIndexOf(KeyDict.trailer);
                if (i >= 0)
                {
                    Trailer = new pdfTrailer(pdfFile.ReadString(i, pdfFile.Length - i));
                    long j =i- Trailer.StartXref;
                    Xref = new pdfXref(pdfFile.ReadString(Trailer.StartXref, j));


                }
            }
            catch (Exception)
            {
                Close();
                throw;
            }
           

        }






        /*******************************************************
         * 
         * 主体部分
         * 
         * *****************************************************/


            public static string getKeyValue(string str,string cr,out string value)
        {
            int i = str.IndexOf(cr);
            if (i > 0 )
            {
                value = str.Substring(i, str.Length - i);
                return str.Substring(0, i);
            }
            value = "";
            return str;
        }






        /// <summary>
        /// 打开文件
        /// </summary>
        private void Open()
        {
            try
            {
                pdfFile = new pdfStream(FileName);
                pdfFile.Position = 0;
            }
            catch (Exception)
            {
                pdfFile.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        private void Close()
        {
            try
            {
                if (pdfFile != null)
                {
                    pdfFile.Close();
                    pdfFile.Dispose();
                }
            }
            catch (Exception)
            {
                pdfFile.Dispose();
                throw;
            }
        }





        public struct KeyDict
        {
            public const string pdf = "%PDF-";
            public const string obj = "obj\r\n";
            public const string endobj = "endobj\r\n";
            public const string trailer = "trailer";
        }
        public class pdfStream : FileStream
        {

            public pdfStream(string FileName) : base(FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite) { }


            /// <summary>
            /// 写入字符串，并返回字符串位长度
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public int WriteString(string str)
            {
                try
                {
                    Position = Length;
                    byte[] buffer = Encoding.UTF8.GetBytes(str);
                    Write(buffer, 0, buffer.Length);
                    return buffer.Length;
                }
                catch (Exception)
                {
                    throw;
                }
            }


            public string ReadString(long star,long strLength)
            {
                try
                {
                    byte[] buffer = new byte[strLength];
                    Position = star;
                    if (Read(buffer, 0, buffer.Length) > 0)
                    {
                        return Encoding.UTF8.GetString(buffer);
                    }
                    throw new Exception("读取文件出错！");
                }
                catch (Exception)
                {
                    throw;
                }
            }


            /// <summary>
            /// 从后往前在流中查找
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public long LastIndexOf(string str, out int strLast)
            {
                if (str == null) throw new Exception("查找的字符串为 null\r\n在 pdfStream.LastIndexOf");
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                strLast = buffer.Length - 1;
                int i = strLast;
                long index = Length;
                while (index > 0)
                {
                    Position = index;
                    if (ReadByte() == buffer[i])
                    {
                        if (i <= 0) { return index; }
                        i--;
                    }else{ i = strLast; }
                    index--;
                }
                return -1;
            }

            /// <summary>
            /// 从后往前在流中查找
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public long LastIndexOf(string str)
            {
                if (str == null) throw new Exception("查找的字符串为 null\r\n在 pdfStream.LastIndexOf");
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                int strLast = buffer.Length - 1;
                int i = strLast;
                long index = Length;
                while (index > 0)
                {
                    Position = index;
                    if (ReadByte() == buffer[i])
                    {
                        if (i <= 0) { return index; }
                        i--;
                    }
                    else { i = strLast; }
                    index--;
                }
                return -1;
            }

            /// <summary>
            /// 在流中查找
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public long IndexOf(string str,out int strLast)
            {
                if (str == null) throw new Exception("查找的字符串为 null\r\n在 pdfStream.IndexOf");
                Position = 0;
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                strLast = buffer.Length - 1;
                int i = 0;
                while (Position < Length)
                {
                    if (ReadByte() == buffer[i])
                    {
                        if (i >= strLast) { return Position - strLast-1; }
                        i++;
                    }
                    else { i = 0; }
                }
                return -1;
            }
            /// <summary>
            /// 在流中查找
            /// </summary>
            /// <param name="str"></param>
            /// <returns></returns>
            public long IndexOf(string str)
            {
                if (str == null) throw new Exception("查找的字符串为 null\r\n在 pdfStream.IndexOf");
                Position = 0;
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                int strLast = buffer.Length - 1;
                int i = 0;
                while (Position < Length)
                {
                    if (ReadByte() == buffer[i])
                    {
                        if (i >= strLast) { return Position - strLast-1; }
                        i++;
                    }
                    else { i = 0; }
                }
                return -1;
            }



        }


        public class pdfHeader
        {
            public string Version="1.7";

            public byte[] getHeader()
            {
                string str = string.Format("%PDF-{0}\n%\u00b5\u00b5\u00b5\u00b5\n", Version);
                return Encoding.UTF8.GetBytes(str); ;
            }
        }

        /// <summary>
        /// 对象位置表
        /// </summary>
        public class pdfXref
        {
            int xrefStar;
            int xrefCount;
            List<string> XrefArray;
            public pdfXref()
            {

            }
            public pdfXref(string xref)
            {
                string[] xf= xref.Split(Environment.NewLine.ToCharArray());
                string[] xc= xf[1].Split(' ');
                xrefStar = int.Parse(xc[0]);
                xrefCount = int.Parse(xc[1]);
                XrefArray = new List<string>();
                for (int i= 0;i< xrefCount; i++)
                {
                    XrefArray.Add(xf[i + 2]);
                }
            }




        }

        /// <summary>
        /// 文件尾
        /// </summary>
        public class pdfTrailer
        {
            public Dictionary<string, string> Dict;
            public long StartXref;

            public pdfTrailer()
            {
            }
            public pdfTrailer(string trailer)
            {
                Dict = new Dictionary<string, string>();
                int i=trailer.IndexOf("startxref");
                if (i > 0)
                {
                    string strHome = trailer.Substring(0, i);
                    string strEnd = trailer.Substring(i, trailer.Length - i);
                    strEnd = strEnd.Replace("startxref","");
                    strEnd = strEnd.Replace("%%EOF", "");
                    strEnd = strEnd.Replace("\n", "");
                    StartXref = long.Parse(strEnd.Trim());

                    strHome = strHome.Replace("trailer", "");
                    strHome = strHome.Replace("<<", "");
                    strHome = strHome.Replace(">>", "");
                }
            }

            public byte[] getBytes(out int size)
            {
                //StringBuilder sb = new StringBuilder();
                //Dict.Select(x => sb.AppendFormat("{0} {1}\n", x.Key, x.Value));
                List<string> ls = Dict.Select(x => string.Format("{0} {1}\n", x.Key, x.Value)).ToList();
                string str = string.Format("trailer\n<<\n{0}>>\nstartxref\n{1}\n%%EOF\n", ls.ToString(), StartXref);
                byte[] buffer = Encoding.UTF8.GetBytes(str);
                size = buffer.Length;
                return buffer;
            }


        }//end class Trailer

        #endregion




        public static void Open(string FilePath)
        {
            try
            {


                //目录
                CatalogDict catalogDict = new CatalogDict();
                //页面树
                PageTreeDict pageTreeDict = new PageTreeDict();

                //字体
                FontDict TimesRoman = new FontDict();
                FontDict TimesItalic = new FontDict();
                FontDict TimesBold = new FontDict();
                FontDict Courier = new FontDict();

                //信息字典
                InfoDict infoDict = new InfoDict();

                //创建字体
                TimesRoman.CreateFontDict("T1", "Times-Roman");
                TimesItalic.CreateFontDict("T2", "Times-Italic");
                TimesBold.CreateFontDict("T3", "Times-Bold");
                Courier.CreateFontDict("T4", "Courier");
                //设置信息
                infoDict.SetInfo("Invoice xxx", "System Generated", "My Company Name");
                //通用控件
                Utility pdfUtility = new Utility();

                //打开文件

                FileStream file = new FileStream(FilePath, FileMode.Create);
                int size = 0;
                file.Write(pdfUtility.GetHeader("1.5", out size), 0, size);
                file.Close();

                //Finished the first step



                //页面字典
                PageDict page = new PageDict();
                ContentDict content = new ContentDict();

                //页面尺寸
                PageSize pSize = new PageSize(595, 842); //A4 paper portrait in 1/72" measurements
                pSize.SetMargins(10, 10, 10, 10);

                //创建页
                page.CreatePage(pageTreeDict.objectNum, pSize);

                //添加页
                pageTreeDict.AddPage(page.objectNum);

                //添加字体资源
                page.AddResource(TimesRoman, content.objectNum);
                page.AddResource(TimesItalic, content.objectNum);
                page.AddResource(TimesBold, content.objectNum);
                page.AddResource(Courier, content.objectNum);

                //表格
                TextAndTables textAndtable = new TextAndTables(pSize);

                //颜色
                ColorSpec rrBorder = new ColorSpec(0, 0, 0);        //main border colour
                ColorSpec rrMainBG = new ColorSpec(204, 204, 204);  //background colour of the round rectangle
                ColorSpec rrTBBG = new ColorSpec(255, 255, 255);    //background colour of the rectangle on top of the round rectangle


                ////添加图片
                //ImageDict I1 = new ImageDict();                    
                //I1.CreateImageDict("I1", ImagePath);               
                //page.AddImageResource(I1.PDFImageName, I1, content.objectNum);  

                ///*
                // * 图像添加到内容流
                // */
                //PageImages pi = new PageImages();
                //content.SetStream(pi.ShowImage("I1", 300, 700, 144, 100));   //tell the PDF we want to draw an image called 'I1', where and what size

                //content.SetStream(pi.ShowImage("I1", 100, 100, 288, 200));  //draw the same image at a different place twice the size.

                //创建圆角矩形
                RoundRectangle rr = new RoundRectangle();

                //打开图形游标
                content.SetStream("q/r/n");


                /*
                 * 发送图形到流
                 */

                content.SetStream(rr.DrawRoundRectangle(45, 582, 240, 130, 20, 0.55, 20, 90, 1, rrBorder, rrMainBG, rrTBBG));

                //直线
                StraightLine line = new StraightLine();             //new straight line object
                ColorSpec vline = new ColorSpec(255, 0, 0);     //line colour - in this case Red

                //发送线到流
                content.SetStream(line.DrawLine(200, 602, 200, 692, 0.5, vline));

                //关闭图形游标
                content.SetStream("Q/r/n");

                //添加文本头
                textAndtable.AddText(165, 143, "Round Rectangle Header", 12, "T3", Align.CenterAlign);


                //对齐
                Align[] align = new Align[3];
                align[0] = Align.LeftAlign;
                align[1] = Align.RightAlign;
                align[2] = Align.RightAlign;

                //颜色
                ColorSpec cellColor = new ColorSpec(60, 128, 255);  //Blue
                ColorSpec lineColor = new ColorSpec(0, 255, 0);     //Green

                //表格
                TableParams table = new TableParams(3, 200, 150, 100);
                table.xPos = 40;
                table.yPos = 548;
                table.rowHeight = 20;

                //添加3行
                textAndtable.SetParams(table, cellColor, Align.LeftAlign, 3);
                textAndtable.AddRow(false, 11, "T4", align, "Row 1, col 1", "Row1, col 2", "Row 1, col3");
                textAndtable.AddRow(false, 11, "T4", align, "Row 2, col 1", "Row2, col 2", "Row 2, col3");
                textAndtable.AddRow(false, 11, "T4", align, "Row 3, col 1", "Row3, col 2", "Row 3, col3");

                //

                /*
                 *添加表格到流
                 */
                content.SetStream(textAndtable.EndTable(lineColor, true));
                content.SetStream(textAndtable.EndText());

                //All done - send the information to the PDF file

                size = 0;
                file = new FileStream(FilePath, FileMode.Append);
                file.Write(page.GetPageDict(file.Length, out size), 0, size);
                file.Write(content.GetContentDict(file.Length, out size), 0, size);
                file.Close();

                file = new FileStream(FilePath, FileMode.Append);
                file.Write(catalogDict.GetCatalogDict(pageTreeDict.objectNum, file.Length, out size), 0, size);
                file.Write(pageTreeDict.GetPageTree(file.Length, out size), 0, size);
                file.Write(TimesRoman.GetFontDict(file.Length, out size), 0, size);
                file.Write(TimesItalic.GetFontDict(file.Length, out size), 0, size);
                file.Write(TimesBold.GetFontDict(file.Length, out size), 0, size);
                file.Write(Courier.GetFontDict(file.Length, out size), 0, size);

                ////write image dict
                //file.Write(I1.GetImageDict(file.Length, out size), 0, size);

                file.Write(infoDict.GetInfoDict(file.Length, out size), 0, size);
                file.Write(pdfUtility.CreateXrefTable(file.Length, out size), 0, size);
                file.Write(pdfUtility.GetTrailer(catalogDict.objectNum, infoDict.objectNum, out size), 0, size);
                file.Close();




                //show the newly created PDF on screen

                string sFile = FilePath;

                FileStream fs = new FileStream(sFile, FileMode.Open, FileAccess.Read);

                //check the file exists

                /*
                 * If you would like to pass some parameters, then you can check that user has
                 * permission to view this file before displaying it.
                 * 
                 * Eg streampdf.aspx?var=123 where the 123 maybe represents a particular file reference in a database.
                 * 
                 * For additional security, you can save the actual files in the App_Data folder which IIS will not serve to a HTTP request
                 */

                if (fs.CanRead)
                {
                    //TheDocument.Text = "<div><center><br><br><iframe src=/"streampdf.aspx / " width=/"850 / " height=/"900 / "></iframe></center></div>";
                    fs.Close();
                }
                else
                {
                    fs.Dispose();
                }
            }
            catch (Exception ex)
            {

            }
        }

  

        #region PDF对象

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


        /// <summary>
        /// PDF文档中对象字节偏移量数组
        /// </summary>
        internal class XrefEntries
        {
            internal static ArrayList offsetArray;

            internal XrefEntries()
            {
                offsetArray = new ArrayList();
            }
        }//end class XrefEntries

        /// <summary>
        /// 对象清单成员
        /// </summary>
        internal class ObjectList : IComparable
        {
            internal long offset;
            internal uint objNum;

            /// <summary>
            /// 创建一个对象
            /// </summary>
            /// <param name="objectNum">对象编号</param>
            /// <param name="fileOffset">文件偏移</param>
            internal ObjectList(uint objectNum, long fileOffset)
            {
                offset = fileOffset;
                objNum = objectNum;
            }
            #region IComparable Members

            /// <summary>
            /// 判断当前对象基于指定对象的位置
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int CompareTo(object obj)
            {

                int result = 0;
                result = (objNum.CompareTo(((ObjectList)obj).objNum));
                return result;
            }

            #endregion
        }//class ObjectList


        /// <summary>
        /// PDF对象
        /// </summary>
        public class PdfObject
        {
            /// <summary>
            /// 存储对象数
            /// </summary>
            internal static uint inUseObjectNum;
            /// <summary>
            /// 对象编号
            /// </summary>
            public uint objectNum;
            /// <summary>
            /// 对象偏移量数组 
            /// </summary>
            private XrefEntries Xref;

            /// <summary>
            /// 构造函数将对象编号递增到反映当前使用的对象编号
            /// </summary>
            protected PdfObject()
            {
                if (inUseObjectNum == 0)
                    Xref = new XrefEntries();
                inUseObjectNum++;
                objectNum = inUseObjectNum;
            }

            ~PdfObject()
            {
                objectNum = 0;
            }

            /// <summary>
            /// 获取UTF8编码
            /// </summary>
            /// <param name="str">字符串</param>
            /// <param name="filePos">文件偏移</param>
            /// <param name="size"></param>
            /// <returns></returns>
            protected byte[] GetUTF8Bytes(string str, long filePos, out int size)
            {
                ObjectList objList = new ObjectList(objectNum, filePos);
                byte[] abuf;
                try
                {
                    byte[] ubuf = Encoding.Unicode.GetBytes(str);
                    Encoding enc = Encoding.GetEncoding(1252);
                    abuf = Encoding.Convert(Encoding.Unicode, enc, ubuf);
                    size = abuf.Length;
                    XrefEntries.offsetArray.Add(objList);
                }
                catch (Exception e)
                {
                    string str1 = string.Format("{0},In PdfObjects.GetBytes()", objectNum);
                    Exception error = new Exception(e.Message + str1);
                    throw error;
                }
                return abuf;
            }

            /// <summary>
            /// 获取图像二进制数据
            /// </summary>
            /// <param name="startStr">开始位置</param>
            /// <param name="endStr">终止位置</param>
            /// <param name="ImageByteStream">图像流</param>
            /// <param name="filePos"></param>
            /// <param name="size">返回值长度</param>
            /// <returns></returns>
            protected byte[] GetImageBytes(string startStr, string endStr, byte[] ImageByteStream, long filePos, out int size)
            {
                ObjectList objList = new ObjectList(objectNum, filePos);
                byte[] s;
                byte[] e;
                try
                {
                    //将图像对象的开始和结束部分转换为字节
                    Encoding enc = Encoding.GetEncoding(1252);

                    s = Encoding.Unicode.GetBytes(startStr);
                    s = Encoding.Convert(Encoding.Unicode, enc, s);

                    e = Encoding.Unicode.GetBytes(endStr);
                    e = Encoding.Convert(Encoding.Unicode, enc, e);

                    XrefEntries.offsetArray.Add(objList);

                    //获取字节流的长度以创建用于连接的新字节
                    size = s.Length + ImageByteStream.Length + e.Length;
                }
                catch (Exception ex)
                {
                    string str1 = string.Format("{0},In PdfObjects.GetImageBytes()", objectNum);
                    Exception error = new Exception(ex.Message + str1);
                    throw error;
                }

                byte[] abuf = new byte[size];

                try
                {
                    int count = 0;
                    int i = 0;

                    //创建图像对象的字节，并包括“流”命令。
                    while (count < s.Length)
                    {
                        abuf[i] = s[count];
                        count++;
                        i++;
                    }

                    //添加表示实际图像数据的字节
                    count = 0;
                    while (count < ImageByteStream.Length)
                    {
                        abuf[i] = ImageByteStream[count];
                        count++;
                        i++;
                    }

                    //添加“终结流”和“终结对象”命令
                    count = 0;
                    while (count < e.Length)
                    {
                        abuf[i] = e[count];
                        count++;
                        i++;
                    }
                }
                catch (Exception ex1)
                {
                    string str2 = string.Format("{0},In PdfObjects.GetImageBytes()", objectNum);
                    Exception error = new Exception(ex1.Message + str2);
                    throw error;
                }

                return abuf;
            }
        }//end class PdfObject

        /// <summary>
        /// 目录字典
        /// </summary>
        public class CatalogDict : PdfObject
        {
            private string catalog;
            public CatalogDict()
            {

            }

            /// <summary>
            /// 获取目录字典
            /// </summary>
            /// <param name="refPageTree">页面树引用</param>
            /// <param name="filePos"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            public byte[] GetCatalogDict(uint refPageTree, long filePos, out int size)
            {
                Exception error = new Exception(" In CatalogDict.GetCatalogDict(), PageTree.objectNum Invalid");
                if (refPageTree < 1)
                {
                    throw error;
                }
                catalog = string.Format("{0} 0 obj<</Type /Catalog/Lang(EN-US)/Pages {1} 0 R>>/rendobj/r", this.objectNum, refPageTree);
                return this.GetUTF8Bytes(catalog, filePos, out size);
            }

        }//end class CatalogDict

        /// <summary>
        /// 页面树
        /// </summary>
        public class PageTreeDict : PdfObject
        {
            private string pageTree;
            private string kids;
            private static uint MaxPages;

            public PageTreeDict()
            {
                kids = "[ ";
                MaxPages = 0;
            }
            /// <summary>
            /// 添加页号
            /// </summary>
            /// <param name="objNum">页面编号</param>
            /// <param name="pageNum"></param>
            public void AddPage(uint objNum)
            {
                Exception error = new Exception("In PageTreeDict.AddPage, PageDict.ObjectNum Invalid");
                if (objNum < 0 || objNum > inUseObjectNum)
                    throw error;
                MaxPages++;
                string refPage = objNum + " 0 R ";
                kids = kids + refPage;
            }
            /// <summary>
            /// 获取页面树字典
            /// </summary>
            /// <returns></returns>
            public byte[] GetPageTree(long filePos, out int size)
            {
                pageTree = string.Format("{0} 0 obj<</Count {1}/Kids {2}]>>/rendobj/r", this.objectNum, MaxPages, kids);
                return this.GetUTF8Bytes(pageTree, filePos, out size);
            }
        }//end class PageTreeDict

        /// <summary>
        /// 页面字典
        /// </summary>
        public class PageDict : PdfObject
        {
            private string page;
            private string pageSize;
            private string fontRef;
            private string imageRef;
            private string resourceDict, contents;
            public PageDict()
            {
                resourceDict = null;
                contents = null;
                pageSize = null;
                fontRef = null;
                imageRef = null;
            }


            /// <summary>
            /// 新建页面
            /// </summary>
            /// <param name="refParent">页号</param>
            /// <param name="pSize">页面尺寸</param>
            public void CreatePage(uint refParent, PageSize pSize)
            {
                Exception error = new Exception("In PageDict.CreatePage(),PageTree.ObjectNum Invalid");
                if (refParent < 1 || refParent > PdfObject.inUseObjectNum)
                    throw error;

                pageSize = string.Format("[0 0 {0} {1}]", pSize.xWidth, pSize.yHeight);
                page = string.Format("{0} 0 obj<</Type /Page/Parent {1} 0 R/Rotate 0/MediaBox {2}/CropBox {2}/r/n/Resources<</ProcSet[/PDF/Text]/r/n", this.objectNum, refParent, pageSize);
            }


            /// <summary>
            ///  添加字体资源
            /// </summary>
            /// <param name="font">字体资源</param>
            /// <param name="contentRef">页编号</param>
            public void AddResource(FontDict font, uint contentRef)
            {
                fontRef += string.Format("/{0} {1} 0 R", font.font, font.objectNum);
                if (contentRef > 0)
                {
                    contents = string.Format("/Contents {0} 0 R", contentRef);
                }
            }

            /// <summary>
            /// 添加图像资源
            /// </summary>
            /// <param name="PDFImageName">图像名称</param>
            /// <param name="image">图像资源</param>
            /// <param name="contentRef">页编号</param>
            public void AddImageResource(string PDFImageName, ImageDict image, uint contentRef)
            {
                imageRef += string.Format("/{0} {1} 0 R ", PDFImageName, image.objectNum);
            }

            /// <summary>
            /// 返回资源字典
            /// </summary>
            /// <param name="filePos"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            public byte[] GetPageDict(long filePos, out int size)
            {
                resourceDict = string.Format("/Font<<{0}>>", fontRef);

                //添加对象引用
                if (imageRef != null)
                {
                    resourceDict += string.Format("/r/n/XObject <<{0}>>", imageRef.ToString());
                }

                resourceDict += ">>";

                page += resourceDict + contents + ">>/rendobj/r";
                return this.GetUTF8Bytes(page, filePos, out size);
            }
        }//end class PageDict

        /// <summary>
        /// 内容字典
        /// </summary>
        public class ContentDict : PdfObject
        {
            private string contentDictStart;
            private string contentDictEnd;
            private string contentStream;
            public ContentDict()
            {
                contentDictStart = null;
                contentDictEnd = null;
                //contentStream = "%stream/r";
                contentStream = "/r";
            }
            /// <summary>
            /// 设置流
            /// </summary>
            /// <param name="stream">流</param>

            public void SetStream(string stream)
            {
                contentStream += stream;
            }

            /// <summary>
            /// 获取内容字典
            /// </summary>
            /// <param name="filePos"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            public byte[] GetContentDict(long filePos, out int size)
            {
                /*
                    以后可以添加Flad压缩在这里工作，因此启动和结束加上Cordon流变量。
                 */

                contentDictStart = string.Format("{0} 0 obj<</Length {1}/r/n>>stream/r/n", this.objectNum, contentStream.Length);
                contentDictEnd = string.Format("/r/n/nendstream/rendobj/r");

                return GetUTF8Bytes(contentDictStart + contentStream + contentDictEnd, filePos, out size);

            }

        }//end class ContentDict


        /// <summary>
        /// PDF字体字典
        /// 存放不需要嵌入的字体
        /// Times-Roman         Helvetica               Courier                 Symbol
        /// Times-Bold          Helvetica-Bold          Courier-Bold            ZapfDingbats
        /// Times-Italic        Helvetica-Oblique       Courier-Oblique
        /// Times-BoldItalic    Helvetica-BoldOblique   Courier-BoldOblique
        /// </summary>
        public class FontDict : PdfObject
        {
            private string fontDict;
            public string font;
            public FontDict()
            {
                font = null;
                fontDict = null;
            }
            /// <summary>
            /// 创建字体字典
            /// </summary>
            /// <param name="fontName"></param>
            public void CreateFontDict(string fontName, string BaseFont)
            {
                font = fontName;
                fontDict = string.Format("{0} 0 obj<</Type/Font/Name /{1}/BaseFont/{2}/Subtype/Type1/Encoding /WinAnsiEncoding>>/nendobj/n", this.objectNum, fontName, BaseFont);
            }


            /// <summary>
            /// 获取字体字典
            /// </summary>
            /// <param name="filePos"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            public byte[] GetFontDict(long filePos, out int size)
            {
                return this.GetUTF8Bytes(fontDict, filePos, out size);
            }

        }//end class FontDict

        /// <summary>
        /// 图像字典
        /// </summary>
        public class ImageDict : PdfObject
        {
            private string imageDictStart;
            private string imageDictEnd;
            private byte[] imagebytes;

            public string PDFImageName;
            public ImageDict()
            {
                PDFImageName = null;
                imagebytes = null;
                imageDictEnd = null;
                imageDictStart = null;
            }

            /// <summary>
            /// 创建图像字典
            /// </summary>
            /// <param name="ImageName">图像名称</param>
            /// <param name="ImagePath">图像路径</param>s
            public void CreateImageDict(string ImageName, string ImagePath)
            {
                int fileLength;
                int Iwidth;
                int Iheight;

                FileInfo fileInfo = null;
                using (FileStream fs = File.OpenRead(ImagePath))
                {
                    try
                    {
                        fileInfo = new FileInfo(ImagePath);
                        imagebytes = new byte[fileInfo.Length];

                        Bitmap bmp = new Bitmap(ImagePath);

                        Iwidth = bmp.Width;
                        Iheight = bmp.Height;

                        bmp.Dispose();

                        if (imagebytes.Length != fs.Read(imagebytes, 0, imagebytes.Length))
                        {
                            throw new Exception(string.Format("读取图像文件时出错：'{0}'", ImagePath));
                        }
                    }
                    catch
                    {
                        throw new Exception(string.Format("打开/读取图像时出错：'{0}'", ImagePath));
                    }
                }

                fileLength = imagebytes.Length;
                PDFImageName = ImageName;
                imageDictStart = string.Format("{0} 0 obj<</Name /{1}/r/n /Type /XObject/r/n /Subtype /Image/r/n /Width {2}/r/n /Height {3}/r/n /Length {4}/r/n /Filter /DCTDecode/r/n /ColorSpace /DeviceRGB/r/n /BitsPerComponent 8/r/n>>/r/nstream/r/n", this.objectNum, ImageName, Iwidth.ToString(), Iheight.ToString(), fileLength.ToString());
                imageDictEnd = "/r/nendstream/r/nendobj/r/n";
            }


            /// <summary>
            /// 获取图像字典
            /// </summary>
            /// <param name="filePos"></param>
            /// <param name="size"></param>
            /// <returns></returns>
            public byte[] GetImageDict(long filePos, out int size)
            {
                return this.GetImageBytes(imageDictStart, imageDictEnd, imagebytes, filePos, out size);
            }

        }//end ImageDict



        /// <summary>
        ///PDF信息字典
        /// </summary>
        public class InfoDict : PdfObject
        {
            private string info;
            public InfoDict()
            {
                info = null;
            }

            /// <summary>
            /// 填充PDF信息
            /// </summary>
            /// <param name="title">标题</param>
            /// <param name="author">作者</param>
            /// <param name="company">公司</param>
            public void SetInfo(string title, string author, string company)
            {
                //创造者内容
                info = string.Format("{0} 0 obj<</ModDate({1})/CreationDate({1})/Title({2})/Creator(应用名称)/Author({3})/Producer(www.AAA.com.au)/Company({4})>>/rendobj/r", this.objectNum, GetDateTime(), title, author, company);
            }

            /// <summary>
            /// 获取文档信息字典
            /// </summary>
            /// <param name="filePos">文件偏移</param>
            /// <param name="size">返回值长度</param>
            /// <returns></returns>
            public byte[] GetInfoDict(long filePos, out int size)
            {
                return GetUTF8Bytes(info, filePos, out size);
            }

            /// <summary>
            /// 获取类似 ISO/IEC 8824 格式的时间
            /// </summary>
            /// <returns></returns>
            private string GetDateTime()
            {
                DateTime universalDate = DateTime.UtcNow;
                DateTime localDate = DateTime.Now;
                string pdfDate = string.Format("D:{0:yyyyMMddhhmmss}", localDate);
                TimeSpan diff = localDate.Subtract(universalDate);
                int uHour = diff.Hours;
                int uMinute = diff.Minutes;
                char sign = '+';
                if (uHour < 0)
                    sign = '-';
                uHour = Math.Abs(uHour);
                pdfDate += string.Format("{0}{1}'{2}'", sign, uHour.ToString().PadLeft(2, '0'), uMinute.ToString().PadLeft(2, '0'));
                return pdfDate;
            }
        }//end class InfoDict


        /// <summary>
        /// 该类包含创建PDF的通用工具
        /// Header
        /// XrefTable
        /// Trailer
        /// </summary>
        public class Utility
        {
            private uint numTableEntries;
            private string table;
            private string infoDict;
            public Utility()
            {
                numTableEntries = 0;
                table = null;
                infoDict = null;
            }
            /// <summary>
            /// 创建参照表
            /// </summary>
            /// <param name="fileOffset">文件偏移</param>
            /// <param name="size">返回值长度</param>
            /// <returns></returns>
            public byte[] CreateXrefTable(long fileOffset, out int size)
            {
                //存储参照表的偏移量
                try
                {
                    ObjectList objList = new ObjectList(0, fileOffset);
                    XrefEntries.offsetArray.Add(objList);
                    XrefEntries.offsetArray.Sort();
                    numTableEntries = (uint)XrefEntries.offsetArray.Count;
                    table = string.Format("xref/r/n{0} {1}/r/n0000000000 65535 f/r/n", 0, numTableEntries);
                    for (int entries = 1; entries < numTableEntries; entries++)
                    {
                        ObjectList obj = (ObjectList)XrefEntries.offsetArray[entries];
                        table += obj.offset.ToString().PadLeft(10, '0');
                        table += " 00000 n/r/n";
                    }
                }
                catch (Exception e)
                {
                    Exception error = new Exception(e.Message + " In Utility.CreateXrefTable()");
                    throw error;
                }
                return GetUTF8Bytes(table, out size);
            }

            /// <summary>
            /// 返回头
            /// </summary>
            /// <param name="version">版本</param>
            /// <param name="size">返回值长度</param>
            /// <returns></returns>
            public byte[] GetHeader(string version, out int size)
            {
                string header = string.Format("%PDF-{0}/r%{1}/r/n", version, "/x82/x82");
                return GetUTF8Bytes(header, out size);
            }

            /// <summary>
            /// 创建trailer并返回位数组
            /// </summary>
            /// <param name="refRoot">引用根</param>
            /// <param name="refInfo">引用信息</param>
            /// <param name="size">返回位长度</param>
            /// <returns></returns>
            public byte[] GetTrailer(uint refRoot, uint refInfo, out int size)
            {
                string trailer = null;
                try
                {
                    if (refInfo > 0)
                    {
                        infoDict = string.Format("/Info {0} 0 R", refInfo);
                    }
                    //已排序的数组将被排在文件的0偏移位置
                    ObjectList objList = (ObjectList)XrefEntries.offsetArray[0];
                    trailer = string.Format("trailer/n<</Size {0}/Root {1} 0 R {2}/ID[<5181383ede94727bcb32ac27ded71c68>" +
                        "<5181383ede94727bcb32ac27ded71c68>]>>/r/nstartxref/r/n{3}/r/n%%EOF/r/n"
                        , numTableEntries, refRoot, infoDict, objList.offset);

                    XrefEntries.offsetArray = null;
                    PdfObject.inUseObjectNum = 0;
                }
                catch (Exception e)
                {
                    Exception error = new Exception(e.Message + " In Utility.GetTrailer()");
                    throw error;
                }

                return GetUTF8Bytes(trailer, out size);
            }

            /// <summary>
            /// 将字符串转为UTF8编码
            /// </summary>
            /// <param name="str">字符串</param>
            /// <param name="size">返回的位长度</param>
            /// <returns></returns>
            private byte[] GetUTF8Bytes(string str, out int size)
            {
                try
                {
                    byte[] ubuf = Encoding.Unicode.GetBytes(str);
                    Encoding enc = Encoding.GetEncoding(1252);
                    byte[] abuf = Encoding.Convert(Encoding.Unicode, enc, ubuf);
                    size = abuf.Length;
                    return abuf;
                }
                catch (Exception e)
                {
                    Exception error = new Exception(e.Message + " In Utility.GetUTF8Bytes()");
                    throw error;
                }
            }
        }//end class Utility


        /********************************************************************************************************************
         * 
         * 
         *                                          表绘制例程
         * 
         * 
         *******************************************************************************************************************/

        /// <summary>
        /// 表格和文本
        /// </summary>
        public class TextAndTables
        {
            private uint fixedTop, lastY;
            private uint tableX;
            private PageSize pSize;
            private ArrayList rowY;
            private uint cPadding;
            private string errMsg;
            private uint numColumn, rowHeight, numRow;
            private uint[] colWidth;
            private uint textX, textY;
            private string tableStream;
            private uint tableWidth;
            private ColorSpec cColor;
            private string textStream;

            /*
			 * 指定字体中的每个字符的相对宽度，在这种情况下，Road是罗马字体。
             * 此信息用于计算页面上文本的实际位置。
             * 可以通过在本地计算机上搜索*.AFM文件或访问AdObjo找到其他字体的详细信息。
             * 寻找相同的。实际文件（Adobe字体管理器= AFM）包含您需要添加的所有细节。
             * 另一种字体。请注意，除了本地PDF字体以外的任何其他内容都必须被嵌入。
            */
            private uint[] TimesRomanWidth = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,250,333,408,500,
            500,833,778,333,333,333,500,564,250,333,250,278,500,500,500,500,500,500,500,500,500,500,278,278,564,564,564,444,
            921,722,667,667,722,611,556,722,722,333,389,722,611,889,722,722,556,722,667,556,611,722,722,944,722,722,611,333,
            278,333,469,500,333,444,500,444,500,444,333,500,500,278,278,500,278,778,500,500,500,500,333,389,278,500,500,722,
            500,500,444,480,200,480,541,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,333,500,500,167,500,500,
            500,500,180,444,500,333,333,556,556,500,500,500,250,453,350,333,444,444,500,100,100,444,333,333,333,333,333,333,
            333,333,333,333,333,333,333,100,889,276,611,722,889,310,667,278,278,500,722,500};

            public TextAndTables(PageSize pageSize)
            {
                fixedTop = lastY = 0;
                pSize = pageSize;
                tableX = 0;
                textStream = tableStream = errMsg = null;
                rowHeight = 0; numRow = 0;
                textX = 0; textY = 0;
            }

            /// <summary>
            /// 设置表参数
            /// </summary>
            /// <param name="table">表参数</param>
            /// <param name="cellColor">单元颜色</param>
            /// <param name="alignment">对齐方式</param>
            /// <param name="cellPadding">单元填充</param>
            /// <returns></returns>
            public bool SetParams(TableParams table, ColorSpec cellColor, Align alignment, uint cellPadding)
            {
                if ((table.yPos > (pSize.yHeight - pSize.topMargin)) || (tableWidth > (pSize.xWidth - (pSize.leftMargin + pSize.rightMargin))))
                    return false;
                tableWidth = table.tableWidth;
                switch (alignment)
                {

                    case (Align.LeftAlign):
                        tableX = pSize.leftMargin + table.xPos;
                        break;
                    case (Align.CenterAlign):
                        tableX = (pSize.xWidth - (pSize.leftMargin + pSize.rightMargin) - tableWidth) / 2;
                        break;
                    case (Align.RightAlign):
                        tableX = pSize.xWidth - (pSize.rightMargin + tableWidth);
                        break;
                }

                textX = tableX;
                textY = table.yPos;
                fixedTop = table.yPos;
                rowHeight = table.rowHeight;
                numColumn = table.numColumn;
                cColor = cellColor;
                cPadding = cellPadding;
                colWidth = new uint[numColumn];
                colWidth = table.columnWidths;
                rowY = new ArrayList();
                return true;

            }


            /// <summary>
            /// 获取行集合
            /// </summary>
            /// <param name="rowText">行</param>
            /// <param name="fontSize">字体大小</param>
            /// <returns></returns>
            private string[][] GetLinesInCell(string[] rowText, uint fontSize)
            {
                string[][] text = new string[numColumn][];
                char[] s = "/x0020".ToArray();
                for (int i = 0; i < rowText.Length; i++)
                {
                    uint width = (colWidth[i] - 2 * cPadding) * 1000 / fontSize;
                    string cellText = rowText[i];//.TrimStart(s);
                                                 //空条目跳出
                    if (cellText == null)
                    {
                        break;
                    }

                    ArrayList lineText = new ArrayList();
                    int index = 0;
                    uint cWidth = 0;
                    int words = 0;
                    for (int chars = 0; chars <= cellText.Length; chars++)
                    {
                        char[] cArray = cellText.ToCharArray();
                        do //assume TimesRoman - 需要定制每个字符的其他字体宽度
                        {
                            cWidth += TimesRomanWidth[cArray[words]];
                            words++;
                        }
                        while (cWidth < width && words < cArray.Length);

                        if (words == cArray.Length)
                        {
                            string line = cellText.Substring(0, words);
                            line = line.TrimEnd(s);
                            lineText.Add(line);
                            break;
                        }
                        else
                        {
                            words--;
                            int space = cellText.LastIndexOf("/x0020", words, words + 1);
                            if (space > 0)
                            {
                                string line = cellText.Substring(0, space + 1);
                                //删除词的尾随空格
                                line = line.TrimEnd(s);
                                lineText.Add(line);
                                index = space + 1;
                                words = 0;
                            }
                            else
                            {
                                string line = cellText.Substring(0, words);
                                lineText.Add(line);
                                index = words;
                                words = 0;
                            }
                        }
                        cWidth = 0;
                        chars = 0;
                        cellText = cellText.Substring(index);
                    }
                    text[i] = new string[lineText.Count];
                    //将行复制到数组之内
                    for (int j = 0; j < lineText.Count; j++)
                        text[i][j] = (string)lineText[j];

                }
                return text;
            }

            /// <summary>
            /// 获取字符串的宽度
            /// </summary>
            /// <param name="text">文本</param>
            /// <param name="fontSize">字体大小</param>
            /// <param name="FontName">字体名称</param>
            /// <returns></returns>
            private uint StrLen(string text, uint fontSize, string FontName)
            {
                char[] cArray = text.ToCharArray();
                uint cWidth = 0;
                foreach (char c in cArray)
                {
                    /*
                     * 在该实例中 fontname 'T4'是信使, 字体的其他实例都假定为 Times Roman.
                     * 所有信使相对宽为600.
                     * 更好的实现该方法需要从特定表中去查找相应字体的宽度。
                     * 每个字体宽度可以通过查找 a *.afm 和手工创建指定宽的数组
                     */

                    if (FontName == "T4")
                    {
                        cWidth += 600;
                    }
                    else
                    {
                        cWidth += TimesRomanWidth[c];
                    }
                }
                return (cWidth * fontSize / 1000);
            }

            /// <summary>
            /// 添加行
            /// </summary>
            /// <param name="textWrap">是否包装文本</param>
            /// <param name="fontSize">字体大小</param>
            /// <param name="fontName">字体名称</param>
            /// <param name="alignment">对齐</param>
            /// <param name="rowText">行</param>
            /// <returns></returns>
            public bool AddRow(bool textWrap, uint fontSize, string fontName, Align[] alignment, params string[] rowText)
            {
                if (rowText.Length > numColumn)
                {
                    return false;
                }

                uint maxLines = 1;
                //对齐表的x坐标
                uint startX = tableX;
                uint y = textY;
                uint x = 0;
                const uint yCellMargin = 4;
                //if(rowText.Length<numColumn || alignment.Length<numColumn)
                //    return false;
                //在添加行时增加行标记
                numRow++;
                //需要包装文本
                if (textWrap)
                {
                    string[][] text = GetLinesInCell(rowText, fontSize);

                    //列循环
                    for (int column = 0; column < rowText.Length; column++)
                    {
                        startX += colWidth[column];
                        uint lines;
                        //空条目跳出
                        if (text[column] == null)
                            break;
                        //行循环
                        for (lines = 0; lines < (uint)text[column].Length; lines++)
                        {
                            y = (uint)(textY - ((lines + 1) * rowHeight)) + yCellMargin;
                            try
                            {
                                switch (alignment[column])
                                {
                                    case (Align.LeftAlign):
                                        x = startX - colWidth[column] + cPadding;
                                        break;
                                    case (Align.CenterAlign):
                                        x = startX - (colWidth[column] + StrLen(text[column][lines], fontSize, fontName)) / 2;
                                        break;
                                    case (Align.RightAlign):
                                        x = startX - StrLen(text[column][lines], fontSize, fontName) - cPadding;
                                        break;
                                };
                            }
                            catch (Exception E)
                            {
                                errMsg = "String too long to fit in the Column" + E.Message;
                                Exception e = new Exception(errMsg);
                                throw e;
                            }

                            //'(', '/' and ')' 是adobe的转义字符 ，删除. '@' 意味着忽略 C# 转义字符

                            text[column][lines] = text[column][lines].Replace("//", "////");
                            text[column][lines] = text[column][lines].Replace("(", "//(");
                            text[column][lines] = text[column][lines].Replace(")", "//)");

                            tableStream += string.Format("/rBT/{0} {1} Tf /r{2} {3} Td /r({4}) Tj/rET", fontName, fontSize, x, y, text[column][lines]);
                        }
                        //计算行中最大行号
                        if (lines > maxLines)
                            maxLines = lines;
                    }
                    //改变Y坐标跳转的下一页
                    if (textY < pSize.bottomMargin)
                    {
                        textY = 0;
                        return false;
                    }

                    else    //改变Y坐标跳转到下一行
                    {
                        textY = textY - rowHeight * maxLines;
                        rowY.Add(textY);
                        rowY.Add(rowHeight * maxLines);
                    }
                }
                //不需要包装文本
                else
                {
                    for (int column = 0; column < rowText.Length; column++)
                    {
                        startX += colWidth[column];
                        y = (uint)(textY - rowHeight) + yCellMargin;
                        try
                        {
                            switch (alignment[column])
                            {
                                case (Align.LeftAlign):
                                    x = startX - colWidth[column] + cPadding;
                                    break;
                                case (Align.CenterAlign):
                                    x = (startX - (colWidth[column] + StrLen(rowText[column], fontSize, fontName)) / 2);
                                    break;
                                case (Align.RightAlign):
                                    x = startX - StrLen(rowText[column], fontSize, fontName) - cPadding;
                                    break;
                            }
                        }
                        catch (Exception E)
                        {
                            errMsg = "String too long to fit in the Column" + E.Message;
                            Exception error = new Exception(errMsg);
                            throw error;
                        }

                        //rowText[column] = rowText[column].Replace("here", "wibble");
                        rowText[column] = rowText[column].Replace("//", "////");
                        rowText[column] = rowText[column].Replace("(", "//(");
                        rowText[column] = rowText[column].Replace(")", "//)");
                        tableStream += string.Format("/rBT/{0} {1} Tf /r{2} {3} Td /r({4}) Tj/rET", fontName, fontSize, x, y, rowText[column]);
                    }
                    if (textY < pSize.bottomMargin)
                    {
                        textY = 0;
                        return false;
                    }
                    //改变Y坐标跳转到下一行
                    else
                    {
                        textY = textY - rowHeight;
                        rowY.Add(textY);
                        rowY.Add(rowHeight);
                    }
                }
                return true;
            }

            /// <summary>
            /// 添加文本
            /// </summary>
            /// <param name="X">起点</param>
            /// <param name="Y">起点</param>
            /// <param name="text">文本</param>
            /// <param name="fontSize">字体大小</param>
            /// <param name="fontName">字体名称</param>
            /// <param name="alignment">对齐方式</param>
            public void AddText(uint X, uint Y, string text, uint fontSize, string fontName, Align alignment)
            {
                Exception invalidPoints = new Exception("The X Y coordinate out of Page Boundary");
                if (X > pSize.xWidth || Y > pSize.yHeight)
                    throw invalidPoints;
                uint startX = 0;
                switch (alignment)
                {
                    case (Align.LeftAlign):
                        startX = X;
                        break;
                    case (Align.CenterAlign):
                        startX = X - (StrLen(text, fontSize, fontName)) / 2;
                        break;
                    case (Align.RightAlign):
                        startX = X - StrLen(text, fontSize, fontName);
                        break;
                };
                text = text.Replace("//", "////");
                text = text.Replace("(", "//(");
                text = text.Replace(")", "//)");
                textStream += string.Format("/rBT/{0} {1} Tf/r{2} {3} Td /r({4}) Tj/rET/r", fontName, fontSize, startX, (pSize.yHeight - Y), text);
            }

            /// <summary>
            /// 结束文本
            /// </summary>
            /// <returns></returns>
            public string EndText()
            {
                Exception noTextStream = new Exception("未创建文本元素！");
                if (textStream == null)
                    throw noTextStream;
                string stream = textStream;
                textStream = null;
                return stream;
            }


            /// <summary>
            /// 结束表，获取数据流
            /// </summary>
            /// <param name="lineColor">线条颜色</param>
            /// <param name="Showlines">是否绘制线条</param>
            /// <returns></returns>
            public string EndTable(ColorSpec lineColor, bool Showlines)
            {
                string tableCode;
                string rect = null;
                uint x = tableX;
                uint yBottom = 0;
                //如果需要增加行数
                if (rowY.Count < numRow * 2)
                    return null;

                //画行线
                if (Showlines)
                {
                    for (int row = 0, yCor = 0; row < numRow; row++, yCor += 2)
                    {
                        rect += string.Format("{0} {1} {2} {3} re/r", x, rowY[yCor], tableWidth, rowY[yCor + 1]);
                    }
                }
                //获取最后一行的y坐标
                if (rowY.Count < 1)
                    return null;
                yBottom = (uint)rowY[rowY.Count - 2];
                string line = null;

                //画列线
                if (Showlines)
                {
                    for (uint column = 0; column < numColumn; column++)
                    {
                        x += colWidth[column];
                        line += string.Format("{0} {1} m/r{0} {2} l/r", x, fixedTop, yBottom);
                    }
                }
                //为表创建代码
                tableCode = string.Format("/rq/r{5} {6} {7} RG {2} {3} {4} rg/r{0}/r{1}B/rQ/r", line, rect, cColor.red, cColor.green, cColor.blue, lineColor.red, lineColor.green, lineColor.blue);

                lastY = yBottom;
                tableCode += tableStream;

                //变量重置
                tableStream = null;
                numRow = 0;
                rowY = null;
                return tableCode;
            }

            /// <summary>
            /// 获取追加下一个表的坐标
            /// </summary>
            /// <returns></returns>
            public uint GetY()
            {
                return lastY;
            }
        }//class TextAndTables

        public class RoundRectangle
        {
            /// <summary>
            /// 绘制矩形
            /// </summary>
            /// <param name="LLX">左下方Y</param>
            /// <param name="LLY">左下方Y</param>
            /// <param name="rrWidth">宽度</param>
            /// <param name="rrHeight">高度</param>
            /// <param name="CornerRadius">圆角</param>
            /// <param name="Circularity">圆度</param>
            /// <param name="HeaderHeight">头高</param>
            /// <param name="TextBoxHeight">文本框高</param>
            /// <param name="Border">边框线宽</param>
            /// <param name="BorderColor">边框颜色</param>
            /// <param name="MainBG">主体颜色</param>
            /// <param name="TextBoxBG">文本框颜色</param>
            /// <returns></returns>
            public String DrawRoundRectangle(
                int LLX, int LLY, int rrWidth, int rrHeight, int CornerRadius,
                double Circularity, int HeaderHeight, int TextBoxHeight, int Border, ColorSpec BorderColor, ColorSpec MainBG, ColorSpec TextBoxBG)
            {

                if (CornerRadius > rrHeight / 2)
                {
                    Exception WrongCornerRadius = new Exception("圆角高度不能超出矩形的一半");
                    throw WrongCornerRadius;
                }

                if (HeaderHeight < CornerRadius)
                {
                    Exception WrongHeaderHeight = new Exception("页眉高度至少和圆角相同.");
                    throw WrongHeaderHeight;
                }

                if (TextBoxHeight > (rrHeight + (2 * CornerRadius)))
                {
                    Exception WrongTextBoxHeight = new Exception("文本框的高度不能超过矩形的高度.");
                    throw WrongTextBoxHeight;
                }

                //Circularity must be > zero and <= 1 - otherwise some funky stuff can happen....
                //    if(Circularity < 0 || Circularity > 1)
                //    {
                //        Exception WrongCircularity = new Exception("The Circularity must be between zero and one.");
                //        throw WrongCircularity;
                //    }

                //border thickness must be >= 0
                if (Border < 0)
                {
                    Exception WrongBorder = new Exception("边框宽度最少为1.");
                    throw WrongBorder;
                }

                //可以检查其他页面

                //在编写代码时忽略高度为0的文本框


                //开始编码

                /*参考：PDF198页引用，贝赛尔曲线信息.*/

                String rrCode = "";
                rrCode = string.Format("B/r/n{0} {1} {2} RG/r/n{3} {4} {5} rg/r/n{6} w/r/n", BorderColor.red, BorderColor.green, BorderColor.blue, MainBG.red, MainBG.green, MainBG.blue, Border);

                int cpx;    //current X
                int cpy;    //current Y
                int x1;     //应用在贝赛尔曲线中
                int y1;
                int x2;     //etc, etc
                int y2;
                int x3;     //final position X
                int y3;     //final position Y

                /*
                 * 移动曲线左下角
                 * 
                 */

                cpx = LLX;
                cpy = LLY + CornerRadius;
                rrCode += string.Format("{0} {1} m/r/n", cpx.ToString(), cpy.ToString());

                cpy = LLY + rrHeight - CornerRadius;
                rrCode += string.Format("{0} {1} l/r/n", cpx, cpy);

                x1 = cpx;
                y1 = cpy + (int)(Circularity * CornerRadius);
                x2 = cpx + CornerRadius - (int)(Circularity * CornerRadius);
                y2 = cpy + CornerRadius;
                x3 = cpx + CornerRadius;
                y3 = cpy + CornerRadius;
                rrCode += string.Format("{0} {1} {2} {3} {4} {5} c/r/n", x1.ToString(), y1.ToString(), x2.ToString(), y2.ToString(), x3.ToString(), y3.ToString());

                cpx = LLX + rrWidth - CornerRadius;
                cpy = LLY + rrHeight;
                rrCode += string.Format("{0} {1} l/r/n", cpx.ToString(), cpy.ToString());

                x1 = LLX + rrWidth - CornerRadius + (int)(CornerRadius * Circularity);
                y1 = LLY + rrHeight;
                x2 = LLX + rrWidth;
                y2 = LLY + rrHeight - CornerRadius + (int)(CornerRadius * Circularity);
                x3 = LLX + rrWidth;
                y3 = LLY + rrHeight - CornerRadius;
                rrCode += string.Format("{0} {1} {2} {3} {4} {5} c/r/n", x1.ToString(), y1.ToString(), x2.ToString(), y2.ToString(), x3.ToString(), y3.ToString());

                cpx = x3;
                cpy = LLY + CornerRadius;
                rrCode += string.Format("{0} {1} l/r/n", cpx.ToString(), cpy.ToString());

                x1 = x3;
                y1 = LLY + CornerRadius - (int)(CornerRadius * Circularity);
                x2 = LLX + rrWidth - CornerRadius + (int)(CornerRadius * Circularity);
                y2 = LLY;
                x3 = LLX + rrWidth - CornerRadius;
                y3 = LLY;
                rrCode += string.Format("{0} {1} {2} {3} {4} {5} c/r/n", x1.ToString(), y1.ToString(), x2.ToString(), y2.ToString(), x3.ToString(), y3.ToString());

                cpx = LLX + CornerRadius;
                cpy = LLY;
                rrCode += string.Format("{0} {1} l/r/n", cpx.ToString(), cpy.ToString());

                x1 = LLX + CornerRadius - (int)(CornerRadius * Circularity);
                y1 = LLY;
                x2 = LLX;
                y2 = LLY + CornerRadius - (int)(CornerRadius * Circularity);
                x3 = LLX;
                y3 = LLY + CornerRadius;
                rrCode += string.Format("{0} {1} {2} {3} {4} {5} c/r/n", x1.ToString(), y1.ToString(), x2.ToString(), y2.ToString(), x3.ToString(), y3.ToString());

                rrCode += "b/r/n";


                if (TextBoxHeight != 0)
                {
                    //画矩形

                    rrCode += string.Format("B/r/n{0} w/r/n{1} {2} {3} rg/r/n", Border.ToString(), TextBoxBG.red, TextBoxBG.green, TextBoxBG.blue);

                    x1 = LLX;
                    y1 = LLY + HeaderHeight;
                    x2 = rrWidth;
                    y2 = TextBoxHeight;

                    rrCode += string.Format("{0} {1} {2} {3} re/r/n", x1.ToString(), y1.ToString(), x2.ToString(), y2.ToString());

                    rrCode += "b/r/n";
                }

                return rrCode;
            }
        }//end class RoundRectangle

        /// <summary>
        /// 直线
        /// </summary>
        public class StraightLine
        {
            /// <summary>
            /// 绘制直线
            /// </summary>
            /// <param name="startX">起点X坐标</param>
            /// <param name="startY">起点Y坐标</param>
            /// <param name="endX">终点X坐标</param>
            /// <param name="endY">终点Y坐标</param>
            /// <param name="lineWidth">线宽</param>
            /// <param name="lineColor">线颜色</param>
            /// <returns></returns>
            public string DrawLine(int startX, int startY, int endX, int endY, Double lineWidth, ColorSpec lineColor)
            {
                //应该检查线是否在页上

                string lineCode = "";
                lineCode += string.Format("{0} {1} {2} RG {3} w {4} {5} m {6} {7} l S/r/n", lineColor.red, lineColor.green, lineColor.blue, lineWidth.ToString(), startX.ToString(), startY.ToString(), endX.ToString(), endY.ToString());
                return lineCode;
            }

        }//end class StraightLine

        /// <summary>
        /// 图像
        /// </summary>
        public class PageImages
        {
            /// <summary>
            /// 显示图像命令行
            /// </summary>
            /// <param name="ImageName">图像名称</param>
            /// <param name="LLX">左下点</param>
            /// <param name="LLY">左下点</param>
            /// <param name="dspWidth">宽度</param>
            /// <param name="dspHeight">高度</param>
            /// <returns></returns>
            public string ShowImage(string ImageName, int LLX, int LLY, int dspWidth, int dspHeight)
            {
                //应该检查图像是否在页上

                string ICode = "";
                ICode += string.Format("q/r/n{0} 0 0 {1} {2} {3} cm/r/n", dspWidth.ToString(), dspHeight.ToString(), LLX.ToString(), LLY.ToString());
                ICode += string.Format("1 0 0 1 0 0 cm/r/n/{0} Do/r/nQ/r/n", ImageName);

                return ICode;
            }
            #endregion
        }//end class PageImages
    }//end class UsePDF
}
