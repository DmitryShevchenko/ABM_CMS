using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ABM_CMS.Helpers;
using ABM_CMS.Interfaces;
using ABM_CMS.Models;
using ABM_CMS.Services;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ABM_CMS.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppSettings _appSettings;
        private readonly IMessageSender _messageSender;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IOptions<AppSettings> appSettings, IMessageSender messageSender)
        {
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _messageSender = messageSender;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerViewModel)
        {
            //Hold all errors related to registration
            var errorList = new List<string>();

            var user = new IdentityUser()
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                //ADD Sending Confirmation Email
                
                return Ok(new
                    {userName = user.UserName, email = user.Email, status = 1, message = "Registration Successful"});
            }
            else
            {
                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                    errorList.Add(identityError.Description);
                }
            }

            return BadRequest(new JsonResult(errorList));
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginViewModel)
        {
            //Get the user from DB
            var user = await _userManager.FindByNameAsync(loginViewModel.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginViewModel.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);

                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));

                var tokenExpiryTime = Convert.ToDouble(_appSettings.ExpireTime);

                // ADD Conformation of email

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, loginViewModel.UserName),
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

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Ok(new
                {
                    token = tokenHandler.WriteToken(token), expiration = token.ValidTo, userName = user.UserName,
                    userRole = roles.FirstOrDefault()
                });
            }
            //return ERR
            ModelState.AddModelError("", "UserName/Password was not fount");
            return Unauthorized(new
            {
                LoginError = "Please Check the Login Credentials - Invalid UserName/Password was entered",
                statusCode = StatusCode(401)
            });
        }
    }
}