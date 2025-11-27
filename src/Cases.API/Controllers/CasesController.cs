using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cases.API.Contracts.Cases;
using Cases.Application.Cases.Queries.GetCase;
using Cases.Application.Cases.Queries.GetCasesWithPrizes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cases.API.Controllers;

[AllowAnonymous]
public sealed class CasesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CaseResponse>>> GetCases(CancellationToken cancellationToken)
    {
        var cases = await Mediator.Send(new GetCasesWithPrizesQuery(IncludeInactive: false), cancellationToken);

        if (cases.Count == 0)
        {
            return Ok(Array.Empty<CaseResponse>());
        }

        var response = CaseResponseMapper.MapMany(cases);

        return Ok(response);
    }
}
