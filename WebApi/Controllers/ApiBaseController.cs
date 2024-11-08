using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ApiBaseController : ControllerBase
{
    public ApiBaseController()
    {
        
    }
    protected IMediator Mediator => (IMediator)HttpContext.RequestServices.GetServices<IMediator>();   
}
