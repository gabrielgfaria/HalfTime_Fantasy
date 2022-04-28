using Application.Services.Interfaces;
using Contract.Requests;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterAsync(UserAuthRequest user)
        {
            try
            {
                await _authenticationService.RegisterAsync(user);
            }
            catch (ExistingUserException ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<string> Login(UserAuthRequest user)
        {
            try
            {
                return Ok(_authenticationService.Login(user));
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
