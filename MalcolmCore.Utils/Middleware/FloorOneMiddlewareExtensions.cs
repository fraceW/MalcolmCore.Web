using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Utils.Middleware
{
    public static class FloorOneMiddlewareExtensions
    {
        public static IApplicationBuilder UseFloorOne(this IApplicationBuilder builder) 
        {
            return builder.UseMiddleware<FloorOneMiddleware>();
        }
    }
}
