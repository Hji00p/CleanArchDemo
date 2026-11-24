namespace ContentService.API.Controllers;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Queries.GetAllContent;
using Requests;

[ApiController]
[Route("api/content")]
[Authorize]
public class ContentController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Автор береться з JWT, а не з тіла запиту, щоб не можна було писати від чужого імені
    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("У токені немає ідентифікатора користувача");

    [HttpPost("article")]
    public async Task<IActionResult> CreateArticle([FromBody] CreateArticleRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(request.ToCommand(CurrentUserId), ct);
        return Ok(new { id });
    }

    [HttpPost("video")]
    public async Task<IActionResult> CreateVideo([FromBody] CreateVideoRequest request, CancellationToken ct)
    {
        var id = await _mediator.Send(request.ToCommand(CurrentUserId), ct);
        return Ok(new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var items = await _mediator.Send(new GetAllContentQuery(), ct);
        return Ok(items);
    }
}
