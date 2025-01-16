using Application.Features.AudioModels.Dto;

namespace Application.Interfaces.Audio;

public interface IOpenAi_AudioModels
{
    Task<TextToSpeechResponseDto> TextToSpeechAsync(TextToSpeechRequestDto dto,CancellationToken cancellationToken);
    Task<double> AudioLengthDurationCalculator(byte[] audioBytesArray,CancellationToken cancellationToken);
}
