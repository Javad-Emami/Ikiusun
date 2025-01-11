using Application.Interfaces;
using Application.Interfaces.Gpt_01Preview;
using Application.Interfaces.Gpt_4oMini;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Services;
using Service.Services.Gpt_01Preview;
using Service.Services.Gpt_4o;
using Service.Services.Gpt_4oMini;

namespace Service;

public static class ServiceDI
{
    public static IServiceCollection AddService(this IServiceCollection services,IConfiguration configuration) 
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IOtpService, OtpService>();
        services.AddScoped<IConversationService, ConversationService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IUserRequestService,UserRequestService>();
        services.AddScoped<IOpenAi_ChatGPT3Point5Turbo, OpenAi_ChatGPT3Point5Turbo>();
        services.AddScoped<IOpenAI_ImageModelDalle3, OpenAI_ImageModelDalle3>();
        services.AddScoped<IOpenAI_ChatGPT4oVisionCapability, OpenAI_ChatGpt4oVisionCapability>();
        services.AddScoped<IOpenAI_ChatGPT4oMiniVisionCapability,OpenAI_ChatGPT4oMiniVisionCapability>();
        services.AddScoped<IOpenAi_ChatGPT01Preview, OpenAi_ChatGPT01Preview>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IPricingService, PricingService>();
        services.AddScoped<ICostCalculationService, CostCalculationService>();
        services.AddScoped<ICurrencyExchangeRateService, CurrencyExchangeRateService>();

        return services;
    }
}
