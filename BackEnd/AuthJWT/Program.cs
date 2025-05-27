using AuthJWT.Configuration;
using AuthJWT.Domain.Contracts;
using AuthJWT.Domain.Entities.Security;
using AuthJWT.Exceptions;
using AuthJWT.Extensions;
using AuthJWT.Infrastructure.Context;
using AuthJWT.Infrastructure.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddControllers();
builder.Services.AddS3Service(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthJWT", Version = "v1", Description = "AuthJWT API" });

    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please enter a valid token in the following format: Bearer {your token}"
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(o =>
{
    o.Password.RequireDigit = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequiredLength = 6;
    o.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders()
.AddTokenProvider<HotelOwnerTokenProvider>("HotelOwnerTokenProvider");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("splConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options => {
    options.TokenLifespan = TimeSpan.FromDays(7); // Token valid for 7 days
});
builder.Services.AddServices();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.ConfigureCors();
builder.Services.AddAuthorizationPolicies();

var app = builder.Build();
app.UseCors("CorsPolicy");
// await app.Seedata();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
