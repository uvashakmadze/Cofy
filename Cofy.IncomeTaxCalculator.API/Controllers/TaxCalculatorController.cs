using Cofy.IncomeTaxCalculator.API.Controllers.Base;
using Cofy.IncomeTaxCalculator.Application.UseCases.IncomeTaxCalculator.Calculate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace Cofy.IncomeTaxCalculator.API.Controllers;

[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Income tax calculator")]
public class TaxCalculatorController(IMediator mediator) : BaseApiController(mediator)
{
    [Authorize(AuthenticationSchemes = OpenApi.Authorization.AuthenticationSchemes.Basic)]
    [HttpPost]
    [Route("calculate")]
    [SwaggerOperation("Calculate income tax by annual gross salary")]
    [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IncomeTaxCalculateResponse), Description = "Success")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "There is validation error")]
    public async Task<IncomeTaxCalculateResponse> CalculateIncomeTaxAsync([FromBody] decimal grossAnnualSalary) =>
        await Mediator.Send(new IncomeTaxCalculateRequest(grossAnnualSalary));
}
