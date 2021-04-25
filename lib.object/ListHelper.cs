using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.obj
{
    class ListHelper
    {
        /// <summary>
        /// 元素前移
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static void ToPrve<T>(List<T> list, int index)
        {
            if (index > 0 && index < list.Count)
            {
                var obj = list[index];
                list.RemoveAt(index);
                index--;
                list.Insert(index, obj);
            }
        }//Last

        /// <summary>
        /// 元素后移
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        public static void ToNext<T>(List<T> list, int index)
        {
            if (index >= 0 && index < list.Count - 1)
            {
                var obj = list[index];
                list.RemoveAt(index);
                index++;
                list.Insert(index, obj);
            }
        }//Next
    }
}
