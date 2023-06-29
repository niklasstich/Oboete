namespace Oboete.API.Services.Errors.Users;

public record InvalidCredentialsError(string Message) : ApplicationError(Message, true);