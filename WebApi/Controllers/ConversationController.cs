using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Conversation.Commads;
using Application.Features.ConversationService.Commands;
using Application.Features.ConversationService.Dto;

namespace WebApi.Controllers
{
    public class conversationController : ApiBaseController
    {
        [HttpDelete("delete/{conversationId:guid}")]
        public async Task<ActionResult<ApiResult<bool>>> DeleteConversation(Guid conversationId,CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new DeleteConversationCommand(conversationId), cancellationToken);
            return new ApiResult<bool>(result);
        }

        [HttpPut("updateConversationName")]
        public async Task<ActionResult<ApiResult<bool>>> UpdateConversationName(UpdateConversationNameDto dto, CancellationToken cancellationToken)
        {
            var result = await Mediator.Send(new UpdateConversationNameCommand(dto), cancellationToken);
            return new ApiResult<bool>(result);
        }
    }
}
