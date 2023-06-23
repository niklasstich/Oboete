using Microsoft.AspNetCore.Mvc;
using Oboete.API.Controllers.Dto;
using Oboete.API.Services.Jwt;
using Oboete.API.Services.Users;

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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var applicationUser = await _userService.RegisterUserAsync(registrationRequest.Username, registrationRequest.Email, registrationRequest.Password);

        return Created("", new RegistrationResponseDto{Email = applicationUser.Email!, Username = applicationUser.UserName!});
    }

    [HttpPost("GetToken")]
    public async Task<ActionResult<AuthenticationResponseDto>> CreateBearerToken(AuthenticationRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest("Bad request");

        var user = await _userService.CheckCredentialsAsync(request.Username, request.Password);

        var token = await _jwtService.IssueTokenAsync(user);
        var response = new AuthenticationResponseDto
        {
            Expiration = token.Expiration,
            Token = token.Token,
            RefreshToken = token.RefreshToken
        };

        return Ok(response);
    }

    [HttpPost("RefreshToken")]
    public async Task<ActionResult<AuthenticationResponseDto>> RefreshToken(AuthenticationRefreshRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Bad request");
        }

        var token = await _jwtService.RefreshTokenAsync(request.RefreshToken);
        var response = new AuthenticationResponseDto
        {
            Expiration = token.Expiration,
            Token = token.Token,
            RefreshToken = token.RefreshToken
        };

        return Ok(response);
    }
}