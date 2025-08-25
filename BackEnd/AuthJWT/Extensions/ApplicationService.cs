using System.Text;
using System.Text.Json;
using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Services.Implements;
using AuthJWT.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;


namespace AuthJWT.Extensions
{
    public static partial class ApplicationService
    {
        public static void ConfigurationS3(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AWSSetting>(configuration.GetSection("AWS"));
            services.AddSingleton<IS3Service, S3Service>();
        }
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder
                        .WithOrigins("*")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }
        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JWTSetting>();
            if(jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
            {
                throw new Exception("JwtSettings not found in appsettings.json");
            }
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>{
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = secret,
                    ValidIssuer = jwtSettings.ValidIssuer,
                    ValidAudience = jwtSettings.ValidAudience
                };
                o.Events  = new JwtBearerEvents
                {
                    OnChallenge = context => {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(new {
                            message = "You are not Authorized to access this resource please authenticate"

                        });
                        return context.Response.WriteAsync(result);
                    },
                };
            }).AddGoogle(option => {
                option.ClientId = configuration["GoogleAuthentication:Client"];
                option.ClientSecret = configuration["GoogleAuthentication:ClientSecret"];
                option.SaveTokens = true;
                option.Scope.Add("email");
                option.Scope.Add("profile");
                
            });

        }

    }
}