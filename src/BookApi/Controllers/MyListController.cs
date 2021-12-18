using System.Net;
using System.Threading.Tasks;
using BookApi.filters;
using BookApi.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookApi.Controllers
{
    [Route("api/MyList")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [LoggingFilter]
    public class MyListController : BaseController
    {
        private readonly IMyListService _myListService;

        public MyListController(IMyListService myListService, ILogger<MyListController> logger) : base(logger)
        {
            _myListService = myListService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyList()
        {
            var result = await _myListService.GetMyList();

            return ToSendResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemToMyList([FromQuery(Name = "itemId")] string itemId)
        {
            if (itemId == null)
            {
                return BadRequest();
            }

            var result = await _myListService.AddItemToMyList(itemId);
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

        [HttpGet("checckiteminmylist")]
        public async Task<IActionResult> CheckItemInMyList(string itemId)
        {
            if (itemId == null)
            {
                return BadRequest();
            }

            return ToSendResponse(await _myListService.CheckItemInMyList(itemId));
        }
    }
}