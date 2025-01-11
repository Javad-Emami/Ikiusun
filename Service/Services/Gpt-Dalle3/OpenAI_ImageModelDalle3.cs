using Application.Features.ImageModels.Dall_E_3.Dto;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using OpenAI.Images;

namespace Service.Services;

public class OpenAI_ImageModelDalle3 : IOpenAI_ImageModelDalle3
{
    private readonly string _openAIKey;
    private readonly IUserRequestService _userRequestService;
    public OpenAI_ImageModelDalle3(IConfiguration configuration, IUserRequestService userRequestService)
    {
        _openAIKey = configuration.GetSection("OpenAI")["Key"];
        _userRequestService = userRequestService;
    }
    public async Task<ImageResponseDto> GenerateImegeAsync(ImageRequestDto requestDto)
    {
        ImageClient client = new("dall-e-3", _openAIKey);
                
        ImageGenerationOptions options = new()
        {
            Quality = requestDto.Quality == (int)ImageQualityEnum.Standard ? GeneratedImageQuality.Standard : GeneratedImageQuality.High,
            Size = requestDto.ImageSize == (int)ImageResolutionEnum.W1024xH1024 ? GeneratedImageSize.W1024xH1024 :
                   requestDto.ImageSize == (int)ImageResolutionEnum.W1024xH1792 ? GeneratedImageSize.W1024xH1792 : GeneratedImageSize.W1792xH1024,
            Style = requestDto.Style =="Natural" ? GeneratedImageStyle.Natural : GeneratedImageStyle.Vivid,                 
        };

        GeneratedImage image = client.GenerateImage(requestDto.ImagePrompt, options);

        var imageUri = image.ImageUri;
        BinaryData bytes = image.ImageBytes;

        var base64String = Convert.ToBase64String(bytes);

        var response = new ImageResponseDto() 
        { 
            ImageUri = imageUri,
            ImageBase64 = base64String,
            ImagePrompt = requestDto.ImagePrompt,
            ImageResolution = requestDto.ImageSize != null ? requestDto.ImageSize : (int)ImageResolutionEnum.W1024xH1024,   
            ImageQuality = requestDto.Quality != null ? requestDto.Quality : (int)ImageQualityEnum.Standard,   
            Style = requestDto.Style
        };

        return response;

    }
}
