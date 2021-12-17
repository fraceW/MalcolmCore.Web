using MalcolmCore.Utils.Logs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MalcolmCore.Utils.HostedServices
{
    public class TokenRefreshService : BackgroundService
    {
        private readonly ILogger<TokenRefreshService> _logger;
        public TokenRefreshService(ILogger<TokenRefreshService> logger) 
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) 
            {
                LogUtils.Info(DateTime.Now.ToLongTimeString() + ": Refresh Token!", "WebLog");
                _logger.LogInformation(DateTime.Now.ToLongTimeString() + ": Refresh Token!");//在此写需要执行的任务
                await Task.Delay(5000, stoppingToken);
            }  
        }
    }
}
