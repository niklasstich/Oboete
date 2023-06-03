using System.ComponentModel.DataAnnotations;

namespace Oboete.API.Dto;

public class AuthenticationRequestDto
{
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
}