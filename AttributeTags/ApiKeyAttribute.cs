using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Notes.Models;
using Notes.Repository;

namespace Notes.AttributeTags
{
    [AttributeUsage(validOn:AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {

        public Roles[] AllowedRoles { get; set; }
        public ApiKeyAttribute(params Roles[] roles)
        {
            AllowedRoles = roles;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(!context.HttpContext.Request.Headers.TryGetValue("apiKey",out var key)) 
            {
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    Content = "No API key provided with request."
                };
                return;
            }

            var validKey = key.ToString().Trim('{', '}');
            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

            if (userRepository.AuthenticateUser(validKey, AllowedRoles) == false)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    Content = "The provided API key is invalid or does not have the required persmissions"
                };
                return;
            }
            userRepository.UpdateLastLogin(validKey);
            await next();
        }
    }
}