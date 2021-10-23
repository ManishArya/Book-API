using System;
using System.Threading.Tasks;
using BookApi.constants;
using BookApi.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MyListController : ControllerBase
    {
        private readonly IMyListService _myListService;
        private readonly ILogger<MyListController> _logger;

        public MyListController(IMyListService myListService, ILogger<MyListController> logger)
        {
            _myListService = myListService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyList()
        {
            try
            {
                _logger.LogInformation($"{nameof(MyListController.GetMyList)} calling");

                var result = await _myListService.GetMyList();

                _logger.LogInformation($"{nameof(MyListController.GetMyList)} returning");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, MessageConstant.ERROR_MESSAGE);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddItemToMyList([FromQuery(Name = "itemId")] string itemId)
        {
            if (itemId == null)
            {
                return BadRequest();
            }

            try
            {
                if (ObjectId.TryParse(itemId, out ObjectId id))
                {
                    _logger.LogInformation($"{nameof(MyListController.AddItemToMyList)} calling");

                    var result = await _myListService.AddItemToMyList(itemId);

                    _logger.LogInformation($"{nameof(MyListController.AddItemToMyList)} returning");

                    return result == null ? NotFound() : Ok(result);
                }

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, MessageConstant.ERROR_MESSAGE);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveItemFromMyList(string itemId)
        {
            if (itemId == null)
            {
                return BadRequest();
            }

            try
            {
                _logger.LogInformation($"{nameof(MyListController.RemoveItemFromMyList)} calling");

                var result = await _myListService.RemoveItemFromMyList(itemId);

                _logger.LogInformation($"{nameof(MyListController.RemoveItemFromMyList)} returning");

                return result == null ? NotFound() : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return StatusCode(500, MessageConstant.ERROR_MESSAGE);
            }
        }

    }
}