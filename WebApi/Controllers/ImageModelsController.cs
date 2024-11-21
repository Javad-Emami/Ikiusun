using Application.Features.ImageModels.Dall_E_3.Dto;
using Application.Features.ImageModels.Dall_E_3.Query;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;

namespace WebApi.Controllers;

public class ImageModelsController : ApiBaseController
{
    [HttpPost("dallE3")]
    public async Task<ActionResult<ApiResult<ImageResponseDto>>> ImageGenerator([FromForm]ImageRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new ImageGeneratorCommand(dto), cancellationToken);
        //byte[] imageBytes = Convert.FromBase64String(result.ImageBase64);

        //return File(imageBytes, "image/jpeg", "downloaded_image.jpg");
       return new ApiResult<ImageResponseDto>(result);
    }
}
