using Application.Features.CostCalculationService.Dto;
using Domain.Enums;

namespace Application.Interfaces;

public interface ICostCalculationService
{
    Task<CostCalculationDto> ChatModelCostCalculationAsync(ServiceModelEnum model,int inputToken,int outPutToken,CancellationToken cancellationToken);
    Task<CostCalculationDto> ImageModelCostCalculationAsync(ServiceModelEnum model, int imageResolution, 
                                                 int imageQuality, CancellationToken cancellationToken);  
}
