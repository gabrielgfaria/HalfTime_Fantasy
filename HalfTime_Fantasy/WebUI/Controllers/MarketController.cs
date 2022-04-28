using Application.Services.Interfaces;
using Contract.Requests;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebUI.Common;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        private readonly IMarketService _marketService;

        public MarketController(IMarketService marketService)
        {
            _marketService = marketService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Transfer>>> GetAsync()
        {
            return Ok(await _marketService.GetTransferListAsync());
        }

        [HttpPost("sell")]
        [Authorize(Roles = "TeamOwner,Admin")]
        public async Task<ActionResult<Transfer>> SellPlayerAsync(SellPlayerRequest sellPlayerRequest)
        {
            var teamId = User.Claims.GetClaim<int>("TeamId");

            try
            {
                return Ok(await _marketService.SellPlayerAsync(sellPlayerRequest, teamId));
            }
            catch (UnauthorizedPlayerActionException ex)
            {
                return BadRequest(ex.Message);
            }  
        }

        [HttpPost("buy")]
        [Authorize(Roles = "TeamOwner,Admin")]
        public async Task<ActionResult<Player>> BuyPlayerAsync(BuyPlayerRequest buyPlayerRequest)
        {
            var teamId = User.Claims.GetClaim<int>("TeamId");

            try
            {
                return Ok(await _marketService.BuyPlayerAsync(buyPlayerRequest, teamId));
            }
            catch (UnauthorizedPlayerActionException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
