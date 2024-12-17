using Application.Features.ImageModels.Dall_E_3.Dto;

namespace Application.Interfaces;

public interface IOpenAI_ImageModelDalle3
{
    Task<ImageResponseDto> GenerateImegeAsync(ImageRequestDto imageRequestDto);
}
