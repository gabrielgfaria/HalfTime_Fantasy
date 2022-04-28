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
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        [Authorize(Roles = "TeamOwner,Admin")]
        public async Task<ActionResult<Team>> Get()
        {
            var teamId = User.Claims.GetClaim<int>("TeamId");
            return Ok(await _teamService.GetTeamAsync(teamId));
        }

        [HttpPatch]
        [Authorize(Roles = "TeamOwner,Admin")]
        public async Task<ActionResult<Player>> Update(UpdateTeamRequest updateTeamRequest)
        {
            var teamId = User.Claims.GetClaim<int>("TeamId");
            try
            {
                return Ok(await _teamService.UpdateAsync(updateTeamRequest, teamId));
            }
            catch (UnauthorizedTeamActionException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
    