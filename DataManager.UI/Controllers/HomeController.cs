using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CRUDProject.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [Route("/home/error")]
        public IActionResult Error()
        {

            IExceptionHandlerPathFeature? exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            if(exceptionHandlerPathFeature != null)
            {
                Exception exception = exceptionHandlerPathFeature.Error;

                if(exception != null)
                {
                    if(exception.InnerException != null)
                    {
                        ViewBag.ExceptionType = exception.InnerException.GetType().ToString();
                        ViewBag.ExceptionMessage = exception.InnerException.Message;
                    }
                    else
                    {
                        ViewBag.ExceptionType = exception.GetType().ToString();
                        ViewBag.ExceptionMessage = exception.Message;
                    }
                }
            }

            return View();
        }
    }
}
