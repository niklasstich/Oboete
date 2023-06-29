namespace Oboete.API.Services.Errors.Jwt;

public record InvalidApplicationUserError(string Message) : ApplicationError(Message, false)
{
    
}