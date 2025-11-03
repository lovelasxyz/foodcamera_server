using System.Collections.Generic;
using Cases.Application.Cases.Commands.AddPrizeToCase;
using Cases.Application.Cases.Commands.CreateCase;
using Cases.Application.Cases.Commands.DeleteCase;
using Cases.Application.Cases.Commands.FreezeCase;
using Cases.Application.Cases.Commands.PublishCase;
using Cases.Application.Cases.Commands.RemovePrizeFromCase;
using Cases.Application.Cases.Commands.UpdateCase;
using System.Text.Json.Serialization;
using Cases.Application.Cases.Queries.GetCase;
using Cases.Application.Cases.Queries.GetCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cases.API.Controllers.Admin;

[Authorize(AuthenticationSchemes = "Bearer,BotApiKey", Roles = "admin")]
[Route("api/admin/cases")]
public sealed class AdminCasesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CaseListItemDto>>> GetCases([FromQuery] int? limit = null, [FromQuery] bool includeInactive = true)
    {
        var cases = await Mediator.Send(new GetCasesQuery(limit, includeInactive));
        return Ok(cases);
    }

    [HttpGet("{caseId:int}")]
    public async Task<ActionResult<CaseDto>> GetCase(int caseId)
    {
        var caseDto = await Mediator.Send(new GetCaseQuery(caseId));
        return Ok(caseDto);
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateCase([FromBody] CreateCaseRequest request)
    {
        var command = new CreateCaseCommand(
            request.Name,
            request.Image,
            request.Price,
            request.CommissionPercent,
            request.SortOrder,
            request.AutoHide,
            request.VisibleFrom,
            request.VisibleUntil);

        var caseId = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetCase), new { caseId }, caseId);
    }

    [HttpPut("{caseId:int}")]
    public async Task<IActionResult> UpdateCase(int caseId, [FromBody] UpdateCaseRequest request)
    {
        var command = new UpdateCaseCommand(
            caseId,
            request.NameSpecified,
            request.Name,
            request.ImageSpecified,
            request.Image,
            request.PriceSpecified,
            request.Price,
            request.CommissionPercentSpecified,
            request.CommissionPercent,
            request.SortOrderSpecified,
            request.SortOrder,
            request.AutoHideSpecified,
            request.AutoHide,
            request.VisibleFromSpecified,
            request.VisibleFrom,
            request.VisibleUntilSpecified,
            request.VisibleUntil);

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{caseId:int}")]
    public async Task<IActionResult> DeleteCase(int caseId)
    {
        await Mediator.Send(new DeleteCaseCommand(caseId));
        return NoContent();
    }

    [HttpPost("{caseId:int}/prizes")]
    public async Task<ActionResult<int>> AddPrizeToCase(int caseId, [FromBody] AddPrizeToCaseRequest request)
    {
        var command = new AddPrizeToCaseCommand(caseId, request.PrizeId, request.Weight);
        var casePrizeId = await Mediator.Send(command);
        return Ok(casePrizeId);
    }

    [HttpDelete("{caseId:int}/prizes/{prizeId:int}")]
    public async Task<IActionResult> RemovePrizeFromCase(int caseId, int prizeId)
    {
        await Mediator.Send(new RemovePrizeFromCaseCommand(caseId, prizeId));
        return NoContent();
    }

    [HttpPost("{caseId:int}/publish")]
    public async Task<IActionResult> PublishCase(int caseId, [FromBody] PublishCaseRequest request)
    {
        await Mediator.Send(new PublishCaseCommand(caseId, request.VisibleFrom, request.VisibleUntil));
        return NoContent();
    }

    [HttpPost("{caseId:int}/freeze")]
    public async Task<IActionResult> FreezeCase(int caseId)
    {
        await Mediator.Send(new FreezeCaseCommand(caseId));
        return NoContent();
    }
}

public sealed record CreateCaseRequest(
    string? Name,
    string? Image,
    decimal Price,
    decimal CommissionPercent,
    int SortOrder,
    bool AutoHide,
    DateTimeOffset? VisibleFrom,
    DateTimeOffset? VisibleUntil);

public sealed record AddPrizeToCaseRequest(int PrizeId, int Weight);

public sealed record PublishCaseRequest(DateTimeOffset? VisibleFrom, DateTimeOffset? VisibleUntil);

public sealed class UpdateCaseRequest
{
    private string? _name;
    private string? _image;
    private decimal? _price;
    private decimal? _commissionPercent;
    private int? _sortOrder;
    private bool? _autoHide;
    private DateTimeOffset? _visibleFrom;
    private DateTimeOffset? _visibleUntil;

    public string? Name
    {
        get => _name;
        set
        {
            _name = value;
            NameSpecified = true;
        }
    }

    [JsonIgnore]
    public bool NameSpecified { get; private set; }

    public string? Image
    {
        get => _image;
        set
        {
            _image = value;
            ImageSpecified = true;
        }
    }

    [JsonIgnore]
    public bool ImageSpecified { get; private set; }

    public decimal? Price
    {
        get => _price;
        set
        {
            _price = value;
            PriceSpecified = true;
        }
    }

    [JsonIgnore]
    public bool PriceSpecified { get; private set; }

    public decimal? CommissionPercent
    {
        get => _commissionPercent;
        set
        {
            _commissionPercent = value;
            CommissionPercentSpecified = true;
        }
    }

    [JsonIgnore]
    public bool CommissionPercentSpecified { get; private set; }

    public int? SortOrder
    {
        get => _sortOrder;
        set
        {
            _sortOrder = value;
            SortOrderSpecified = true;
        }
    }

    [JsonIgnore]
    public bool SortOrderSpecified { get; private set; }

    public bool? AutoHide
    {
        get => _autoHide;
        set
        {
            _autoHide = value;
            AutoHideSpecified = true;
        }
    }

    [JsonIgnore]
    public bool AutoHideSpecified { get; private set; }

    public DateTimeOffset? VisibleFrom
    {
        get => _visibleFrom;
        set
        {
            _visibleFrom = value;
            VisibleFromSpecified = true;
        }
    }

    [JsonIgnore]
    public bool VisibleFromSpecified { get; private set; }

    public DateTimeOffset? VisibleUntil
    {
        get => _visibleUntil;
        set
        {
            _visibleUntil = value;
            VisibleUntilSpecified = true;
        }
    }

    [JsonIgnore]
    public bool VisibleUntilSpecified { get; private set; }
}

