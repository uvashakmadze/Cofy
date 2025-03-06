using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cofy.IncomeTaxCalculator.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController(IMediator mediator) : ControllerBase
{
    protected readonly IMediator Mediator = mediator;
}
