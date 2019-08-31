using System.Threading;
using System.Threading.Tasks;
using ABM_CMS.Interfaces;
using ABM_CMS.Models.Identity;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ABM_CMS.Controllers
{
   public class CustonToken
    {
        public string PassToken { get; set; }
        public string EmailToken { get; set; }
    }
    
    [Route("api/[controller]")]
    public class Test : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public Test(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CreateToken()
        {
            var userId = "c7f52af7-f29b-4d7a-bf01-061afb4ca7f1";
            var user = await _userManager.FindByIdAsync(userId);
            var passToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            
            BackgroundJob.Enqueue(() =>
                _emailSender.Send(user.Email, "tokenTest", $"EmailToken: {emailToken} , PassToken: {passToken}"));

            return Ok(new {passToken = passToken , emailToken = emailToken});
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> VerifyToken([FromBody] CustonToken model, string token)
        {
            var userId = "c7f52af7-f29b-4d7a-bf01-061afb4ca7f1";
            var user = await _userManager.FindByIdAsync(userId);
            bool isTokenSame = model.PassToken == token;
            var verifyUrl = await _userManager.VerifyUserTokenAsync(user, this._userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
            var verify = await _userManager.VerifyUserTokenAsync(user, this._userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", model.PassToken);
            var verify2 = await _userManager.VerifyUserTokenAsync(user, this._userManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", model.EmailToken);
            
            
            return Ok(new {passToken = verify , emailToken = verify2, UrlToken = verifyUrl, isTokenSame = isTokenSame});
            
        }
    }
}