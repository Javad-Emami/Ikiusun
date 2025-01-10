using Application.Features.ImageModels.Dall_E_3.Dto;
using Application.Features.ImageModels.Dall_E_3.Query;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

public class imageModelsController : ApiBaseController
{
    [HttpPost("dallE3")]
    [Authorize]
    public async Task<ActionResult<ApiResult<ImageResponseDto>>> ImageGenerator([FromForm]ImageRequestDto dto, CancellationToken cancellationToken)
    {      
            var result = await Mediator.Send(new ImageGeneratorCommand(dto, LoggedinUserMobile), cancellationToken);            
           return new ApiResult<ImageResponseDto>(result);        
    }
}
