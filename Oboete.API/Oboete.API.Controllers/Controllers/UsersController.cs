using Microsoft.AspNetCore.Mvc;
using Oboete.API.Controllers.Dto;
using Oboete.API.Entities.Users;
using Oboete.API.Services.Errors;
using Oboete.API.Services.Errors.Jwt;
using Oboete.API.Services.Errors.Users;
using Oboete.API.Services.Jwt;
using Oboete.API.Services.Users;
using Oboete.API.Shared;

namespace Oboete.API.Controllers.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;

    public UsersController(IJwtService jwtService, IUserService userService)
    {
        _jwtService = jwtService;
        _userService = userService;
    }


    [HttpPost("Register")]
    public async Task<ActionResult<RegistrationResponseDto>> Register(RegistrationRequestDto registrationRequest)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        return await _userService
            .RegisterUserAsync(registrationRequest.Username,
                registrationRequest.Email, registrationRequest.Password)
            .MapAsync(UserToResponseDto)
            .ToActionResult(RegisterErrorToActionResult);
    }

    private ActionResult RegisterErrorToActionResult(ApplicationError error) =>
        error switch
        {
            RegisterInvalidInputError e => BadRequest(e.Message),
            RegisterExceptionalError e => StatusCode(500, e.Message),
            _ => StatusCode(500)
        };

    [HttpPost("GetToken")]
    public async Task<ActionResult<AuthenticationResponseDto>> CreateBearerToken(AuthenticationRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest("Bad request");

        return await _userService
            .CheckCredentialsAsync(request.Username, request.Password)
            .MapAsync(_jwtService.IssueTokenAsync)
            .FlattenAsync()
            .MapAsync(TokenToResponseDto)
            .ToActionResult(Ok, CreateBearerTokenErrorToActionResult);
    }

    private ActionResult CreateBearerTokenErrorToActionResult(ApplicationError error) =>
        error switch
        {
            InvalidCredentialsError e => Unauthorized(e.Message),
            InvalidApplicationUserError => StatusCode(500),
            _ => StatusCode(500)
        };

    private RegistrationResponseDto UserToResponseDto(ApplicationUser user) =>
        new() { Email = user.Email!, Username = user.UserName! };

    private AuthenticationResponseDto TokenToResponseDto(JwtTokenIssuance t) =>
        new()
            { Expiration = t.Expiration, Token = t.Token, RefreshToken = t.RefreshToken };

    [HttpPost("RefreshToken")]
    public async Task<ActionResult<AuthenticationResponseDto>> RefreshToken(AuthenticationRefreshRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Bad request");
        }

        return await _jwtService
            .RefreshTokenAsync(request.RefreshToken)
            .MapAsync(TokenToResponseDto)
            .ToActionResult(Ok, RefreshTokenErrorToActionResult);
    }

    private ActionResult RefreshTokenErrorToActionResult(ApplicationError error) => error switch
    {
        InvalidApplicationUserError => StatusCode(500),
        InvalidTokenError e => Unauthorized(e.Message),
        _ => StatusCode(500)
    };
}