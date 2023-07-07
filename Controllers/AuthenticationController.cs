using CustomerCore.Contract;
using CustomerCore.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CustomerWeb.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IJwtAuthManager authManager;
       
        public AuthenticationController(IUserRepository _userRepository, IJwtAuthManager 
            _authManager)
        {
            userRepository = _userRepository;
            authManager = _authManager;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest("Login credentials invalid");

            var user = await userRepository.GetByUserNameAsync(loginRequest.UserName);

            if (user == null)
                return NotFound("No user found");

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                new Claim(ClaimTypes.Name,user.UserName)
            };

            var jwtAuthResponse = authManager.GenerateToken(claims);

            return Ok(new LoginResponse
            {
                token_type="Bearer",
                access_token=jwtAuthResponse.AccessToken
            });
        }
    }
}
