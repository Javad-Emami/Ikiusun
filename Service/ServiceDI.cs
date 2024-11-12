using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Services;

namespace Service;

public static class ServiceDI
{
    public static IServiceCollection AddService(this IServiceCollection services,IConfiguration configuration) 
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IOpenAi_ChatGPT, OpenAi_ChatGpt>();

        return services;
    }
}
