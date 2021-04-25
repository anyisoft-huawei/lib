using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib.cache
{
    public interface ICache
    {
        bool Set(string key, object value);
        object Get(string key);
        bool Remove(string key);
    }
}
