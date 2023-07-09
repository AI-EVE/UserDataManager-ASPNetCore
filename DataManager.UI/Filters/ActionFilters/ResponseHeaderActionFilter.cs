using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDProject.Filters.ActionFilters
{

    public class ResponseHeaderActionFilterFactory : Attribute, IFilterFactory
    {
        private readonly string _key;
        private readonly string _value;

        public ResponseHeaderActionFilterFactory(string key, string value)
        {
            this._key = key;
            this._value = value;
        }

        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new ResponseHeaderActionFilter((ILogger<ResponseHeaderActionFilter>)serviceProvider.GetRequiredService(typeof(ILogger<ResponseHeaderActionFilter>)), _key, _value);
        }
    }

    public class ResponseHeaderActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string _key;
        private readonly string _value;

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value)
        {
            _logger = logger;
            this._key = key;
            this._value = value;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} before", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
            
            await next();

            _logger.LogInformation("{FilterName}.{MethodName} after", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));
            context.HttpContext.Response.Headers[_key] = _value.ToString();
        }
    }
}
