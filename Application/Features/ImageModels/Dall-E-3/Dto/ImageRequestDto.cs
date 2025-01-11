namespace Application.Features.ImageModels.Dall_E_3.Dto;

public class ImageRequestDto
{
    public Guid? Id { get; set; }
    public string ImagePrompt { get; set; }
    public int? ImageSize { get; set; }
    public int? Quality { get; set; }
    public string? Style { get; set; }
}
