using System.Collections.Generic;
using Cases.Application.Prizes.Commands.CreatePrize;
using Cases.Application.Prizes.Commands.UpdatePrize;
using Cases.Application.Prizes.Queries.GetPrize;
using Cases.Application.Prizes.Queries.GetPrizes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cases.API.Controllers.Admin;

[Authorize(AuthenticationSchemes = "Bearer,BotApiKey", Roles = "admin")]
[Route("api/admin/prizes")]
public sealed class AdminPrizesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PrizeListItemDto>>> GetPrizes([FromQuery] bool onlyActive = true)
    {
        var prizes = await Mediator.Send(new GetPrizesQuery(onlyActive));
        return Ok(prizes);
    }

    [HttpGet("{prizeId:int}")]
    public async Task<ActionResult<PrizeDetailsDto>> GetPrize(int prizeId)
    {
        var prize = await Mediator.Send(new GetPrizeQuery(prizeId));

        if (prize is null)
        {
            return NotFound();
        }

        return Ok(prize);
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreatePrize([FromBody] CreatePrizeRequest request)
    {
        var command = new CreatePrizeCommand(
            request.Name,
            request.Price,
            request.Image,
            request.Rarity,
            request.IsShard,
            request.ShardKey,
            request.ShardsRequired,
            request.Description,
            request.UniqueKey,
            request.Stackable,
            request.NotAwardIfOwned,
            request.NonRemovableGift,
            request.BenefitType,
            request.BenefitDataJson,
            request.DropWeight);

        var prizeId = await Mediator.Send(command);
        var prize = await Mediator.Send(new GetPrizeQuery(prizeId));

        return CreatedAtAction(nameof(GetPrize), new { prizeId }, prize);
    }

    [HttpPut("{prizeId:int}")]
    public async Task<IActionResult> UpdatePrize(int prizeId, [FromBody] UpdatePrizeRequest request)
    {
        var command = new UpdatePrizeCommand(
            prizeId,
            request.Name,
            request.Price,
            request.Image,
            request.Rarity,
            request.IsShard,
            request.ShardKey,
            request.ShardsRequired,
            request.Description,
            request.UniqueKey,
            request.Stackable,
            request.NotAwardIfOwned,
            request.NonRemovableGift,
            request.BenefitType,
            request.BenefitDataJson,
            request.DropWeight,
            request.IsActive);

        await Mediator.Send(command);

        return NoContent();
    }
}

public sealed record CreatePrizeRequest(
    string? Name,
    decimal Price,
    string? Image,
    string Rarity,
    bool IsShard,
    string? ShardKey,
    int? ShardsRequired,
    string? Description,
    string? UniqueKey,
    bool Stackable,
    bool NotAwardIfOwned,
    bool NonRemovableGift,
    string? BenefitType,
    string? BenefitDataJson,
    decimal DropWeight);

public sealed record UpdatePrizeRequest(
    string? Name,
    decimal Price,
    string? Image,
    string Rarity,
    bool IsShard,
    string? ShardKey,
    int? ShardsRequired,
    string? Description,
    string? UniqueKey,
    bool Stackable,
    bool NotAwardIfOwned,
    bool NonRemovableGift,
    string? BenefitType,
    string? BenefitDataJson,
    decimal DropWeight,
    bool IsActive);
