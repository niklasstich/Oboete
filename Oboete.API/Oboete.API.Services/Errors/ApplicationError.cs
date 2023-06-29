
namespace Oboete.API.Services.Errors;

public abstract record ApplicationError(string Message, bool Expected);