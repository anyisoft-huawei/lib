using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace lib.cache
{
    public class MemCached : ICache
    {
        static MemcachedClient MC;
        public MemCached(MemcachedClientConfiguration memConfig)
        {
            MC = new MemcachedClient(memConfig);
        }

        public MemcachedClientConfiguration CreateConfig(List<IPEndPoint> ips, MemcachedProtocol pol)
        {
            MemcachedClientConfiguration memConfig = new MemcachedClientConfiguration();
            foreach (var ip in ips)
            {
                memConfig.Servers.Add(ip);//服务器
            }
            memConfig.Protocol = MemcachedProtocol.Binary;//协议
            //文件权限
            memConfig.Authentication.Type = typeof(PlainTextAuthenticator);
            memConfig.Authentication.Parameters["zone"] = "";
            //memConfig.Authentication.Parameters["userName"] = "?";
            //memConfig.Authentication.Parameters["password"] = "?";
            memConfig.SocketPool.MinPoolSize = 5;
            memConfig.SocketPool.MaxPoolSize = 200;
            return memConfig;
        }
        public MemcachedClientConfiguration CreateConfig(string[] ips = null, int port = 11211)
        {
            List<IPEndPoint> ipports = new List<IPEndPoint>();
            if (ips == null)
            {
                ipports.Add(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11211));
            }
            else
            {
                foreach (var i in ips)
                {
                    ipports.Add(new IPEndPoint(IPAddress.Parse(i), port));
                }
            }
            return CreateConfig(ipports, MemcachedProtocol.Binary);
        }

        public bool Set(string key, object value)
        {
            return MC.Store(StoreMode.Set, key, value);
        }

        public object Get(string key)
        {
            return MC.Get(key);
        }

        public bool Remove(string key)
        {
            return MC.Remove(key);
        }
    }
}
