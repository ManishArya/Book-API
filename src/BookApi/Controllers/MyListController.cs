using System;
using System.Threading.Tasks;
using BookApi.filters;
using BookApi.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookApi.models;

namespace BookApi.Controllers
{
    [Route("api/MyList")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [LoggingFilter]
    public class MyListController : BaseController
    {
        private readonly IMyListService _myListService;

        public MyListController(IMyListService myListService, ILogger<MyListController> logger) : base(logger) => _myListService = myListService;

        [HttpGet]
        public async Task<IActionResult> GetMyList()
        {
            var result = await _myListService.GetMyList();
            return ToSendResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemToMyList(MyList myList)
        {
            if (myList == null)
            {
                return BadRequest();
            }

            var result = await _myListService.AddItemToMyList(myList);
            return ToSendResponse(result);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveItemFromMyList(string itemId)
        {
            if (itemId == null)
            {
                return BadRequest();
            }

            var result = await _myListService.RemoveItemFromMyList(itemId);
            return ToSendResponse(result);
        }

        [HttpGet("checkitem")]
        [Obsolete]
        public async Task<IActionResult> CheckItem(string itemId)
        {
            if (itemId == null)
            {
                return BadRequest();
            }

            return ToSendResponse(await _myListService.CheckItem(itemId));
        }

        [HttpGet("counts")]
        public async Task<IActionResult> GetListCounts() => ToSendResponse(await _myListService.GetListCounts());

        [HttpDelete("removeAll")]
        public async Task<IActionResult> RemoveAllItems() => ToSendResponse(await _myListService.RemoveAllItems());
    }
}