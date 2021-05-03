using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.db
{
    /// <summary>
    /// 提供一组数据操作方法，数据模块可继承此类。
    ///注意继承此类所有数据表字段均使用字段而非属性，如：public int id;
    /// </summary>
    public class ModelBase
    {

        #region 基方法

        /// <summary>
        /// 查找数据,用于可变条件数据查询
        /// </summary>
        /// <param name="_db">数据库连接字符串</param>
        /// <param name="_sql">前段sql</param>
        /// <param name="_where">参数列表，请在传入之前做键名安全检查</param>
        /// <param name="_end">排序、分组等，第一个字符需要为空格</param>
        /// <returns></returns>
        public static DataTable GetList(string _db, string _sql, Dictionary<string, string> _where, string _end = "")
        {
            var sqlw = new StringBuilder();
            var pms = new List<SqlParameter>();
            foreach (var key in _where.Keys)
            {
                pms.Add(new SqlParameter("@" + key, _where[key]));
                sqlw.Append(string.Format(" and {0}=@{0}", key));
            }
            var sql = string.Format("{0} where {2} {3}", _sql, sqlw.ToString().Substring(5), _end);
            return SqlHelper.Read(_db, sql, pms.Count > 0 ? pms.ToArray() : null, CommandType.Text);
        }

        /// <summary>
        /// 查找数据,用于可变条件数据查询
        /// </summary>
        /// <param name="_db">数据库连接字符串</param>
        /// <param name="_stored">过程名</param>
        /// <param name="dic">查询条件，注：只适用过程参数为'@字段名'</param>
        /// <returns></returns>
        public static DataTable GetListForStored(string _db, string _stored, Dictionary<string, string> dic = null)
        {
            var pms = new List<SqlParameter>();
            if (null != dic)
            {
                foreach (var key in dic.Keys)
                {
                    pms.Add(new SqlParameter("@" + key, dic[key]));
                }
            }
            return SqlHelper.Read(_db, _stored, pms.Count > 0 ? pms.ToArray() : null, CommandType.StoredProcedure);
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="_db">数据库连接字符串</param>
        /// <param name="_table">表名</param>
        /// <param name="_wherename">条件名称</param>
        /// <param name="_pms">参数列表</param>
        /// <returns></returns>
        public static int Update(string _db, string _table, List<string> _wherename, Dictionary<string, string> _pms)
        {
            var sqlw = new StringBuilder();
            var sqlv = new StringBuilder();
            var pms = new List<SqlParameter>();
            foreach (var key in _pms.Keys)
            {
                pms.Add(new SqlParameter("@" + key, _pms[key]));
                if (_wherename.Contains(key)) sqlw.Append(string.Format(" and {0}=@{0}", key));//如果是条件
                else sqlv.Append(string.Format(",{0}=@{0}", key));
            }
            var sql = string.Format("update {0} set {1} where {2}", _table, sqlv.ToString().Substring(1), sqlw.ToString().Substring(5));
            return SqlHelper.Query(_db, sql, pms.Count > 0 ? pms.ToArray() : null, CommandType.Text);
        }

     
        #endregion
    }
}
