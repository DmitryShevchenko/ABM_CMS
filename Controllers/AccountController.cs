using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ABM_CMS.Database;
using ABM_CMS.Helpers;
using ABM_CMS.Interfaces;
using ABM_CMS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using ABM_CMS.Extensions;
using Hangfire;
using Newtonsoft.Json;

namespace ABM_CMS.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppSettings _appSettings;
        private readonly IEmailSender _emailSender;
        private readonly AppDbContext _db;


        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
            IOptions<AppSettings> appSettings, IEmailSender emailSender, AppDbContext db)
        {
            _appSettings = appSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _db = db;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerViewModel)
        {
            //Hold all errors related to registration
            var errorList = new List<string>();

            var user = new IdentityUser()
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                // Conformation of email
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account",
                    new {UserId = user.Id, EmailConfirmationToken = emailConfirmationToken}, protocol: HttpContext.Request.Scheme);
                BackgroundJob.Enqueue( () => _emailSender.Send(user.Email, "Confirm Your Email",
                    @"Please confirm your e-mail by clicking this link: <a href=\" + callbackUrl + "\">click here</a>"));
                /*await _messageSender.Send(user.Email, "Confirm Your Email",
                    "Please confirm your e-mail by clicking this link: <a href=\"" + callbackUrl + "\">click here</a>");*/

                return Ok(new
                    {/*userName = user.UserName,*/ email = user.Email, status = 1, message = "Registration Successful"});
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
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginViewModel.Password))
            {
                // Conformation of email
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "User Has not Confirmed Email.");
                    return Unauthorized(new { LoginError = "We sent you an Confirmation Email. Please Confirm Your Registration With ABM.com To Log in." });
                }
                
                var roles = await _userManager.GetRolesAsync(user);

                var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));

                var tokenExpiryTime = Convert.ToDouble(_appSettings.ExpireTime);

                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, loginViewModel.Email),
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
                var tokenString = tokenHandler.WriteToken(token);

                await _userManager.SetAuthenticationTokenAsync(user, "SecurityTokenDescriptor", token.Issuer, tokenString);

                return Ok(new
                {
                    token = tokenString, expiration = token.ValidTo, email = user.Email,
                    userRole = roles.FirstOrDefault(), statusCode = StatusCode(200)
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



        [HttpGet("[action]")]
        public async Task<IActionResult> ConfirmEmail(string userid, string emailConfirmationToken)
        {
            if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(emailConfirmationToken))
            {
                ModelState.AddModelError("", "User Id and Code are Required");
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userid);

            if (user == null)
            {
                return new JsonResult("ERROR");
            }

            if (user.EmailConfirmed)
            {
                return Redirect("/login");
            }

            var result = await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);

            if (result.Succeeded)
            {
                return RedirectToAction("EmailConfirmed", "Notifications", new {userid, emailConfirmationToken});
            }
            else
            {
                var errors = result.Errors.Select(identityError => identityError.ToString());

                return new JsonResult(errors);
            }
        }
    }
}