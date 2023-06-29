namespace Oboete.API.Services.Errors.Users;

public record RegisterExceptionalError(string Message) : ApplicationError(Message, false);