using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDProject.Filters.AuthorizationFilter
{
    public class TokenAuthorizationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Cookies.ContainsKey("Auth-Token") || context.HttpContext.Request.Cookies["Auth-Token"] != "A200")
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
