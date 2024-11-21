using Application.Features.ChatModels.GPT_3._5Turbo.Dto;
using Application.Features.ChatModels.GPT_3._5Turbo.Query;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class ChatModelsController : ApiBaseController
    {
        [HttpPost("GPTThreePointFiveTurbo")]
        public async Task<ActionResult<ApiResult<ChatResponseDto>>> GPTThreePointFiveTurbo(ChatRequestDto dto,CancellationToken cancellationToken) 
        {
            var result = await Mediator.Send(new GPTThreePointFiveTurboCommand(dto), cancellationToken);
            return new ApiResult<ChatResponseDto>(result);
        }

    }
}
