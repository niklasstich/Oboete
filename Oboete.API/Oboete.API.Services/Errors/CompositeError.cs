namespace Oboete.API.Services.Errors;

public record CompositeError(IEnumerable<ApplicationError> Errors) : ApplicationError("Multiple errors occured", true);