//Purpose: Contains the extension method to add authentication services and JWT configuration to the specified <see cref="IServiceCollection"/>.

using System.Text;
using CustomEd.Shared.JWT;
using CustomEd.Shared.JWT.Interfaces;
using CustomEd.Shared.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Adds authentication services and JWT configuration to the specified <see cref="IServiceCollection"/>.
/// </summary>
/// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
/// <returns>The modified <see cref="IServiceCollection"/>.</returns>
namespace Board.User.Service.Settings;

public static class Extensions
{
    private static JWTSettings? _jwtSettings;

    public static IServiceCollection AddAuth(this IServiceCollection services)
    {

        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
        services.Configure<JWTSettings>(configuration!.GetSection(nameof(JWTSettings)));

        services.AddScoped<IJwtService, JwtService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {

                _jwtSettings = configuration!.GetSection(nameof(JWTSettings)).Get<JWTSettings>()!;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                    ValidateIssuerSigningKey = true,
                };
            });
        return services;
    }
}