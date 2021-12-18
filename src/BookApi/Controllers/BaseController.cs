using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using BookApi.extensions;
using BookApi.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            _logger.LogError(ex, $"{ControllerContext.ActionDescriptor.ControllerName}.{ControllerContext.ActionDescriptor.ActionName} {message}");
        }

        void IBaseController.LogInformation(string message)
        {
            _logger.LogInformation($"{ControllerContext.ActionDescriptor.ControllerName}.{ControllerContext.ActionDescriptor.ActionName} {message}");
        }

        protected IActionResult ToSendResponse(BaseResponse baseResponse)
        {
            var statusCode = baseResponse.StatusCode;
            return StatusCode(statusCode, baseResponse);
        }

        protected IActionResult ToSendResponse(ModelStateDictionary modelState)
        {
            var validationErrors = modelState.Where(ms => ms.Value.Errors.Count >= 1)
                                         .Select(ms => new { Key = ms.Key, Message = ms.Value.Errors[0].ErrorMessage }).
                                         ToDictionary(ms => ms.Key.ToLowerFirstCharacter(), ms => ms.Message);

            var responseApi = new ResponseApi<Dictionary<string, string>>(validationErrors, HttpStatusCode.BadRequest);

            return ToSendResponse(responseApi);
        }
    }
}