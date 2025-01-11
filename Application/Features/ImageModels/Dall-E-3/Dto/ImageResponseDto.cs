namespace Application.Features.ImageModels.Dall_E_3.Dto;

public class ImageResponseDto
{
    public Guid ConversationId { get; set; }
    public string ImageBase64 { get; set; }
    public Uri ImageUri { get; set; }
    public string ImagePrompt { get; set; }
    public int? ImageResolution { get; set; }
    public int? ImageQuality { get; set; }
    public string? Style { get; set; }
}
