using System.ComponentModel.DataAnnotations;

namespace Oboete.API.Dto;

public class AuthenticationRefreshRequestDto
{
    [Required] public string Token { get; set; }
    [Required] public string RefreshToken { get; set; }
}