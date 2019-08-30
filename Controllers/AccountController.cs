using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
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
using ABM_CMS.Models.Account;
using ABM_CMS.Models.Identity;
using ABM_CMS.Models.Password;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Twilio.TwiML.Messaging;

namespace ABM_CMS.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppSettings _appSettings;
        private readonly IEmailSender _emailSender;
        private readonly AppDbContext _db;


        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
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

            var user = new ApplicationUser()
            {
                Email = registerViewModel.Email,
                UserName = registerViewModel.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            //Auto check for already taken Email and UserName
            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                // Conformation of email
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action("ConfirmEmail", "Account",
                    new {UserId = user.Id, EmailConfirmationToken = emailConfirmationToken},
                    protocol: HttpContext.Request.Scheme);

                //Hangfire Job Email Send
                BackgroundJob.Enqueue(() => _emailSender.Send(user.Email, "Confirm Your Email",
                    $"Please confirm your e-mail by clicking this link: <a href={callbackUrl}>click here</a>"));
                /*await _messageSender.Send(user.Email, "Confirm Your Email",
                    "Please confirm your e-mail by clicking this link: <a href=\"" + callbackUrl + "\">click here</a>");*/

                return Ok(new
                    {userName = user.UserName, email = user.Email, status = 1, message = "Registration Successful"});
            }
            
            errorList.AddRange(result.Errors.Select(err => err.Description));

            return BadRequest(new JsonResult(errorList));
        }


        [HttpPost("[action]")]
        [Authorize(Policy = "RequireLoggedId")]
        public async Task<IActionResult> PasswordChange([FromBody] PasswordChangeModel model)
        {
            var errorList = new List<string>();
            if (model == null) return new StatusCodeResult(500);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return new StatusCodeResult(500);

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new {Message = "Password was Changed"});
            }

            errorList.AddRange(result.Errors.Select(err => err.Description));

            return BadRequest(errorList);
        }
        

        //IN Use result
        /*[HttpPost("[action]")]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest(new {ResetPasswordError = "User Email are Required"});
            
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return BadRequest(new {ResetPasswordError = $"User with Email: {email} are not Found"});

            //var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callBackUrl = Url.Action("ResetPasswordView", "Notifications", new {UserId = user.Id});
            BackgroundJob.Enqueue(() => _emailSender.Send(user.Email, "Confirm password change",
                $"Please confirm your e-mail by clicking this link: <a href={callBackUrl}>click here</a>"));
            
            return Ok();
        }*/
        
        //Test
        [HttpGet("[action]")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest(new {ResetPasswordError = "User Email are Required"});
            
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return BadRequest(new {ResetPasswordError = $"User with Email: {email} are not Found"});

            //var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callBackUrl = Url.Action("ResetPasswordView", "Notifications", new {UserId = user.Id}, protocol: HttpContext.Request.Scheme);
            BackgroundJob.Enqueue(() => _emailSender.Send(user.Email, "Confirm password change",
                $"Please confirm your e-mail by clicking this link: <a href={callBackUrl}>click here</a>"));
            
            return Ok();
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> ConfirmEmail(string userid, string emailConfirmationToken)
        {
            if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(emailConfirmationToken))
            {
                ModelState.AddModelError(string.Empty, "User Id and Code are Required");
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