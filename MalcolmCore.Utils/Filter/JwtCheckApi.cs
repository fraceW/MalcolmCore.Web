using MalcolmCore.Utils.Caches;
using MalcolmCore.Utils.Common;
using MalcolmCore.Utils.Model;
using MalcolmCore.Utils.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MalcolmCore.Utils.Filter
{
    public class JwtCheckApi: ActionFilterAttribute
    {
        private IMemoryCache _cache;
        public JwtCheckApi(MemoryCacheHelp cache) 
        {
            _cache = cache._cache;
        }
        public override void OnActionExecuting(ActionExecutingContext context) 
        {
            //1.判断是否需要校验
            var isDefined = false;
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor != null)
            {
                isDefined = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                   .Any(a => a.GetType().Equals(typeof(SkipAttribute)));
            }

            if (isDefined == false)
            {
                //2. 判断是什么请求(ajax or 非ajax)
                var actionContext = context.HttpContext;
                //if (IsAjaxRequest(actionContext.Request))
                {
                    //表示是ajax
                    var token = context.HttpContext.Request.Headers["Authorization"].ToString();    //ajax请求传过来
                    string pattern = "^Bearer (.*?)$";
                    if (!Regex.IsMatch(token, pattern))
                    {
                        context.Result = new ContentResult { StatusCode = 401, Content = "token格式不对!格式为:Bearer {token}" };
                        return;
                    }
                    token = Regex.Match(token, pattern).Groups[1]?.ToString();
                    if (token == "null" || string.IsNullOrEmpty(token))
                    {
                        context.Result = new ContentResult { StatusCode = 401, Content = "token不能为空" };
                        return;
                    }
                    string refreshToken = _cache.Get(token).ToString();
                    Token newToken = TokenHelp.UpdateAccessToken(token, refreshToken);
                    //校验auth的正确性
                    var result = JWTHelp.JWTJieM(newToken.accessToken, ConfigHelp.GetString("AccessTokenKey"));
                    
                    if (result == "expired")
                    {
                        context.Result = new ContentResult { StatusCode = 401, Content = "expired" };
                        return;
                    }
                    else if (result == "invalid")
                    {
                        context.Result = new ContentResult { StatusCode = 401, Content = "invalid" };
                        return;
                    }
                    else if (result == "error")
                    {
                        context.Result = new ContentResult { StatusCode = 401, Content = "error" };
                        return;
                    }
                    else
                    {
                        //表示校验通过,用于向控制器中传值
                        context.RouteData.Values.Add("auth", result);
                    }

                }
                //else
                //{
                //    表示是非ajax请求，则auth拼接在参数中传过来
                //    context.Result = new RedirectResult("/Home/NoPerIndex?reason=null");
                //    context.Result = new ContentResult { StatusCode = 401, Content = "expired" };
                //    return;
                //}
            }

        }

        /// <summary>
        /// 判断该请求是否是ajax请求
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool IsAjaxRequest(HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }

        
    }
}
