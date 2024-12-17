using Application.Features.ImageModels.Dall_E_3.Dto;
using Application.Features.ImageModels.Dall_E_3.Query;
using Application.Interfaces;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class ImageModelsController : ApiBaseController
{
    private readonly IWalletService _walletService;
    public ImageModelsController(IWalletService walletService)
    {
        _walletService = walletService;
    }


    [HttpPost("dallE3")]
    [Authorize]
    public async Task<ActionResult<ApiResult<ImageResponseDto>>> ImageGenerator([FromForm]ImageRequestDto dto, CancellationToken cancellationToken)
    {
        var hasEnoughValue = await _walletService.HasMinumumBalanceValueForChatModelAsync(LoggedinUserMobile, cancellationToken);
        if (hasEnoughValue)
        {
            var result = await Mediator.Send(new ImageGeneratorCommand(dto), cancellationToken);
            //byte[] imageBytes = Convert.FromBase64String(result.ImageBase64);

            //return File(imageBytes, "image/jpeg", "downloaded_image.jpg");
           return new ApiResult<ImageResponseDto>(result);
        }
        else
            throw new CustomException(500, "اعتبار شما برای استفاده از این سرویس کافی نمی باشد. لطفا حساب خود را شارژ نمایید.");
    }
}
