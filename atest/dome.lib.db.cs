using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.db;
using lib.obj;
using System.Data.SqlClient;
using System.Data;

namespace atest
{
    public class domedb
    {
        public static string DB1 = "你的数据库连接字符串";
        public static string DB2 = "你的数据库连接字符串";

        static domedb()
        {
            //为你的数据连接添加连接池支持
            //DB1 = SqlHelper.GetConnectionConfig(DB1, 3, 10).ConnectionString;
            //DB2 = SqlHelper.GetConnectionConfig(DB2, 3, 10).ConnectionString;
        }


        //你可以这样调用
        public void Test2()
        {
            var list = SqlHelper.Read(DB2, "select * from table where id=@id", new SqlParameter[] {
                    new SqlParameter("@id", 1)
                    //...
                 }, CommandType.Text).ToClassFields<TestModle>();
            //...
        }
        //你也可以这样调用
        public void Test()
        {
            DataTable dt;
            using (var conn = new SqlConnection(DB1))
            {
                conn.Open();//需要显式打开
                var trans = conn.BeginTransaction();//此处为示例，仅写才需要使用到事务
                try
                {
                    dt = conn.Read("select * from table where id=@id", new SqlParameter[] {
                         new SqlParameter("@id", 1)
                        //...
                    }, CommandType.Text);
                    trans.Commit();//此处为示例，仅写才需要使用到事务
                    //
                    TestModle tm = dt.ToClassFields<TestModle>()[0];
                }
                catch (Exception)
                {
                    trans.Rollback();//此处为示例，仅写才需要使用到事务
                    //其它处理
                }
            }

        }


        /// <summary>
        /// 你的数据库表对应类
        /// </summary>
        public class TestModle
        {
            //你的数据库字段
            public string name;
        }


    }
}
