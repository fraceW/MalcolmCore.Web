using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Utils.Caches
{
    public class MemoryCacheHelp
    {
        public IMemoryCache _cache { get; set; }
        public MemoryCacheHelp()
        {
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }
    }
}
