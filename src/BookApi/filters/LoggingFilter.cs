using System;
using System.Net;
using System.Security;
using BookApi.constants;
using BookApi.Controllers;
using BookApi.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookApi.filters
{
    public class LoggingFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is IBaseController baseController)
            {
                if (context.Exception != null)
                {
                    baseController.LogException(context.Exception.Message, context.Exception);
                    string message = string.Empty;

                    if (context.Exception is InvalidOperationException exception)
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        message = exception.Message;
                    }
                    else if (context.Exception is SecurityException securityException)
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        message = securityException.Message;
                    }
                    else
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        message = MessageConstant.ERROR_MESSAGE;
                    }

                    context.Result = new JsonResult(new BaseResponse(message, (HttpStatusCode)context.HttpContext.Response.StatusCode));
                    context.ExceptionHandled = true;
                }
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