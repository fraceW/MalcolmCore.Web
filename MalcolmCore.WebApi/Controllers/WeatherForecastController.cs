using MalcolmCore.IService.Login;
using MalcolmCore.Utils.Caches;
using MalcolmCore.Utils.Common;
using MalcolmCore.Utils.Filter;
using MalcolmCore.Utils.Model;
using MalcolmCore.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MalcolmCore.WebApi.Controllers
{
    
    public class WeatherForecastController : BaseController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILoginService _ILoginService;

        private readonly ILogger<WeatherForecastController> _logger;

        private IMemoryCache _cache;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ILoginService iLoginService, IMemoryCache cache)
        {
            _logger = logger;
            _ILoginService = iLoginService;
            _cache = cache;
        }

        [HttpGet]
        [SkipAttribute]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        //[HttpGet]
        //public string GetInfor()
        //{
        //    return "hello";
        //}

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userAccount">账号</param>
        /// <param name="userPwd">密码</param>
        /// <returns></returns>
        [HttpPost]
        [SkipAttribute]
        public RetuenResult<Token> CheckLogin(string userAccount, string userPwd)
        {
            RetuenResult <Token> res = _ILoginService.Login(userAccount, userPwd);
            string refreshToken = string.Empty;
            if (!_cache.TryGetValue(res.data.accessToken, out refreshToken)) 
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSize(1024)
                    .SetAbsoluteExpiration(TimeSpan.FromHours(12));
                _cache.Set(res.data.accessToken, res.data.refreshToken, cacheOptions);
            }
            return res;
        }

        /// <summary>
         /// 获取信息
         /// </summary>
         /// <returns></returns>
        [HttpPost]
        public string GetInfo()
         {
            return "hello";
         }

        [HttpPost]
        [SkipAttribute]
        public string TextAction1() 
        {
            ActionHelp.Init(Capture());
            return "TextAction1";
        }

        [HttpPost]
        [SkipAttribute]
        public string TextAction2()
        {
            //ActionHelp.Do();
            string data = string.Empty;
            _cache.TryGetValue("mykey", out data);
            return string.IsNullOrWhiteSpace(data) ? "空" : data;
            //return _cache.Get<string>("mykey");
            //return "TextAction2";
            //DateTime cacheEntry;

            //// Look for cache key.
            //if (!_cache.TryGetValue("mykey", out cacheEntry))
            //{
            //    // Key not in cache, so get data.
            //    cacheEntry = DateTime.Now;

            //    // Set cache options.
            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //        // Keep in cache for this time, reset time if accessed.
            //        .SetSlidingExpiration(TimeSpan.FromMinutes(3));

            //    // Save data in cache.
            //    _cache.Set("mykey", cacheEntry, cacheEntryOptions);
            //}
            //return cacheEntry;
        }
        [HttpPost]
        [SkipAttribute]
        public Action<string, string> Capture()
        {
            return (string carNumber, string ip) =>
            {

            };
        }

        [HttpPost]
        [SkipAttribute]
        public string CacheText() 
        {
            var cacheEntry = _cache.GetOrCreate("mykey", entry =>
            {
                entry.SetSlidingExpiration(TimeSpan.FromMinutes(3));
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            });
            return cacheEntry;
        }
        
    }
}
