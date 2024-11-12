using Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class ChatModelsController : ApiBaseController
    {
        public async Task<ActionResult<ApiResult<bool>>> GPTThreePointFiveTurbo(CancellationToken cancellationToken) 
        {
            var result = await Mediator.Send(new GPTThreePointFiveTurboQuery(), cancellationToken);
            return new ApiResult<bool>(true);
        }

    }
}
