using System.Security.Claims;
using GraphApiLeastPrivilegeManager.Services;
using GraphApiLeastPrivilegeManager.Services.Groups;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

// Add services to the container.
builder.Services.AddSingleton<IGraphApiAuthenticationService, GraphApiAuthenticationService>(svc => new GraphApiAuthenticationService(
    Environment.GetEnvironmentVariable("ARM_TENANT_ID") ?? "",
    Environment.GetEnvironmentVariable("ARM_CLIENT_ID") ?? "",
    Environment.GetEnvironmentVariable("ARM_CLIENT_SECRET") ?? ""
));
builder.Services.AddSingleton<IGraphApiGroupService, GraphApiGroupService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{Environment.GetEnvironmentVariable("ARM_TENANT_ID")}/v2.0";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://login.microsoftonline.com/{Environment.GetEnvironmentVariable("ARM_TENANT_ID")}/v2.0",
            ValidateAudience = true,
            ValidAudience = Environment.GetEnvironmentVariable("ARM_CLIENT_ID")
        };
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("Group.Read", policy => policy.RequireClaim(ClaimTypes.Role, "Group.Read"));
    o.AddPolicy("Group.ReadWriteControl.OwnedBy", policy => policy.RequireClaim(ClaimTypes.Role, "Group.ReadWriteControl.OwnedBy"));
    o.AddPolicy("Group.ReadWriteOwner.OwnedBy", policy => policy.RequireClaim(ClaimTypes.Role, "Group.ReadWriteOwner.OwnedBy"));
    o.AddPolicy("Group.ReadWriteMember.OwnedBy", policy => policy.RequireClaim(ClaimTypes.Role, "Group.ReadWriteMember.OwnedBy"));
    o.AddPolicy("Group.ReadWrite.OwnedBy", policy => policy.RequireClaim(ClaimTypes.Role, new[] {"Group.ReadWrite.OwnedBy"}));
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    var identifierUri = Environment.GetEnvironmentVariable("IDENTIFIER_URI") ?? $"api://{Environment.GetEnvironmentVariable("IDENTIFIER_URI")}";
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{Environment.GetEnvironmentVariable("ARM_TENANT_ID")}/oauth2/v2.0/authorize"),
                TokenUrl = new Uri($"https://login.microsoftonline.com/{Environment.GetEnvironmentVariable("ARM_TENANT_ID")}/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    { $"{identifierUri}/OAuthToken", "OAuth access token" }
                }
            }
        }
    });

    

c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        o.OAuthClientId(Environment.GetEnvironmentVariable("ARM_CLIENT_ID"));
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

#pragma warning disable CA1050
public partial class Program { }
#pragma warning restore CA1050
