using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ABM_CMS.Helpers;
using ABM_CMS.Interfaces;
using ABM_CMS.Models.Identity;
using ABM_CMS.Models.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace ABM_CMS.Services.Token
{
    public class TokenCreator : ITokenCreator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppSettings _appSettings;

        public TokenCreator(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        public async Task<TokenResponseModel> CreateAccessToken(ApplicationUser user, string refreshToken)
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
                Email = user.Email,
            };
        }

        public RefreshTokenModel CreateRefreshToken(string clientId, string userId)
        {
            return new RefreshTokenModel()
            {
                ClientId = clientId,
                UserId = userId,
                Value = Guid.NewGuid().ToString("N"),
                CreatedDate = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(90),
            };
        }
    }
}