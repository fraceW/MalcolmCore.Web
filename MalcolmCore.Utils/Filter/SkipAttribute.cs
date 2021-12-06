using System;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Text;

namespace MalcolmCore.Utils.Filter
{
    public class SkipAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
        }
    }
}
