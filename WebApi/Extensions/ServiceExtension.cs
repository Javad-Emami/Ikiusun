using Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistance;
using Service;
using System.Text;

namespace WebApi.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection Config(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Ikiusun AI Application",
                Version = "v1",
            });
        });

        #region CoresPolicy
        //services.AddCors(o => o.AddPolicy("CoresPolicy", builder =>
        //{
        //    builder.AllowAnyOrigin()
        //            .WithOrigins()
        //            //.AllowCredentials()
        //            .WithMethods("Get", "Post", "Put","Delete")
        //            .AllowAnyHeader();
        //}));
        #endregion

        #region Authentication & Authorization
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddCookie(o =>
            {
                o.LoginPath = "/api/v1/account/login";
                o.ExpireTimeSpan = TimeSpan.FromDays(30);
                o.SlidingExpiration = true;
            })
        .AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;
            o.Events = new JwtBearerEvents();
            o.Events.OnTokenValidated = context =>
            {
                context.Response.StatusCode = 200;
                return Task.CompletedTask;
            };
            o.Events.OnAuthenticationFailed = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
            o.Events.OnChallenge = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
            o.Events.OnMessageReceived = context =>
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            };
            var key = Encoding.UTF8.GetBytes(configuration.GetSection("JWT")["Key"]);
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,               
                ValidIssuer = configuration.GetSection("JWT")["Key"],
                ClockSkew = TimeSpan.Zero,
                //ValidAudience = builder.Configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        #endregion

        services.AddControllers();
        services.AddHttpClient();

        services.AddApplication(configuration)
                .AddPersistance(configuration)
                .AddService(configuration);

        services.AddLogging();
        return services;
    }
}
