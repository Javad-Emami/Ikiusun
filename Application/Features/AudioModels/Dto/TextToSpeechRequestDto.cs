namespace Application.Features.AudioModels.Dto;

public class TextToSpeechRequestDto
{
    public Guid? Id { get; set; }
    public int ModelNameId { get; set; }
    public string InputText { get; set; }
    public string VoiceName { get; set; }
    public float? Speed { get; set; }
}
