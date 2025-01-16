using Microsoft.AspNetCore.Http;

namespace Application.Features.AudioModels.Dto;

public class TextToSpeechResponseDto
{
    public byte[] AudioFile { get; set; }
}
