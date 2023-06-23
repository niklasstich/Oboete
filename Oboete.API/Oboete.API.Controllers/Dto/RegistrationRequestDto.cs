using System.ComponentModel.DataAnnotations;

namespace Oboete.API.Controllers.Dto;

public class RegistrationRequestDto
{
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string Email { get; set; }
}