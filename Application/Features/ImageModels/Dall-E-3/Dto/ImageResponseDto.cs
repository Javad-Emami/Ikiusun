namespace Application.Features.ImageModels.Dall_E_3.Dto;

public class ImageResponseDto
{
    public string ImageBase64 { get; set; }
    public Uri ImageUri { get; set; }
    public string ImagePrompt { get; set; }
    public int? ImageSize { get; set; }
    public string? Quality { get; set; }
    public string? Style { get; set; }
}
