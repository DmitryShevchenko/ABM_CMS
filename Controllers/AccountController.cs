using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABM_CMS.Database;
using ABM_CMS.Helpers;
using ABM_CMS.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ABM_CMS.Models.Account;
using ABM_CMS.Models.Identity;
using ABM_CMS.Models.Password;
using ABM_CMS.Models.Token;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


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
                {
                    userName = user.UserName, email = user.Email, status = 1, message = "Registration Successful",
                    emailConfirmationToken = emailConfirmationToken,
                });
            }

            errorList.AddRange(result.Errors.Select(err => err.Description));

            return BadRequest(new JsonResult(errorList));
        }


        [HttpPost("[action]")]
        [Authorize(Policy = "RequireLoggedId")]
        public async Task<IActionResult> PasswordChange([FromBody] PasswordChangeModel model)
        {
            if (model == null) return new StatusCodeResult(500);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return new StatusCodeResult(500);

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new {Message = "Password was Changed"});
            }
            
            return BadRequest(result.Errors.Select(err => err.Description).ToList());
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest(new {Error = "User Email are Required"});

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return BadRequest(new {Error = $"User with Email: {email} are not Found"});

            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            _db.UserTokens.Add(new UserToken()
            {
                User = user, TokenProvider = _userManager.Options.Tokens.PasswordResetTokenProvider,
                Purpose = "ResetPassword", Token = resetPasswordToken
            });
            await _db.SaveChangesAsync();
            var callBackUrl = Url.Action("ResetPasswordView", "Notifications", new {token = resetPasswordToken},
                protocol: HttpContext.Request.Scheme);
            BackgroundJob.Enqueue(() => _emailSender.Send(user.Email, "Confirm password change",
                $"Please confirm your e-mail by clicking this link: <a href={callBackUrl}>click here</a>"));

            return Ok(new
            {
                OkResult =
                    "Check your email for a link to reset your password. If it doesn’t appear within a few minutes, check your spam folder."
            });
        }

        /*[HttpPost("ResetPasswordConfirm")]
        public async Task<IActionResult> ResetPassword([FromBody] string userId, string token, string password)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(password)) return new StatusCodeResult(500);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return BadRequest(new {Error = $"User with Email: {userId} are not Found"});

            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (result.Succeeded)
            {
                return Ok(new {OkResult = "New password set successfully."});
            }

            var errorList = new List<string>();
            errorList.AddRange(result.Errors.Select(err => err.Description));
            return BadRequest(errorList);
        }*/
        /*[HttpPost("ResetPasswordConfirm")]*/
        [HttpPost("[action]")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            using (_db)
            {
                var userToken = await _db.UserTokens.FirstAsync(t => t.Token == model.Token);
                var user = await _userManager.FindByIdAsync(userToken.UserId);

                if (user == null) return BadRequest(new {Error = $"User with Email: {userToken.UserId} are not Found"});

                var result = await _userManager.ResetPasswordAsync(user, userToken.Token, model.Password);
                if (result.Succeeded)
                {
                    _db.UserTokens.Remove(userToken);
                    await _db.SaveChangesAsync();
                    return Ok(new {OkResult = "New password set successfully."});
                }

                var errorList = new List<string>();
                errorList.AddRange(result.Errors.Select(err => err.Description));
                return BadRequest(errorList);
            }
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