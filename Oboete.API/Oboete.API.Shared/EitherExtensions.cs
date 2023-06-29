using LanguageExt;

namespace Oboete.API.Shared;

public static class EitherExtensions
{
    public static async Task<Either<TL, TR>> FlattenAsync<TL, TR>(this Task<Either<TL, Either<TL, TR>>> task) =>
        (await task).Flatten();
}