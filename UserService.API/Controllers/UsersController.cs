namespace UserService.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Requests;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] UserRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        var userId = await _mediator.Send(request.ToCommand(), cancellationToken);
        return Ok(new { userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] UserLoginRequest request,
        CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(request.ToCommand(), cancellationToken);
        return Ok(new { token });
    }
}