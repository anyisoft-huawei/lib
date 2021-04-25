using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace lib.cache
{
    public class LocalCache : ICache
    {

        public object Get(string key)
        {
            var objCache = HttpRuntime.Cache;
            return objCache[key];
        }

        public bool Set(string key, object value)
        {
            var objCache = HttpRuntime.Cache;
            objCache.Insert(key, value);
            return true;
        }

        public static void SetMemcached(string key, object obj, TimeSpan slidingExpiration)
        {
            var objCache = HttpRuntime.Cache;
            objCache.Insert(key, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, slidingExpiration);
        }

        public bool Remove(string key)
        {
            var objCache = HttpRuntime.Cache;
            objCache.Remove(key);
            return true;
        }
    }
}
