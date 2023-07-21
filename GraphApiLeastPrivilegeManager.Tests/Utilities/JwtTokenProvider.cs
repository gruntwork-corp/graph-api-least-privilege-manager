using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GraphApiLeastPrivilegeManagement.Tests.Utilities;

public static class JwtTokenProvider
{
    public static string Issuer { get; } = "integration-test-auth-server";
    public static SecurityKey SecurityKey { get; } = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom secret key for authentication"));
    public static SigningCredentials SigningCredentials { get; } = new(SecurityKey, SecurityAlgorithms.HmacSha256);
    internal static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new();
}