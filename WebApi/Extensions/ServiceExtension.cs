using Application;
using Microsoft.OpenApi.Models;
using Persistance;
using Service;

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
        services.AddCors(o => o.AddPolicy("CoresPolicy", builder =>
        {
            builder.AllowAnyOrigin()
                    .WithOrigins()
                    .AllowCredentials()
                    .WithMethods("Get", "Post", "Put","Delete")
                    .AllowAnyHeader();
        }));
        #endregion

        #region Authentication & Authorization

        #endregion

        services.AddControllers();

        services.AddApplication(configuration)
                .AddPersistance(configuration)
                .AddService(configuration);

        services.AddLogging();
        return services;
    }
}
