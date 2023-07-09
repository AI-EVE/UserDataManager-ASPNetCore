using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDProject.Filters.ResourceFilters
{
    public class DisableResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<DisableResourceFilter> _logger;
        private readonly bool _disable;

        public DisableResourceFilter(ILogger<DisableResourceFilter> logger, bool disable = true)
        {
            _logger = logger;
            this._disable = disable;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} before", nameof(DisableResourceFilter), nameof(OnResourceExecutionAsync));

            if(_disable)
                context.Result = new StatusCodeResult(501);
            else
                await next();            
        }
    }
}
