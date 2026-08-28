using KASHOP.BLL.Services;
using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authenticationService.RegisterAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authenticationService.LoginAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
        {
            //Console.WriteLine(request.Token);   
            var result = await _authenticationService.ConfirmEmail(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
