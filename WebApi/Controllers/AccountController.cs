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

namespace WebApi.Controllers;

public class AccountController : ApiBaseController
{
    private readonly string _jwtKey;
    public AccountController(IConfiguration configuration)
    {
        _jwtKey = configuration.GetSection("Jwt")["Key"];
    }

    [HttpPost("registration")]
    public async Task<IActionResult> RegistrationOrSignIn([FromBody] string mobileNumber, CancellationToken cancellationToken)
    {
        var userIsExist = await Mediator.Send(new GetUserQuery(mobileNumber),cancellationToken);
        //if (userIsExist) 
        return RedirectToRoute("DefaultApi", new { controller = "Account", action = "Login" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
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

            return Ok(token);
        }

        return Unauthorized();
    }

    private string GenerateJwtToken(UserDto dto)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, dto.Mobile),
                //new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
