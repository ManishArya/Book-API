using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BookApi.Controllers
{
    public abstract class BaseController : ControllerBase, IBaseController
    {
        protected readonly ILogger<BaseController> _logger;
        protected BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;

        }

        void IBaseController.LogException(string message, Exception ex)
        {
            _logger.LogError($"{ControllerContext.ActionDescriptor.ControllerName}.{ControllerContext.ActionDescriptor.ActionName} {message}", ex);
        }

        void IBaseController.LogInformation(string message)
        {
            _logger.LogInformation($"{ControllerContext.ActionDescriptor.ControllerName}.{ControllerContext.ActionDescriptor.ActionName} {message}");
        }
    }
}