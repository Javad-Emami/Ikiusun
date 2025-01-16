using Application.Features.AudioModels.Dto;
using Application.Interfaces.Audio;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using NAudio.Wave;
using OpenAI;
using OpenAI.Audio;
using System;

namespace Service.Services.AudioModels;

public class OpenAi_AudioModel : IOpenAi_AudioModels
{
    private readonly string _openAIKey;
    public OpenAi_AudioModel(IConfiguration configuration)
    {
        _openAIKey = configuration.GetSection("OpenAI")["Key"];
    }
    public async Task<TextToSpeechResponseDto> TextToSpeechAsync(TextToSpeechRequestDto dto, CancellationToken cancellationToken)
    {
        var openAiClient = new OpenAIClient(_openAIKey);

        var speedOptions = new SpeechGenerationOptions()
        {
            SpeedRatio = dto.Speed,                      
        };
        var audioModel = dto.ModelNameId == 1 ? "tts-1" : dto.ModelNameId == 2 ? "tts-1-hd" : "whisper";
        var audioAssistantResult = await openAiClient.GetAudioClient(audioModel).GenerateSpeechAsync(dto.InputText,dto.VoiceName, speedOptions,cancellationToken);

        var bytes = audioAssistantResult.Value;
        byte[] audioBytes = bytes.ToArray();

        var res = new TextToSpeechResponseDto()
        {
            AudioFile = audioBytes
        };
       return res;
    }

    public async Task<double> AudioLengthDurationCalculator(byte[] audioBytesArray, CancellationToken cancellationToken)
    {
        using (var stream = new MemoryStream(audioBytesArray))
        using (var mp3Reader = new Mp3FileReader(stream))
        {
            return mp3Reader.TotalTime.TotalSeconds;
        }
    }
}
