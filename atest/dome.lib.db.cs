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
            var pm = new SqlParameter[] {
                        new SqlParameter("@id", 1),
                        new SqlParameter("@name", "name")
            };
            var list = SqlHelper.Read(DB2, "select * from table where id=@id and name=@name",pm, CommandType.Text).ToClassFields<TestModle>();
            var list2 = SqlHelper.Read(DB2, "存储过程名", pm, CommandType.StoredProcedure).ToClassFields<TestModle>();
            //...
        }
        //你也可以这样调用
        public void Test()
        {
            int dt;
            using (var conn = new SqlConnection(DB1))
            {
                conn.Open();//需要显式打开
                var trans = conn.BeginTransaction();//此处为示例，仅写才需要使用到事务
                try
                {
                    dt = conn.Query("insert into table (id,name) values(1,'name')", null, CommandType.Text, trans);
                    dt = conn.Query("insert into table (id,name) values(2,'name')", null, CommandType.Text, trans);

                    trans.Commit();//此处为示例，仅写才需要使用到事务
                    
                    //...
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


       static  string conn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\Code\test.mdf;Integrated Security=True;Connect Timeout=30";

        public static void Inster(DataTable dt)
        {
            SqlHelper.InsertTable(conn, dt, "dbtest");
        } 

        public static DataTable Get(ref int val)
        {
            var pm = new SqlParameter("@AYCOUNT", SqlDbType.Int);
            var dt = SqlHelper.Read(conn, "select *,@AYCONUT=COUNT(*) from dbtest", new SqlParameter[] { pm });
            val = (int)pm.Value;
            return dt;
        }





    }
}
