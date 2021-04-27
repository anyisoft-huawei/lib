using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.office
{
    public static class NPOIHelper
    {

        /// <summary>
        /// 转为excel格式数据
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="title">sheet标题</param>
        /// <returns></returns>
        public static byte[] ToExcel(this DataTable dt, string title)
        {
            var book = new HSSFWorkbook();
            var sheet = book.CreateSheet(title);
            //列头
            var row = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                row.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
            }
            //数据
            for (int ri = 0; ri < dt.Rows.Count; ri++)
            {
                var dr = dt.Rows[ri];
                row = sheet.CreateRow(ri + 1);
                for (int b = 0; b < dt.Columns.Count; b++)
                {
                    row.CreateCell(b).SetCellValue(dr[b] == DBNull.Value ? string.Empty : dr[b].ToString());
                }
            }
            using (MemoryStream ms = new MemoryStream())
            {
                book.Write(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Excel文件导成Datatable
        /// </summary>
        /// <param name="ms">Excel文件流</param>
        /// <param name="sheetname">Excel表名</param>
        /// <returns></returns>
        public static DataTable EecelToDataTable(Stream ms, string sheetname)
        {
            DataTable dt = new DataTable();
            HSSFWorkbook workbook = new HSSFWorkbook(ms);
            var sheet = workbook.GetSheet(sheetname);
            if (null == sheet) return null;
            //添加列
            var row = sheet.GetRow(0);
            var val = "";
            for (int i = 0; i < row.LastCellNum; i++)
            {
                val = row.GetCell(i).StringCellValue;
                if (string.IsNullOrEmpty(val) || dt.Columns.Contains(val)) dt.Columns.Add(string.Format("第{0}列{1}", i + 1, val)); else dt.Columns.Add(val);
            }
            //数据
            for (int ri = 1; ri < sheet.LastRowNum; ri++)
            {
                var dr = dt.NewRow();
                row = sheet.GetRow(ri);
                for (int i = 0; i < dt.Columns.Count && i < row.LastCellNum; i++)
                {
                    dr[i] = row.GetCell(i).StringCellValue;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 从Excel导入数据
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="ms">excel流</param>
        /// <param name="sheetname">Excel表名</param>
        /// <param name="namerow">列名所在行</param>
        /// <param name="mapping">列映射</param>
        public static void FromExcle(this DataTable dt, Stream ms, string sheetname, int namerow, List<KeyValuePair<string, string>> mapping = null)
        {
            HSSFWorkbook workbook = new HSSFWorkbook(ms);
            var sheet = workbook.GetSheet(sheetname);
            if (null == sheet) return;
            //获取excel列
            var row = sheet.GetRow(namerow);
            List<string> cols = new List<string>();
            for (int i = 0; i < row.LastCellNum; i++)
            {
                cols.Add(row.GetCell(i).ToString());
            }
            //映射列
            if (null == mapping)
            {
                //自动映射列
                mapping = new List<KeyValuePair<string, string>>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    mapping.Add(new KeyValuePair<string, string>(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName));
                }
            }
            else
            {
                foreach (var kv in mapping)
                {
                    if (!dt.Columns.Contains(kv.Key)) dt.Columns.Add(kv.Key);//添加必须的列
                }
            }
            //列映射
            int[] _map = new int[mapping.Count];//映射关系
            for (int i = 0; i < _map.Length; i++)
            {
                _map[i] = cols.IndexOf(mapping[i].Value);
            }
            //导入数据
            dt.Rows.Clear();
            for (int ri = namerow + 1; ri < sheet.LastRowNum; ri++)
            {
                var dr = dt.NewRow();
                row = sheet.GetRow(ri);
                for (int i = 0; i < _map.Length; i++)
                {
                    if(_map[i] >= 0)
                    {
                        var cell = row.GetCell(_map[i]);
                        if (null != cell) 
                        {
                            dr[mapping[i].Key] =  GetValue(cell, dt.Columns[mapping[i].Key].DataType);
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
        }

        public static object GetValue(ICell e, Type t)
        {
            //时间
            if(t == typeof(DateTime))
            {
                if (e.CellType == CellType.Numeric || e.CellType == CellType.Formula)
                    return e.DateCellValue;
                else 
                {
                    DateTime.TryParse(e.StringCellValue, out DateTime tm); return tm;
                }
            }
            //其它
            switch (e.CellType)
            {
                case CellType.Formula:
                case CellType.Numeric:
                    return e.NumericCellValue;
                case CellType.Unknown:
                case CellType.String:
                    if (IsNumeric(t))
                    {
                        double.TryParse(e.StringCellValue, out double d);
                        return d;
                    }
                    return e.StringCellValue;
                case CellType.Boolean:
                    return e.NumericCellValue;
                case CellType.Error:
                case CellType.Blank:
                default:
                    return default;
            }
        }

        public static bool IsNumeric(Type t)
        {
            return !t.IsClass && !t.IsInterface && t.GetInterfaces().Any(z => z == typeof(IFormattable));
        }


    }
}
