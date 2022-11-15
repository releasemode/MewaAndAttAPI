using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mwan.Licensing.MEWA.API.Data.Models;
using Mwan.Licensing.MEWA.API.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Mwan.Licensing.MEWA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login-user")]
        public async Task<AuthResultResponse> Login([FromBody] LoginVM loginVM)
        {
            AuthResultResponse response = new AuthResultResponse();
            if (loginVM.EmailAddress == null || loginVM.Password==null)
            {
                response.ErrorMessage = "Please, provide valid email and password";
                response.StatusCode = 400;
                response.Result = null;
                response.Success = false;

                return response;
            }

            UserInfo userInfo = new UserInfo()
            {
                UserName = _configuration["UserInfo:UserName"],
                Id = _configuration["UserInfo:Id"],
                Email = _configuration["UserInfo:Email"],
                Password = _configuration["UserInfo:Password"]
            };
            List<UserInfo> listOfUsers = _configuration.GetSection("UserInfo").Get<List<UserInfo>>();
            UserInfo userExists = await isUserExists(loginVM.EmailAddress,loginVM.Password, listOfUsers);
            if (userExists is not null)
            {
                var tokenValue = await GenerateJWTTokenAsync(userExists, null);
                response.ErrorMessage = "";
                response.StatusCode = 200;
                response.Result = tokenValue;
                response.Success = true;

                return response;
            }
            response.ErrorMessage = "UnAuthorized";
            response.StatusCode = 401;
            response.Result = null;
            response.Success = false;
          return response;
        }

        private async Task<UserInfo> isUserExists(string email,string password, List<UserInfo> users)
        {
            UserInfo userMatchingCount = users.FindAll(e => e.Email==email && e.Password == password).FirstOrDefault();
            return userMatchingCount;
            
        }

        private async Task<AuthResultVM> GenerateJWTTokenAsync(UserInfo user, RefreshToken rToken)
        {
          
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email,  user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              
            };
            string userRoles = GetRolesAsync(user.Email);
            authClaims.Add(new Claim(ClaimTypes.Role,userRoles));
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(180),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
       
            if (rToken != null)
            {
                var rTokenResponse = new AuthResultVM()
                {
                    Token = jwtToken,
                    //RefreshToken = rToken.Token,
                    ExpiresAt = token.ValidTo.AddHours(3)
                };
                return rTokenResponse;
            }

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsRevoked = false,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                DateExpire = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
            };
            var response = new AuthResultVM()
            {
                Token = jwtToken,
                //RefreshToken = refreshToken.Token,
                ExpiresAt = token.ValidTo.AddHours(3)
            };

            return response;

        }

        private string GetRolesAsync(string email)
        {
            if(email == "temp-user@mwan.gov.sa")
            {
                return "MEWA";
            }
            else
            {
                return "MWAN";
            }
        }
    }
}
