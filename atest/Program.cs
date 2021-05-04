using lib.file;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.icv;
using Newtonsoft.Json;

namespace atest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string val = "[{'id':1},{'id':2},{'id':3},{'id':4}]";
                var dt = JsonConvert.DeserializeObject<System.Data.DataTable>(val);
                dt.TableName = "dbtest";
                for (int i = 0; i < 16; i++)
                {
                    
                }


                Console.Out.WriteLine(string.Format(":{0}", Get(3)[1].i));
                Console.Out.WriteLine(string.Format(":{0}", Get(3)[0].i));
            }
            catch (Exception e)
            {
                Console.Out.Write(e.Message);
            }
            Console.In.Read();
        }

        public static test[] Get(int i)
        {
            using (var a = new test())
            using (var t = new test())
            {
                t.i = i;
                return new test[] { t, a };
            }
        }

        public class test : IDisposable
        {
            public int i = 0;
            public void Dispose()
            {
                i++;
            }
        }


    }



}
