using Application.Features.AudioModels.Commands;
using Application.Features.AudioModels.Dto;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class audioModelsController : ApiBaseController
{
    [HttpPost("text-to-speech")]
    public async Task<ActionResult> TextToSpeech(TextToSpeechRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new TextToSpeechCommand(dto,LoggedinUserMobile), cancellationToken);
        return File(result.AudioFile, "audio/mpeg", "audio.mp3");
    }
}
