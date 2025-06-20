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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, SignInManager<ApplicationUser> signInManager, ILogger<AuthController> logger)
        {
            _authService = authService;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var result = await _authService.RegisterAsync(dto);

                if (result != "success")
                    return BadRequest(new { error = result });

                return Ok(new { message = "User registered successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registeration failed");
                return StatusCode(500, new { message = "Internal server error!" });
            }

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);

                if (!result.Success)
                {
                    return result.Message switch
                    {
                        "Invalid Email" => BadRequest(new { message = "Please use a valid email" }),
                        "Invalid Password" => BadRequest(new { message = "Please use your correct password" }),
                        _ => Unauthorized(new { message = result.Message ?? "Login failed" })
                    };
                }

                return Ok(new { token = result.Token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Login failed", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            // Keeping it simple.
            // This is stateless JWT auth — logout just means front-end should discard the token
            return Ok(new { message = "Logged out successfully" });
        }


    }
}
