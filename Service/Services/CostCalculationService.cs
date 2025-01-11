using Application.Features.CostCalculationService.Dto;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Service.Services;

public class CostCalculationService : ICostCalculationService
{
    private readonly IPricingService _pricingService;
    private readonly ICurrencyExchangeRateService _currencyExchangeRateService;
    public CostCalculationService(IPricingService pricingService, ICurrencyExchangeRateService currencyExchangeRateService)
    {
        _pricingService = pricingService;
        _currencyExchangeRateService = currencyExchangeRateService;
    }
    public async Task<CostCalculationDto> ChatModelCostCalculationAsync(ServiceModelEnum model, int inputToken, int outPutToken, CancellationToken cancellationToken)
    {
        var pricingParameter = await _pricingService.GetAsync(a => a.ServiceModelId == (int)model, cancellationToken);
        var exchangeRate = await _currencyExchangeRateService.ListAsync(cancellationToken);

        var cost = ((inputToken * pricingParameter.InputTokenCost) / 1000000 + (outPutToken * pricingParameter.OutputTokenCost) / 1000000) * 
                   exchangeRate.Select(er => er.CurrencyExchangeRate1).LastOrDefault();

        return new CostCalculationDto() { PricingId = pricingParameter.Id, CostUsage = (decimal)cost };
    }

    public async Task<CostCalculationDto> ImageModelCostCalculationAsync(ServiceModelEnum model, int imageResolution, 
                                                              int imageQuality, CancellationToken cancellationToken)
    {
        var pricingParameter = await _pricingService.BaseQuery.Where(p => p.ServiceModelId == (int)model &&
                                                                     p.ImageQualityId == (int)imageQuality && 
                                                                     p.ImageResolutionId == (int)imageResolution)
                                                              .FirstOrDefaultAsync(cancellationToken);
        var exchangeRate = await _currencyExchangeRateService.ListAsync(cancellationToken);

            var cost = (decimal)pricingParameter.ImageAudioPrice * exchangeRate.Select(er => er.CurrencyExchangeRate1).LastOrDefault();
        return new CostCalculationDto() { PricingId = pricingParameter.Id, CostUsage = cost };

    }
}
