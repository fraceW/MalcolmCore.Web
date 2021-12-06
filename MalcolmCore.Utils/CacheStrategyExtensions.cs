using CSRedis;
using MalcolmCore.Utils.Common;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Utils
{
    public static class CacheStrategyExtensions
    {
        /// <summary>
        /// 添加缓存类型
        /// (最后无论哪种模式，都把AddMemoryCache注入，方便单独使用IMemoryCache)（视情况而定）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="CacheType">有4种取值 (Redis：代表基于CSRedisCore使用redis缓存, 并实例化redis相关对象. Memory：代表使用内存缓存; 
        /// StackRedis: 代表基于StackExchange.Redis初始化; "null":表示什也不注入)</param>
        /// <returns></returns>
        public static IServiceCollection AddCacheStrategy(this IServiceCollection services, string CacheType) 
        {
            switch (CacheType)
            {
                case "Memory": services.AddDistributedMemoryCache(); break;
                case "Redis":
                    {
                        //基于CSRedisCore初始化
                        //初始化redis的两种使用方式
                        var csredis = new CSRedisClient(ConfigHelp.GetString("RedisStr"));
                        services.AddSingleton(csredis);
                        RedisHelper.Initialization(csredis);

                        //初始化缓存基于redis
                        services.AddSingleton<IDistributedCache>(new CSRedisCache(csredis));
                    }; break;
                case "StackRedis":
                    {
                        //基于StackExchange.Redis初始化（该程序集这里不初始化缓存）
                        var connectionString = ConfigHelp.GetString("RedisStr");
                        //int defaultDB = Convert.ToInt32(ConfigHelp.GetString("RedisStr:defaultDB"));
                        services.AddSingleton(new SERedisHelp(connectionString));
                    }; break;
                case "null":
                    {
                        //什么也不注入
                    }; break;
                default: throw new Exception("缓存类型无效");
            }
            //最后都把AddMemoryCache注入，方便单独使用IMemoryCache进行内存缓存（视情况而定）
            //services.AddMemoryCache();

            return services;
        }

    }
}
