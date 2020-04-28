using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MyAuth.Authentication;

namespace TestServer.Models
{
    [ModelBinder(typeof(Binder))]
    public class RequiredClaimsObject : IRequiredClaimsIndicator
    {
        public string UserId { get; set; }
        public string AccountId { get; set; }

        public class Binder : IModelBinder
        {
            public Task BindModelAsync(ModelBindingContext bindingContext)
            {
                var headers = bindingContext.HttpContext.Request.Headers;

                bindingContext.Result = ModelBindingResult.Success(new RequiredClaimsObject
                {
                    UserId = headers["X-Claim-User-Id"],
                    AccountId = headers["X-Claim-Account-Id"]
                });

                return Task.CompletedTask;
            }
        }

        public bool RequiredClaimHeadersHasSpecified()
        {
            return !string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(AccountId);
        }
    }
}
