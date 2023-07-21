using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GraphApiLeastPrivilegeManagement.Tests.Utilities;

public class TestJwtToken
{
    public List<Claim> Claims { get; } = new();
    public int ExpiresInMinutes { get; set; } = 30;

    public TestJwtToken WithRole(string roleName)
    {
        Claims.Add(new Claim(ClaimTypes.Role, roleName));
        return this;
    }

    public TestJwtToken WithUpn(string username)
    {
        Claims.Add(new Claim(ClaimTypes.Upn, username));
        return this;
    }
    
    public TestJwtToken WithCustom(string key, string value)
    {
        Claims.Add(new Claim(type: key, value: value));
        return this;
    }

    public TestJwtToken WithEmail(string email)
    {
        Claims.Add(new Claim(ClaimTypes.Email, email));
        return this;
    }

    public TestJwtToken WithExpiration(int expiresInMinutes)
    {
        ExpiresInMinutes = expiresInMinutes;
        return this;
    }

    public string Build()
    {
        var token = new JwtSecurityToken(
            JwtTokenProvider.Issuer,
            JwtTokenProvider.Issuer,
            Claims,
            expires: DateTime.Now.AddMinutes(ExpiresInMinutes),
            signingCredentials: JwtTokenProvider.SigningCredentials
        );
        return JwtTokenProvider.JwtSecurityTokenHandler.WriteToken(token);
    }
}