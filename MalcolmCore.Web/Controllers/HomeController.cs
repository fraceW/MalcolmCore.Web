using MalcolmCore.Data;
using MalcolmCore.Data.Models;
using MalcolmCore.IService;
using MalcolmCore.Utils.Logs;
using MalcolmCore.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MalcolmCore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBaseService _IBaseService;

        public HomeController(ILogger<HomeController> logger, IBaseService baseService)
        {
            _logger = logger;
            _IBaseService = baseService;
        }

        public IActionResult Index()
        {
            //useinfo useinfo = new useinfo()
            //{
            //    id = Guid.NewGuid().ToString(),
            //    usename = "wll",
            //    pwd = "123456",
            //    useremark = "普通用户",
            //    creatdate = DateTime.Now
            //};
            //_IBaseService.Add<useinfo>(useinfo);
            //LogUtils.Debug("ok");

            //LogUtils.Info("Info", "ApiLog");
            string guid = Guid.NewGuid().ToString();
            OneToManySingle data = new OneToManySingle() { Id = guid
                , oneToManies = new List<OneToManyMany>() 
                { 
                    new OneToManyMany(){Id = Guid.NewGuid().ToString(),OneId = guid},
                    new OneToManyMany(){Id = Guid.NewGuid().ToString(),OneId = guid},
                    new OneToManyMany(){Id = Guid.NewGuid().ToString(),OneId = guid}
                } };
            _IBaseService.Add<OneToManySingle>(data);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
