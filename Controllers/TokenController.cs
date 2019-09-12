using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ABM_CMS.Database;
using ABM_CMS.Extensions;
using ABM_CMS.Helpers;
using ABM_CMS.Interfaces;
using ABM_CMS.Models;
using ABM_CMS.Models.Identity;
using ABM_CMS.Models.Token;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ABM_CMS.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        //jwt and refresh tokens
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenCreator _tokenCreator;
        private readonly AppSettings _appSettings;
        private readonly AppDbContext _db;

        public TokenController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings,
            AppDbContext db, ITokenCreator tokenCreator)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _db = db;
            _tokenCreator = tokenCreator;
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

            switch (model.GrantType)
            {
                case GrantType.Login:
                    return await GenerateNewToken(model);
                case GrantType.RefreshToken:
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
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    // If user has confirmed his Email
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty, "User has not Confirmed Email");
                        return Unauthorized(new
                        {
                            LoginError =
                                $"We sent you an Confirmation Email. On {user.Email}. Please Confirm Your Registration to Log in."
                        });
                    }

                    //UserName & Password matches: create the refresh token
                    var newRefreshToken = _tokenCreator.CreateRefreshToken(_appSettings.ClientId, user.Id);
                    //Delete any existing old refresh_tokens
                    var oldRefreshTokens = _db.RefreshTokens.Where(rt => rt.UserId == user.Id);
                    _db.RefreshTokens.RemoveRange(oldRefreshTokens);
                    //Add refresh token to Db.
                    _db.RefreshTokens.Add(newRefreshToken);
                    await _db.SaveChangesAsync();

                    //Create & Return access token which contains JWT and Refresh token

                    var accessToken = await _tokenCreator.CreateAccessToken(user, newRefreshToken.Value);
                    return Ok(new {authToken = accessToken});
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email/Password wat not Found");
                    return Unauthorized(new
                        {LoginError = "Please Check the Login Credentials - Invalid Email/Password was entered"});
                }
            }
            catch (Exception e)
            {
                return new UnauthorizedResult();
            }
        }

        /// <summary>
        /// Method to Refresh JWT and Refresh Token.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task<IActionResult> RefreshToken(TokenRequestModel model)
        {
            try
            {
                var rt = _db.RefreshTokens.FirstOrDefault(t =>
                    t.ClientId == _appSettings.ClientId &&
                    t.Value == model.RefreshToken);

                if (rt == null || rt.ExpiryTime < DateTime.UtcNow)
                {
                    return new UnauthorizedResult();
                }

                //Check if there`s user with the refresh token`s userId
                var user = await _userManager.FindByIdAsync(rt.UserId);

                if (user == null)
                {
                    return new UnauthorizedResult();
                }

                //Generate new Refresh token 
                var rtNew = _tokenCreator.CreateRefreshToken(rt.ClientId, rt.UserId);

                _db.RefreshTokens.Remove(rt);
                _db.RefreshTokens.Add(rtNew);
                await _db.SaveChangesAsync();


                var response = await _tokenCreator.CreateAccessToken(user, rtNew.Value);
                return Ok(new {authToken = response});
            }
            catch (Exception e)
            {
                return new UnauthorizedResult();
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> VerifyUserToken([FromBody] string token)
        {
            using (_db)
            {
                try
                {
                    var userTokenData = _db.UserTokens.First(t => t.Token == token);
                    var user = await _userManager.FindByIdAsync(userTokenData.UserId);
                    var result = await _userManager.VerifyUserTokenAsync(user, userTokenData.TokenProvider,
                        userTokenData.Purpose, userTokenData.Token);

                    if (result)
                        return Ok(new
                        {
                            UserName = user.UserName,
                            Token = userTokenData.Token
                        });

                    _db.Remove(userTokenData);
                    await _db.SaveChangesAsync();
                    return BadRequest(new
                    {
                        Error = "Invalid token.",
                        Message = "It looks like you clicked on an invalid password reset link. Please try again."
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return BadRequest(new
                    {
                        Error = "Invalid token.",
                        Message = "It looks like you clicked on an invalid password reset link. Please try again."
                    });
                }
            }
        }

        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] string googleJwtToken)
        {
            var payload =
                await GoogleJsonWebSignature.ValidateAsync(googleJwtToken,
                    new GoogleJsonWebSignature.ValidationSettings());

            using (_db)
            {
                if (!await _db.Users.AnyAsync(u => u.Email == payload.Email))
                {
                    var user = new ApplicationUser()
                    {
                        UserName = payload.Email,
                        Email = payload.Email,
                        SecurityStamp = Guid.NewGuid().ToString()
                    };
                    var result = await _userManager.CreateAsync(user);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    else
                    {
                        return BadRequest(result.Errors.Select(err => err.Description).ToList());
                    }
                }

                var dbUser = await _userManager.FindByEmailAsync(payload.Email);
                if (dbUser == null) return BadRequest();

                //UserName & Password matches: create the refresh token
                var newRefreshToken = _tokenCreator.CreateRefreshToken(_appSettings.ClientId, dbUser.Id);
                //Delete any existing old refresh_tokens
                var oldRefreshTokens = _db.RefreshTokens.Where(rt => rt.UserId == dbUser.Id);
                _db.RefreshTokens.RemoveRange(oldRefreshTokens);
                //Add refresh token to Db.
                _db.RefreshTokens.Add(newRefreshToken);
                await _db.SaveChangesAsync();

                //Create & Return access token which contains JWT and Refresh token

                var accessToken = await _tokenCreator.CreateAccessToken(dbUser, newRefreshToken.Value);
                return Ok(new {authToken = accessToken});
            }

            
        }
    }
}