using BookApi.constants;
using BookApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookApi.filters
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var controller = context.RouteData.Values["controller"];

            if (controller is IBaseController baseController)
            {
                var exception = context.Exception;
                baseController.LogException(exception.Message, exception);
                var result = new { StatusCode = 500, Message = MessageConstant.ERROR_MESSAGE };
                context.Result = new JsonResult(result);
            }
        }
    }
}