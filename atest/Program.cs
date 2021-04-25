using lib.file;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib.icv;

namespace atest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                var v = ImageHelper.GetSizeForProduct(500, 100, 120);
                Console.Out.WriteLine(string.Format("500, 100, 120:{0},{1},{2}", v.Width, v.Height, v.Width * v.Height));
                v = ImageHelper.GetSizeForProduct(1800, 1600, 120);
                Console.Out.WriteLine(string.Format("1800, 1600, 120:{0},{1},{2}", v.Width, v.Height, v.Width * v.Height));
                v = ImageHelper.GetSizeForProduct(1500, 600, 120);
                Console.Out.WriteLine(string.Format("1500, 600, 120:{0},{1},{2}", v.Width, v.Height, v.Width * v.Height));
                v = ImageHelper.GetSizeForProduct(300, 500, 120);
                Console.Out.WriteLine(string.Format("300, 500, 120:{0},{1},{2}", v.Width, v.Height, v.Width * v.Height));
                v = ImageHelper.GetSizeForProduct(500, 500, 120);
                Console.Out.WriteLine(string.Format("500, 500, 120:{0},{1},{2}", v.Width, v.Height, v.Width * v.Height));




            }
            catch (Exception e)
            {
                Console.Out.Write(e.Message);
            }
            Console.In.Read();
        }
    }
}
