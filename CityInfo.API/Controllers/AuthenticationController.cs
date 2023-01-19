using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CityInfo.API.Controllers;

public class AuthenticationRequestBody
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
}

[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthenticationController(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    [HttpPost("authenticate")]
    public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
    {
        // Step 1: validate the username/password

        var user = ValidateUserCredentials(
            authenticationRequestBody.UserName,
            authenticationRequestBody.Password);

        if (user == null)
            return Unauthorized();

        // Step 2: create a token

        var securityKey =
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));

        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>();

        claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
        claimsForToken.Add(new Claim("given_name", user.FirstName));
        claimsForToken.Add(new Claim("family_name", user.LastName));
        claimsForToken.Add(new Claim("city", user.City));

        var jwtSecurityToken = new JwtSecurityToken(
            _configuration["Authentication:Issuer"],
            _configuration["Authentication:Audience"],
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        var tokenToReturn = new JwtSecurityTokenHandler()
            .WriteToken(jwtSecurityToken);

        return Ok(tokenToReturn);
    }

    private CityInfoUser ValidateUserCredentials(string? userName, string? password)
    {
        return new CityInfoUser(
            1,
            userName ?? "",
            "Kevin",
            "Docx",
            "Antwerp");
    }


    private class CityInfoUser
    {
        public CityInfoUser(
            int userId,
            string userName,
            string firstName,
            string lastName,
            string city)
        {
            UserId = userId;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            City = city;
        }

        public int UserId { get; }
        public string UserName { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string City { get; }
    }
}
