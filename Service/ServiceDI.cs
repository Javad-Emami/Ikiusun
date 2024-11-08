using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Service
{
    public static class ServiceDI
    {
        public static IServiceCollection AddService(this IServiceCollection services,IConfiguration configuration) 
        {
            return services;
        }
    }
}
