using Domain.Common;
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

    protected string LoggedinUserMobile
    {
        get
        {
            var mobileClaim = User.Claims.FirstOrDefault(u => u.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            if (mobileClaim == null) throw new CustomException(401, "Claim Fail");
            if(!string.IsNullOrEmpty(mobileClaim.Value))
            {
                return mobileClaim.Value.ToString();
            }
            throw new CustomException(401, "Claim Fail");
        }
    }
}
