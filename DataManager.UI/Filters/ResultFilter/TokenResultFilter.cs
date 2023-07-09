using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDProject.Filters.ResultFilter
{
    public class TokenResultFilter : IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Cookies.Append("Auth-Token", "A200");

        }
    }
}
