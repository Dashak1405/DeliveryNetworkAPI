using DeliveryNetworkAPI.Data;
using DeliveryNetworkAPI.Models;
using DeliveryNetworkAPI.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeliveryNetworkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly DeliveryNetworkDbContext _context;
        private readonly IOptions<AuthOptions> _options;
        public AuthenticationController(DeliveryNetworkDbContext context, IOptions<AuthOptions> authOptions)
        {
            _context = context;
            _options = authOptions;
        }

        [HttpPost]
        public IActionResult Login([FromBody] Login request)
        {
            var user = AuthenticateUser(request.username, request.password);
            if (user is null)
            {
                return NotFound("Invalid user request!");
            }
            if (user != null)
            {
                var token = GenerateJWT(user);

                return Ok(new JwtTokenResponse
                {
                    Token = token
                });
            }

            return Unauthorized();
        }

        private Users AuthenticateUser(string email, string password)
        {
            return _context.Users.Include(x => x.Person).ThenInclude(x => x.post).FirstOrDefault(u => u.Login == email && u.Password == password);
        }

        private string GenerateJWT(Users User) 
        {
            var authParams = _options.Value;

            var posts = _context.Posts.Where(x => x.ID == User.Person.post.ID).ToList();

            var securityKey = GetSymmetricSecurityKey();
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, User.Login),
                new Claim(JwtRegisteredClaimNames.Sub, User.ID.ToString())
            };

            foreach (var role in posts)
            {
                claims.Add(new Claim("role", role.Post));
            }

            var token = new JwtSecurityToken(Issuer,
                Audience,
                claims,
                expires: DateTime.Now.AddSeconds(TokenLifetime),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string Issuer = "authServer";
        private string Audience = "resourseServer";
        private string Secret = "superMegaSecretKey";
        private int TokenLifetime = 3600;
        private SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));
        }
    }
}
