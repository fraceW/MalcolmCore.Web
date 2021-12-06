using MalcolmCore.Utils.Logs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Utils.Common
{
    public static class LogStrategy
    {
        public static IServiceCollection AddLogStrategy(this IServiceCollection services) 
        {
            LogUtils.InitLog();
            return services;
        }
    }
}
