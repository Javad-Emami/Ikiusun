using Application.Features.Account.Dto;
using Application.Features.Account.Query;
using Application.Features.User.Queries;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Common;

namespace WebApi.Controllers;

public class accountController : ApiBaseController
{
    private readonly string _jwtKey;
    private readonly string _issuer;
    private readonly string _audience;
    public accountController(IConfiguration configuration)
    {
        _jwtKey = configuration.GetSection("Jwt")["Key"];
        _issuer = configuration.GetSection("Jwt")["Issuer"];
        _audience = configuration.GetSection("Jwt")["Audience"];
    }

    [HttpPost("registration")]
    public async Task<IActionResult> RegistrationOrSignIn([FromBody] string mobileNumber, CancellationToken cancellationToken)
    {
        var userIsExist = await Mediator.Send(new GetUserQuery(mobileNumber),cancellationToken);        
        return Ok(userIsExist);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResult<string>>> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new LoginUserQuery(dto), cancellationToken);
        var token = GenerateJwtToken(result);
        if (token != null)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, result.Mobile),
    };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return new ApiResult<string>(token);
        }

        return Unauthorized();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok("Logged out");
    }

    private string GenerateJwtToken(UserDto dto)
    {
        var claims = new[]
        {
            new Claim("mobile", dto.Mobile),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
