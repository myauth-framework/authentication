using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyAuth.Authentication
{
    class AuthenticationCriticalInputParametersFilter : IAsyncActionFilter
    {
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments != null)
            {
                foreach (var indicator in context.ActionArguments.Values.OfType<IRequiredClaimsIndicator>())
                {
                    if (!indicator.RequiredClaimHeadersHasSpecified())
                    {
                        context.Result = new UnauthorizedResult();
                        return Task.CompletedTask;
                    }
                }
            }

            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var methodParameters = controllerActionDescriptor.MethodInfo.GetParameters();

                foreach (var p in controllerActionDescriptor.Parameters)
                {
                    var mParam = methodParameters.FirstOrDefault(mp => mp.Name == p.Name);
                    var reqClaimHeaderAttr = mParam?.GetCustomAttribute<RequiredClaimHeaderAttribute>();
                    
                    if (reqClaimHeaderAttr != null &&
                        !context.HttpContext.Request.Headers.ContainsKey(reqClaimHeaderAttr.Name))
                    {
                        context.Result = new UnauthorizedResult();
                        return Task.CompletedTask;
                    }
                }
            }

            return next();
        }
    }
}
