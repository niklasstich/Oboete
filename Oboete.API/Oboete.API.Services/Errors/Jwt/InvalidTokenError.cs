namespace Oboete.API.Services.Errors.Jwt;

public record InvalidTokenError(string Message) : ApplicationError(Message, true)
{
    
}