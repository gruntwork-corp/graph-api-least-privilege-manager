using System.Xml.Linq;
using GraphApiLeastPrivilegeManagement.Tests.Utilities;
using GraphApiLeastPrivilegeManagement.Tests.Utilities.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Extensions.Hosting;
using GraphApiLeastPrivilegeManager.Services;

namespace GraphApiLeastPrivilegeManagement.Tests.Factories;

public class ApiFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// To inject your own Stubs/Mocks to substitute external services
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        // builder.ConfigureAppConfiguration(config =>
        // {
        //     config.AddJsonFile("appsettings.dev.json");
        // });
        // The following will be called after the "ConfigureServices" from the Startup.
        // Utilize this builder to inject stubs/mocks
        builder.ConfigureTestServices(services =>
        {
            services.Configure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.Configuration = new OpenIdConnectConfiguration
                    {
                        Issuer = JwtTokenProvider.Issuer,
                    };
                    options.TokenValidationParameters.ValidIssuer = JwtTokenProvider.Issuer;
                    options.TokenValidationParameters.ValidAudience = JwtTokenProvider.Issuer;
                    options.Configuration.SigningKeys.Add(JwtTokenProvider.SecurityKey);
                }
            );
            // Remove the existing registration for IGraphApiService
            // var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IGraphApiAuthenticationService));
            // if (descriptor != null)
            // {
            //     services.Remove(descriptor);
            // }
            // services.AddSingleton<IGraphApiAuthenticationService, MockGraphApiAuthenticationService>();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = builder.Build();
        Task.Run(() => host.StartAsync()).GetAwaiter().GetResult();
        return host;
    }
}