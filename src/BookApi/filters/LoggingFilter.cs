using BookApi.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookApi.filters
{
    public class LoggingFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is IBaseController baseController)
            {
                baseController.LogInformation("returning");
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is IBaseController baseController)
            {
                baseController.LogInformation($"beginning {context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString}");
            }
        }
    }
}