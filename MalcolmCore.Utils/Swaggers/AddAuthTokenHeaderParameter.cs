using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;
using System.Linq;
using MalcolmCore.Utils.Filter;


namespace MalcolmCore.Utils.Swaggers
{
    public class AddAuthTokenHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            //if (operation.Parameters == null) operation.Parameters = new List<IParameter>();
            //var attrs = context.ApiDescription.ActionDescriptor.AttributeRouteInfo;

            //先判断是否是匿名访问,
            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor != null)
            {
                var isDefined = descriptor.MethodInfo.GetCustomAttributes(inherit: true)
                   .Any(a => a.GetType().Equals(typeof(SkipAttribute)));
                //非匿名的方法,链接中添加accesstoken值
                if (!isDefined)
                {
                    //operation.Parameters.Add(new NonBodyParameter()
                    //{
                    //    Name = "Authorization",
                    //    In = "query",//query header body path formData
                    //    Type = "string",
                    //    Required = true //是否必选
                    //});
                }
            }
        }
    }
}
