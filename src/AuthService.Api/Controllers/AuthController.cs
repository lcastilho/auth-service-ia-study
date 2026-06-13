using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Application.Common;
using AuthService.Application.Features.Auth.Commands.Login;
using AuthService.Application.Features.Auth.Commands.RefreshToken;
using AuthService.Application.Features.Auth.Commands.RegisterUser;
using AuthService.Application.Features.Auth.Commands.RevokeRefreshToken;
using AuthService.Application.Features.Auth.Queries.GetCurrentUser;
using AuthService.Contracts.Requests;
using AuthService.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiCurrentUserResponse = AuthService.Contracts.Responses.CurrentUserResponse;
using ApiRefreshTokenResponse = AuthService.Contracts.Responses.RefreshTokenResponse;
using ApiRegisterUserResponse = AuthService.Contracts.Responses.RegisterUserResponse;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiRegisterUserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.Name, request.Email, request.Password);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            return BadRequest(ToError(result));
        }

        var response = new ApiRegisterUserResponse(
            result.Value.Id,
            result.Value.Name,
            result.Value.Email,
            result.Value.IsActive,
            result.Value.CreatedAt);

        return CreatedAtAction(nameof(GetCurrentUser), new { }, response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResultResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            return IsInvalidCredentials(result)
                ? Unauthorized(ToError(result))
                : BadRequest(ToError(result));
        }

        var response = new AuthResultResponse(
            result.Value.UserId,
            result.Value.Name,
            result.Value.Email,
            result.Value.AccessToken,
            result.Value.AccessTokenExpiresAt,
            result.Value.RefreshToken,
            result.Value.RefreshTokenExpiresAt);

        return Ok(response);
    }

    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiRefreshTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.Token);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            return result.Error == "Refresh token is invalid."
                ? Unauthorized(ToError(result))
                : BadRequest(ToError(result));
        }

        var response = new ApiRefreshTokenResponse(
            result.Value.AccessToken,
            result.Value.AccessTokenExpiresAt,
            result.Value.RefreshToken,
            result.Value.RefreshTokenExpiresAt);

        return Ok(response);
    }

    [HttpPost("revoke-token")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeRefreshToken(
        RevokeRefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RevokeRefreshTokenCommand(request.Token);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ToError(result));
        }

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiCurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = GetAuthenticatedUserId();

        if (userId is null)
        {
            return Unauthorized(new ErrorResponse("Authenticated user id was not found."));
        }

        var query = new GetCurrentUserQuery(userId.Value);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure || result.Value is null)
        {
            return NotFound(ToError(result));
        }

        var response = new ApiCurrentUserResponse(
            result.Value.Id,
            result.Value.Name,
            result.Value.Email,
            result.Value.IsActive,
            result.Value.Roles);

        return Ok(response);
    }

    private Guid? GetAuthenticatedUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private static ErrorResponse ToError(Result result)
    {
        return new ErrorResponse(result.Error ?? "Request failed.");
    }

    private static bool IsInvalidCredentials(Result result)
    {
        return result.Error is "Invalid email or password." or "User is inactive.";
    }
}
