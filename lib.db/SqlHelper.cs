using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;
using System.Reflection;

namespace lib.db
{

    /// <summary>
    /// sql帮助类
    /// </summary>
    public static class SqlHelper
    {

        /// <summary>
        /// 以现有连接字符串初始化一个连接池打开的连接配置
        /// </summary>
        /// <param name="_conntext">数据库连接字符串</param>
        /// <param name="min">最小连接数</param>
        /// <param name="max">最大连接数</param>
        /// <returns>连接配置</returns>
        public static SqlConnectionStringBuilder GetConnectionConfig(string _conntext, int min = 5, int max = 20)
        {
            var config = new SqlConnectionStringBuilder();
            config.ConnectionString = _conntext;
            config.Pooling = true;//开启连接池
            config.MinPoolSize = min;
            config.MaxPoolSize = max;
            //conn.LoadBalanceTimeout;//是否需要设置负载平衡超时
            return config;
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="_conn">已打开的数据库连接</param>
        /// <param name="sql">sql命令文本</param>
        /// <param name="_params">sql参数集</param>
        /// <param name="_type">sql命令类型</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        public static int Query(this SqlConnection _conn, string sql, SqlParameter[] _params = null, CommandType _type = CommandType.Text, SqlTransaction trans = null)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = _type;
                cmd.Transaction = trans;
                if (null != _params) cmd.Parameters.AddRange(_params);
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///  执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="_conn">数据库连接字符串</param>
        /// <param name="sql">sql命令文本</param>
        /// <param name="_params">sql参数集</param>
        /// <param name="_type">sql命令类型</param>
        /// <returns></returns>
        public static int Query(string _conn, string sql, SqlParameter[] _params = null, CommandType _type = CommandType.Text)
        {
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                return conn.Query(sql, _params, _type);
            }
        }

        /// <summary>
        ///  事务方式执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="_conn">数据库连接字符串</param>
        /// <param name="sql">sql命令文本</param>
        /// <param name="_params">sql参数集</param>
        /// <param name="_type">sql命令类型</param>
        public static int TransQuery(string _conn, string sql, SqlParameter[] _params = null, CommandType _type = CommandType.Text)
        {
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                var trans = conn.BeginTransaction();
                try
                {
                    var i = conn.Query(sql, _params, _type, trans);
                    trans.Commit();
                    return i;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }


        /// <summary>
        /// 执行SQL语句，并返回第一行第一列
        /// </summary>
        /// <param name="_conn">已打开的数据库连接</param>
        /// <param name="sql">sql命令文本</param>
        /// <param name="_params">sql参数集</param>
        /// <param name="_type">sql命令类型</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        public static object Scalar(this SqlConnection _conn, string sql, SqlParameter[] _params = null, CommandType _type = CommandType.Text, SqlTransaction trans = null)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = _type;
                cmd.Transaction = trans;
                if (null != _params) cmd.Parameters.AddRange(_params);
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        ///  执行SQL语句，并返回第一行第一列
        /// </summary>
        /// <param name="_conn">数据库连接字符串</param>
        /// <param name="sql">sql命令文本</param>
        /// <param name="_params">sql参数集</param>
        /// <param name="_type">sql命令类型</param>
        /// <returns></returns>
        public static object Scalar(string _conn, string sql, SqlParameter[] _params = null, CommandType _type = CommandType.Text)
        {
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                return conn.Scalar(sql, _params, _type);
            }
        }


        /// <summary>
        /// 从数据库读取数据
        /// </summary>
        /// <param name="_conn">已打开的数据库连接</param>
        /// <param name="sql">sql命令文本</param>
        /// <param name="_params">sql参数集</param>
        /// <param name="_type">sql命令类型</param>
        /// <param name="trans">事务</param>
        /// <returns></returns>
        public static DataTable Read(this SqlConnection _conn, string sql, SqlParameter[] _params = null, CommandType _type = CommandType.Text, SqlTransaction trans = null)
        {
            using (SqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = _type;
                cmd.Transaction = trans;
                if (null != _params) cmd.Parameters.AddRange(_params);
                using (var rd = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(rd);
                    rd.Close();
                    return dt;
                }
            }
        }

        /// <summary>
        /// 从数据库读取数据
        /// </summary>
        /// <param name="_conn">数据库连接字符串</param>
        /// <param name="sql">sql命令文本</param>
        /// <param name="_params">sql参数集</param>
        /// <param name="_type">sql命令类型</param>
        /// <returns></returns>
        public static DataTable Read(string _conn, string sql, SqlParameter[] _params = null, CommandType _type = CommandType.Text)
        {
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                return conn.Read(sql, _params, _type);
            }
        }

        /// <summary>
        /// Update 函数的操作类型
        /// </summary>
        public enum UpdateType
        {
            /// <summary>
            /// 将数据库的数据下载到数据表中
            /// </summary>
            Select,
            /// <summary>
            /// 将数据表中的数据更新或插入到数据库
            /// </summary>
            InsertOrUpdate,
            /// <summary>
            /// 将数据表中的数据从数据库删除
            /// </summary>
            Delete
        }

        /// <summary>
        /// 更新数据到数据库, 默认为获取, 返回更新后的表
        /// </summary>
        /// <param name="_conn">已打开的数据库连接</param>
        /// <param name="dt">数据集</param>
        /// <param name="sql">sql数据筛选文本，筛选范围越小执行速度越快</param>
        /// <param name="key">数据主键</param>
        /// <param name="_params">sql参数集</param>
        /// <param name="_type">sql命令类型</param>
        /// <param name="up">操作类型</param>
        /// <returns></returns>
        public static DataTable Update(this SqlConnection _conn, DataTable dt, string key, string sql, SqlParameter[] _params = null, CommandType _type = CommandType.Text, UpdateType up = UpdateType.Select)
        {
            using (SqlCommand cmd = _conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = _type;
                if (null != _params) cmd.Parameters.AddRange(_params);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    var db = new DataTable();
                    da.Fill(db);//下载数据
                    switch (up)
                    {
                        case UpdateType.InsertOrUpdate:
                            db.PrimaryKey = new DataColumn[] { db.Columns[key] };//设置主键                         
                            var div = new Dictionary<int, int>();
                            for (int i = 0; i < dt.Columns.Count; i++)//获取列映射
                            {
                                int p = db.Columns.IndexOf(dt.Columns[i].ColumnName);
                                if (p >= 0) div.Add(i, p);
                            }                           
                            for (int i = dt.Rows.Count - 1; i >= 0; i--)//要修改的数据
                            {
                                var dr = db.Rows.Find(dt.Rows[i][key]);//以主键查找数据行
                                if(null != dr)//对行进行修改
                                {
                                    foreach (var k in div.Keys) dr[div[k]] = dt.Rows[i][k];//修改行
                                    dt.Rows.RemoveAt(i);//移除已修改行
                                }
                            }
                            foreach (DataRow item in dt.Rows)//要增加的数据
                            {
                                var dr = db.NewRow();
                                foreach (var k in div.Keys) dr[div[k]] = item[k];//修改行
                                db.Rows.Add(dr);
                            }
                            var cb = new SqlCommandBuilder(da);
                            da.UpdateCommand = cb.GetUpdateCommand();
                            da.InsertCommand = cb.GetInsertCommand();
                            da.Update(dt);//提交修改
                            break;
                        case UpdateType.Delete:
                            db.PrimaryKey = new DataColumn[] { db.Columns[key] };//设置主键
                            for (int i = 0; i < dt.Rows.Count; i++) db.Rows.Find(dt.Rows[i][key])?.Delete();//标记删除
                            da.DeleteCommand = new SqlCommandBuilder(da).GetDeleteCommand();
                            da.Update(dt);//提交修改
                            break;
                        default:
                            break;
                    }
                    return db;
                }
            }
        }

        /// <summary>
        /// 更新数据到数据库, 默认为获取, 返回更新后的表
        /// </summary>
        /// <param name="_conn">已打开的数据库连接</param>
        /// <param name="dt">数据集</param>
        /// <param name="sql">sql数据筛选文本，筛选范围越小执行速度越快</param>
        /// <param name="key">数据主键</param>
        /// <param name="_params">sql参数集</param>
        /// <param name="_type">sql命令类型</param>
        /// <param name="up">操作类型</param>
        /// <returns></returns>
        public static DataTable Update(string _conn, DataTable dt, string key, string sql, SqlParameter[] _params = null, CommandType _type = CommandType.Text, UpdateType up = UpdateType.Select)
        {
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                return conn.Update(dt, key, sql, _params, _type, up);
            }
        }


        /// <summary>
        /// 批量插入数据,映射列包含数据表中的全部列
        /// </summary>
        /// <param name="_conn">已打开的数据库连接</param>
        /// <param name="dt">要插入的数据</param>
        /// <param name="tablename">数据库表名</param>
        public static void InsertTable(this SqlConnection _conn, DataTable dt, string tablename)
        {
            using (var sqlBC = new SqlBulkCopy(_conn))
            {
                sqlBC.BatchSize = dt.Rows.Count;
                //sqlBC.BulkCopyTimeout = 100;
                //sqlBC.NotifyAfter = dt.Rows.Count;
                //sqlBC.SqlRowsCopied += rce;
                sqlBC.DestinationTableName = tablename;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    sqlBC.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                }
                sqlBC.WriteToServer(dt);
            }
        }

        /// <summary>
        /// 批量插入数据,映射列包含数据表中的全部列
        /// </summary>
        /// <param name="_conn">数据库连接字符串</param>
        /// <param name="dt">要插入的数据</param>
        /// <param name="tablename">数据库表名</param>
        public static void InsertTable(string _conn, DataTable dt, string tablename)
        {
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                conn.InsertTable(dt, tablename);
            }
        }

        /// <summary>
        /// 批量插入数据,用指定列名映射列
        /// </summary>
        /// <param name="_conn">已打开的数据库连接</param>
        /// <param name="dt">要插入的数据</param>
        /// <param name="tablename">数据库表名</param>
        /// <param name="nvs">映射关系：表列-数据库列</param>
        public static void InsertTable(this SqlConnection _conn, DataTable dt, string tablename, KeyValuePair<string, string>[] nvs)
        {
            using (var sqlBC = new SqlBulkCopy(_conn))
            {
                sqlBC.BatchSize = dt.Rows.Count;
                //sqlBC.BulkCopyTimeout = 100;
                //sqlBC.NotifyAfter = dt.Rows.Count;
                //sqlBC.SqlRowsCopied += rce;
                sqlBC.DestinationTableName = tablename;
                foreach (KeyValuePair<string, string> item in nvs)
                {
                    sqlBC.ColumnMappings.Add(item.Key, item.Value);
                }
                sqlBC.WriteToServer(dt);
            }
        }

        /// <summary>
        /// 批量插入数据,用指定列名映射列
        /// </summary>
        /// <param name="_conn">数据库连接字符串</param>
        /// <param name="dt">要插入的数据</param>
        /// <param name="tablename">数据库表名</param>
        /// <param name="nvs">映射关系：表列-数据库列</param>
        public static void InsertTable(string _conn, DataTable dt, string tablename, KeyValuePair<string, string>[] nvs)
        {
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                conn.InsertTable(dt, tablename, nvs);
            }
        }


        /// <summary>
        /// 事务方式执行多条sql
        /// </summary>
        /// <param name="_conn">已打开的数据库连接</param>
        /// <param name="sqls">sql集合</param>
        public static void TransSqlCollection(this SqlConnection _conn, string[] sqls)
        {
            using (var trans = _conn.BeginTransaction())
            using (var cmd = _conn.CreateCommand())
            {
                cmd.Transaction = trans;//关联事务
                try
                {
                    foreach (var sql in sqls)
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 事务方式执行多条sql
        /// </summary>
        /// <param name="_conn">数据库连接字符串</param>
        /// <param name="sqls">sql集合</param>
        public static void TransSqlCollection(string _conn, string[] sqls)
        {
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                conn.TransSqlCollection(sqls);
            }
        }


        /// <summary>
        /// 事务方式执行多个参数集
        /// </summary>
        /// <param name="_conn">已打开的数据库连接</param>
        /// <param name="sql">sql命令文本</param>
        /// <param name="_type">命令类型</param>
        /// <param name="_list">参数集集合</param>
        public static void TransParamsCollection(this SqlConnection _conn, string sql, CommandType _type, List<SqlParameter[]> _list)
        {
            using (var trans = _conn.BeginTransaction())
            using (var cmd = _conn.CreateCommand())
            {
                cmd.Transaction = trans;//关联事务
                cmd.CommandText = sql;
                try
                {
                    foreach (var _params in _list)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddRange(_params);
                        cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// 事务方式执行多个参数集
        /// </summary>
        /// <param name="_conn">数据库连接字符串</param>
        /// <param name="sql">sql命令文本</param>
        /// <param name="_type">命令类型</param>
        /// <param name="_list">参数集集合</param>
        public static void TransParamsCollection(string _conn, string sql, CommandType _type, List<SqlParameter[]> _list)
        {
            using (SqlConnection conn = new SqlConnection(_conn))
            {
                conn.Open();
                conn.TransParamsCollection(sql, _type, _list);
            }
        }


        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="_conn">已打开的数据库连接</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public static bool HasTable(this SqlConnection _conn, string table)
        {
            var o = _conn.Scalar(string.Format("select count(1) from sysobjects where name = '{0}'", table));
            return DBNull.Value == o ? false : Convert.ToInt32(o) > 0;
        }

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="_conn">数据库连接字符串</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public static bool HasTable(string _conn, string table)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                return conn.HasTable(table);
            }
        }

        /// <summary>
        /// 数据库是否存在
        /// </summary>
        /// <param name="_conn">已打开的数据库连接</param>
        /// <param name="name">数据库名</param>
        /// <returns></returns>
        public static bool HasDataBase(this SqlConnection _conn, string name)
        {
            //v.InitialCatalog = "master";
            var o = _conn.Scalar(string.Format("select count(1) from sys.databases where name = '{0}'", name));
            return DBNull.Value == o ? false : Convert.ToInt32(o) > 0;
        }

        /// <summary>
        /// 数据库是否存在
        /// </summary>
        /// <param name="_conn">数据库连接字符串</param>
        /// <param name="name">数据库名</param>
        /// <returns></returns>
        public static bool HasDataBase(string _conn, string name)
        {
            using (var conn = new SqlConnection(_conn))
            {
                conn.Open();
                return conn.HasDataBase(name);
            }
        }

        /// <summary>
        /// 检测sql是否安全
        /// </summary>
        /// <param name="sql">要判断的字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSql(this string sql)
        {
            return !System.Text.RegularExpressions.Regex.IsMatch(sql, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

    }
}
