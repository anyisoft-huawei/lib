using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lib.cache
{
    /// <summary>
    /// 缓存管理
    /// </summary>
    public class CacheManager
    {
        ICache Cached;
        public CacheManager(ICache ic)
        {
            Cached = ic;
        }

        public object Get(string key)
        {
            return Cached.Get(key);
        }

        public bool Set(string key, object o)
        {
            return Cached.Set(key, o);
        }

        public void Remove(string key)
        {
            Cached.Remove(key);
        }

    }
}
