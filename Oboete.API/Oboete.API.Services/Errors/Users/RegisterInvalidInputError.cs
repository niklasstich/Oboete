namespace Oboete.API.Services.Errors.Users;

public record RegisterInvalidInputError(string Message) : ApplicationError(Message, true);