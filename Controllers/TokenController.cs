using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ABM_CMS.Database;
using ABM_CMS.Helpers;
using ABM_CMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ABM_CMS.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        //jwt and refresh tokens
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppSettings _appSettings;
        private readonly TokenModel _token;
        private readonly AppDbContext _db;

        public TokenController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings,
            TokenModel token, AppDbContext db)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _token = token;
            _db = db;
        }

        /// <summary>
        /// model.GrantType == "password" this is loginRequest
        /// model.GrantType == "refresh_token" this is refreshRequest
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Auth([FromBody] TokenRequestModel model)
        {
            //We will return Generic 500 Http Server Error 
            //If we receive an invalid payload 

            if (model == null)
            {
                return new StatusCodeResult(500);
            }

            switch (model.GrandType)
            {
                case "password":
                    return await GenerateNewToken(model);
                case "refresh_token":
                    return await RefreshToken(model);
                default:
                    // Non supported return 401(Unauthorized)
                    return new UnauthorizedResult();
            }
        }

        /// <summary>
        /// Method to Create New JWT and Refresh Token.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<IActionResult> GenerateNewToken(TokenRequestModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // If user has confirmed his Email
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "User has not Confirmed Email");
                    return Unauthorized(new { LoginError = "We sent you an Confirmation Email. Please Confirm Your Registration With ABM.com To Log in." });
                }
                
                //UserName & Password matches: create the refresh token
                var newRefreshToken = CreateRefreshToken(_appSettings.ClientId, user.Id);
                //Delete any existing old refresh_tokens
                var oldRefreshTokens = _db.Tokens.Where(rt => rt.UserId == user.Id);
                _db.Tokens.RemoveRange(oldRefreshTokens);
                await _db.SaveChangesAsync();
                
                //Create & Return access token which contains JWT and Refresh token

                var accessToken = await CreateAccessToken(user, newRefreshToken.Value);
                return Ok(new {authToken = accessToken});
            }
            else
            {
                ModelState.AddModelError(string.Empty, "UserName/Password wat not Found");
                return Unauthorized(new
                    {LoginError = "Please Check the Login Credentials - Invalid UserName/Password was entered"});
            }
        }

        private async Task<TokenResponseModel> CreateAccessToken(ApplicationUser user, string refreshToken)
        {
            var tokenExpiryTime = Convert.ToDouble(_appSettings.ExpireTime);
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim("LoggedOn", DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                }),

                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _appSettings.Site,
                Audience = _appSettings.Audience,
                Expires = DateTime.UtcNow.AddMinutes(tokenExpiryTime),
            };
             //Generate token 
             var newToken = tokenHandler.CreateToken(tokenDescriptor);
             var encodedToken = tokenHandler.WriteToken(newToken);
             
             return new TokenResponseModel()
             {
                 Token = encodedToken,
                 Expiration = newToken.ValidTo,
                 RefreshToken = refreshToken,
                 Roles = roles.FirstOrDefault(),
                 UserName = user.UserName,
             };
        }

        private TokenModel CreateRefreshToken(string clientId, string userId)
        {
            return new TokenModel()
            {
                ClientId = clientId,
                UserId = userId,
                Value = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(90),
            };
        }

        /// <summary>
        /// Method to Refresh JWT and Refresh Token.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task<IActionResult> RefreshToken(TokenRequestModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}