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

    private IMediator _mediator;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    //protected IMediator Mediator => HttpContext.RequestServices.GetServices<IMediator>();   
}
