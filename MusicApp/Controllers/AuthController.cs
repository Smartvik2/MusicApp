using Microsoft.AspNetCore.Mvc;
using MusicApp.DTO;
using MusicApp.Interfaces;
using MusicApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace MusicApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(IAuthService authService, SignInManager<ApplicationUser> signInManager)
        {
            _authService = authService;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result != "success")
                return BadRequest(new { Error = result});

            return Ok(new {Message = "User registered succcessfully!"});
           

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.Success)
            {
                if (result.Message == "Invalid Email")
                    return BadRequest(new { message = "Please use a valid email" });

                if (result.Message == "Invalid Password")
                    return BadRequest(new { message = "Please use your password" });

                return Unauthorized(new { message = result.Message ?? "Login failed" });
            }

            return Ok(new {token = result.Token});
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            // Keeping it simple, front-end will handle token removal

            return Ok(new { message = "Logged out successfully" });
        }


    }
}
