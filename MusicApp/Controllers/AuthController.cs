using Microsoft.AspNetCore.Mvc;
using MusicApp.DTO;
using MusicApp.Interfaces;



namespace MusicApp.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        //private readonly ApplicationDbContext _context;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
            //_context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register (RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result != "User registered successfully")
                return BadRequest(result);

            return Ok(result);
           

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null )
                return Unauthorized(token);
            if (token == "Invalid Email")
                return BadRequest(new { message = "Please use a valid email" });
            if (token == "Invalid Password")
                return BadRequest(new { message = "Please use your password" });

            return Ok(new {token});
        }

       
    }
}
