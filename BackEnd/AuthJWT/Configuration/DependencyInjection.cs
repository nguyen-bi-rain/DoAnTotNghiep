using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.DTOs;
using AuthJWT.Repositories;
using AuthJWT.Services;
using AuthJWT.Services.Implements;
using AuthJWT.Services.Interfaces;
using AuthJWT.UnitOfWorks;

namespace AuthJWT.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserIdentity, UserIdentity>();
            services.AddScoped<IHotelService, HotelService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IHotelImageService, HotelImageService>();
            services.AddScoped<IRoomImageService, RoomImageService>();
            services.AddScoped<IHotelOwnerService, HotelOwnerService>();
            services.AddTransient<ISendEmailService, SendEmailService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IRoomConveniceService, RoomConvenienceService>();
            services.AddScoped<IRoomTypeService, RoomTypeService>();
            services.AddScoped<IConvenienceService, ConvenienceService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IDashboardService, DashboardService>();
            
            return services;
        }
    }

    public static class AuthorizationPolicyConfig
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("HotelOwner", policy => policy.RequireRole("HotelOwner"));
                options.AddPolicy("User", policy => policy.RequireRole("User"));
            });
            return services;
        }
    }
    public static class ServicesConfig
    {
        public static IServiceCollection AddServicesConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSetting>(configuration.GetSection("MailSettings"));
            services.AddTransient<ISendEmailService, SendEmailService>();

            return services;
        }
    }
    public static class S3ServiceConfig
    {
        public static IServiceCollection AddS3Service(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AWSSetting>(configuration.GetSection("AWS"));
            services.AddSingleton<IS3Service, S3Service>();
            return services;
        }
    }

}