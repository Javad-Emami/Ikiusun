using Application;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
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
            x.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme= CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(o =>
            {
                o.LoginPath = "/api/v1/account/login";
                o.LogoutPath = "/api/v1/account/logout";
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
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration.GetSection("JWT")["Issuer"],
                ValidAudience = configuration.GetSection("JWT")["Audience"],
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        #endregion
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Ikiusun AI Application",
                Version = "v1",
            });
            opt.AddSecurityDefinition("Bearer ", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authentication",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Please Enter Your Token with this format: 'Bearer YOUR_TOKEN'",
                Type = SecuritySchemeType.ApiKey, //Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
            opt.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                        Reference = new OpenApiReference
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = ReferenceType.SecurityScheme,
                        }
                    },
                    new List<string>()
                }
            });
        });

        services.AddControllers();
        services.AddHttpClient();

        services.AddApplication(configuration)
                .AddPersistance(configuration)
                .AddService(configuration);

        services.AddLogging();
        return services;
    }
}
