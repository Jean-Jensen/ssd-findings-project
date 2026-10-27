using System.Security.Claims;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    public const string ControllerRoute = "api/auth/";
    public const string LoginRoute = ControllerRoute + nameof(Login);
    public const string CallbackRoute = ControllerRoute + nameof(Callback);
    public const string UserInfoRoute = ControllerRoute + "UserInfo";

    
    [HttpGet(LoginRoute)]
    public async Task Login([FromServices] IConfiguration config)
    {
        await HttpContext.ChallengeAsync(
          OpenIdConnectDefaults.AuthenticationScheme,
          new AuthenticationProperties
          {
              RedirectUri = config.GetSection("OpenIDConnectSettings")["Frontend"]
          });
    }
    
    [HttpGet(CallbackRoute)]
    public async Task Callback()
    {
        await HttpContext.AuthenticateAsync();
    }

    [HttpGet(UserInfoRoute)]
    public ActionResult<UserModel> GetUserInfo()
    {
        var user = this.HttpContext.User;

        if (user?.Identity != null && user.Identity.IsAuthenticated)
        {
            return this.Ok(new UserModel
            {
                IsAuthenticated = true,
                Name = user.FindFirst("name")?.Value,
                EmailAddress = user.FindFirst(c => c.Type == ClaimTypes.Email)?.Value,
                AdditionalClaims = user.Identities.First().Claims.ToDictionary(claim => claim.Type, claim => claim.Value)
            });
        }

        return this.Ok(new UserModel
        {
            IsAuthenticated = false,
        });
    }
    
}