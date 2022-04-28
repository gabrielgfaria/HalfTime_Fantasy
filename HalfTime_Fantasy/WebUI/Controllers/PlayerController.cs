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
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        [HttpGet("{playerId}")]
        [Authorize(Roles = "TeamOwner,Admin")]
        public async Task<ActionResult<Player>> Get(int playerId)
        {
            var teamId = User.Claims.GetClaim<int>("TeamId");

            try
            {
                return Ok(await _playerService.GetAsync(playerId, teamId));
            }
            catch (UnauthorizedPlayerActionException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch]
        [Authorize(Roles = "TeamOwner,Admin")]
        public async Task<ActionResult<Player>> Update(UpdatePlayerRequest updatePlayerRequest)
        {
            var teamId = User.Claims.GetClaim<int>("TeamId");

            try
            {
                return Ok(await _playerService.UpdateAsync(updatePlayerRequest, teamId));
            }
            catch (UnauthorizedPlayerActionException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
