using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace lib.obj
{

    /// <summary>
    /// 对象转换帮助类
    /// </summary>
   public static class ClassHelper
    {

        /// <summary>
        /// 复制类(深度克隆字段/属性)
        /// </summary>
        /// <param name="_t">源</param>
        /// <returns></returns>
        public static T GetCopy<T>(T _t)
        {
            T t = Activator.CreateInstance<T>();
            foreach (var i in _t.GetType().GetFields())
            {
                i.SetValue(t, i.GetValue(_t));
            }
            foreach (var i in _t.GetType().GetProperties())
            {
                i.SetValue(t, i.GetValue(_t));
            }
            return t;
        }

        /// <summary>
        /// 获取常量值集合
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetConstVals(Type t)
        {
            var _dic = new Dictionary<string, string>();
            foreach (FieldInfo i in t.GetFields())
            {
                _dic.Add(i.Name, i.GetRawConstantValue().ToString());
            }
            return _dic;
        }

        /// <summary>
        /// 获取对象字段的值
        /// </summary>
        /// <param name="_o">要取值的对象</param>
        /// <param name="name">成员名称</param>
        /// <returns></returns>
        public static object GetValue(object _o, string name)
        {
            var i= _o.GetType().GetField(name);
            return i?.GetValue(_o);
        }

        /// <summary>
        /// 设置对象字段的值
        /// </summary>
        /// <param name="_o">对象</param>
        /// <param name="name">成员名称</param>
        /// <param name="value">值</param>
        public static bool SetValue(object _o, string name, object value)
        {
            var i = _o.GetType().GetField(name);
            if (i == null) return false;
            i.SetValue(_o, value);
            return true;  
        }

        /// <summary>
        /// 将类字段/属性转为字典集合
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary<T>(T t)
        {
            var d = new Dictionary<string, string>();
            foreach (FieldInfo i in t.GetType().GetFields())
            {
                var v = i.GetValue(t);
                d.Add(i.Name, v == null ? "" : v.ToString());
            }
            foreach (PropertyInfo i in t.GetType().GetProperties())
            {
                var v = i.GetValue(t);
                d.Add(i.Name, v == null ? "" : v.ToString());
            }
            return d;
        }

        /// <summary>
        /// 将数据表转为类集合(仅为字段赋值)
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns></returns>
        public static List<T> ToClassFields<T>(this System.Data.DataTable dt)
        {
            var list = new List<T>();
            if (dt != null)
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var t = Activator.CreateInstance<T>();
                    foreach (FieldInfo i in t.GetType().GetFields())
                    {
                        if (dt.Columns.Contains(i.Name))
                        {
                            object value = dr[i.Name];
                            Type tmpType = Nullable.GetUnderlyingType(i.FieldType) ?? i.FieldType;
                            object safeValue = (value == null) ? null : Convert.ChangeType(value, tmpType);
                            if (value != DBNull.Value)
                            {
                                i.SetValue(t, safeValue);
                            }
                        }
                    }
                    list.Add(t);
                }
            }
            return list;
        }

        /// <summary>
        /// 将数据表转为类集合(为字段和属性赋值)
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns></returns>
        public static List<T> ToClassFieldsPropertyies<T>(this System.Data.DataTable dt)
        {
            var list = new List<T>();
            if (dt != null)
            {
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var t = Activator.CreateInstance<T>();
                    foreach (PropertyInfo i in t.GetType().GetProperties())
                    {
                        if (dt.Columns.Contains(i.Name))
                        {
                            object value = dr[i.Name];
                            Type tmpType = Nullable.GetUnderlyingType(i.PropertyType) ?? i.PropertyType;
                            object safeValue = (value == null) ? null : Convert.ChangeType(value, tmpType);
                            if (value != DBNull.Value)
                            {
                                i.SetValue(t, safeValue);
                            }
                        }
                    }
                    foreach (FieldInfo i in t.GetType().GetFields())
                    {
                        if (dt.Columns.Contains(i.Name))
                        {
                            object value = dr[i.Name];
                            Type tmpType = Nullable.GetUnderlyingType(i.FieldType) ?? i.FieldType;
                            object safeValue = (value == null) ? null : Convert.ChangeType(value, tmpType);
                            if (value != DBNull.Value)
                            {
                                i.SetValue(t, safeValue);
                            }
                        }
                    }
                    list.Add(t);
                }
            }
            return list;
        }

        /// <summary>
        /// 将列表(字段)转为数据表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_list">对象列表</param>
        /// <returns></returns>
        public static System.Data.DataTable ToDataTableFields<T>(this List<T> _list)
        {
            var dt = new System.Data.DataTable();
            var fs = typeof(T).GetFields();
            foreach (var item in fs)
            {
                dt.Columns.Add(item.Name, item.FieldType);
            }
            foreach (var item in _list)
            {
                var dr = dt.NewRow();
                foreach (var key in fs)
                {
                    dr[key.Name] = key.GetValue(item);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }


    }
}
