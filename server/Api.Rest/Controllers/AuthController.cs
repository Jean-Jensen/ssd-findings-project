using Api.Rest.AuthExtensions;
using Application.Interfaces;
using Application.Models.Dtos.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace Api.Rest.Controllers;

[ApiController]
//[RequiredScope(RequiredScopesConfigurationKey = "OpenIDConnectSettings")]
public class AuthController(ISecurityService securityService) : ControllerBase
{
    public const string ControllerRoute = "api/auth/";
    public const string LoginRoute = ControllerRoute + nameof(Login);
    public const string RegisterRoute = ControllerRoute + nameof(Register);
    public const string SecuredRoute = ControllerRoute + nameof(Secured);

    
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

    [HttpPost]
    [Route(RegisterRoute)]
    public ActionResult<AuthResponseDto> Register([FromBody] AuthRequestDto dto)
    {
        return Ok(securityService.Register(dto));
    }

    [HttpGet]
    [Route(SecuredRoute)]
    public ActionResult Secured()
    {
        securityService.VerifyJwtOrThrow(HttpContext.GetJwt());
        return Ok("You are authorized to see this message");
    }
}