using Application.Features.ChatModels.GPT_3._5Turbo.Dto;
using Application.Features.ChatModels.GPT_3._5Turbo.Query;
using Application.Features.ChatModels.GPT_4o.Command;
using Application.Features.ChatModels.GPT_4o.Dto;
using Application.Features.ChatModels.Gpt_4oMini.Command;
using Application.Features.ChatModels.Gpt_4oMini.Dto;
using Application.Interfaces;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class ChatModelsController : ApiBaseController
    {
        private readonly IWalletService _walletService;
        public ChatModelsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost("gpt-three-point-five-turbo")]
        [Authorize]
        public async Task<ActionResult<ApiResult<ChatResponseDto>>> GPTThreePointFiveTurbo(ChatRequestDto dto,CancellationToken cancellationToken) 
        {
            var hasEnoughValue = await _walletService.HasMinumumBalanceValueForChatModel(LoggedinUserMobile, cancellationToken);
            if (hasEnoughValue)
            {
                var result = await Mediator.Send(new GPTThreePointFiveTurboCommand(dto), cancellationToken);
                return new ApiResult<ChatResponseDto>(result);
            }
            else
                throw new CustomException(500, "اعتبار شما برای استفاده از این سرویس کافی نمی باشد. لطفا حساب خود را شارژ نمایید.");
        }

        [HttpPost("gpt-4o-vision-capability")]
        [Authorize]
        public async Task<ActionResult<ApiResult<Gpt4oResponseDto>>> GPT4oVisionCapability([FromForm]Gpt4oRequestDto dto, CancellationToken cancellationToken)
        {
            var hasEnoughValue = await _walletService.HasMinumumBalanceValueForChatModel(LoggedinUserMobile, cancellationToken);
            if (hasEnoughValue)
            {
                var result = await Mediator.Send(new GPT4oVisionCapabilityCommand(dto), cancellationToken);
                return new ApiResult<Gpt4oResponseDto>(result);
            }
            else
                throw new CustomException(500, "اعتبار شما برای استفاده از این سرویس کافی نمی باشد. لطفا حساب خود را شارژ نمایید.");
        }

        [HttpPost("gpt-4omini-vision-capability")]
        [Authorize]
        public async Task<ActionResult<ApiResult<Gpt4oMiniResponseDto>>> GPT4oMiniVisionCapability([FromForm]Gpt4oMiniRequestDto dto, CancellationToken cancellationToken)
        {
            var hasEnoughValue = await _walletService.HasMinumumBalanceValueForChatModel(LoggedinUserMobile, cancellationToken);
            if (hasEnoughValue)
            {
                var result = await Mediator.Send(new ChatGpt4oMiniVisionCapibilityCommand(dto), cancellationToken);
                return new ApiResult<Gpt4oMiniResponseDto>(result);
            }
            else
                throw new CustomException(500, "اعتبار شما برای استفاده از این سرویس کافی نمی باشد. لطفا حساب خود را شارژ نمایید.");
        }

    }
}
