using System.ComponentModel.DataAnnotations;

namespace Oboete.API.Controllers.Dto;

public class AuthenticationRefreshRequestDto
{
    [Required] public string RefreshToken { get; set; }
}