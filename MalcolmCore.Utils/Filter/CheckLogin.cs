using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MalcolmCore.Utils.Filter
{
    public class CheckLogin : Attribute, IAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public CheckLogin(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //跳过登录
            var isDefined = false;
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            //var data = context.ActionDescriptor.EndpointMetadata.ToList();

            //bool res = data[1].ToString().Equals(typeof(SkipAttribute).ToString());

            if (controllerActionDescriptor != null)
            {
                isDefined = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                   .Any(a => a.GetType().Equals(typeof(SkipAttribute)));
            }

            //1. 校验是否标记跨过登录验证
            if (isDefined)
            {
                //表示该方法或控制器跨过登录验证
                //继续走控制器中的业务即可
            }
            else
            {
                //这里只是简单的做一下校验
                var userName = _session.GetString("userName");
                if (string.IsNullOrEmpty(userName))
                {
                    //截断请求
                    context.Result = new RedirectResult("~/Login/index");
                }
            }
        }
    }
}
