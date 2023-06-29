using LanguageExt;
using Microsoft.AspNetCore.Mvc;

namespace Oboete.API.Controllers.Controllers;

public static class ControllerExtensions
{
    public static ActionResult ToActionResult<TL, TR>(this Either<TL, TR> either) =>
        either.Match(Ok, BadRequest);

    public static ActionResult ToActionResult<TL, TR>(this Either<TL, TR> either, Func<TL, ActionResult> left) =>
        either.Match(Ok, left);

    public static ActionResult ToActionResult<TL, TR>(this Either<TL, TR> either, Func<TR, ActionResult> right,
        Func<TL, ActionResult> left) =>
        either.Match(right, left);

    public static Task<ActionResult> ToActionResult<TL, TR>(this EitherAsync<TL, TR> either) =>
        either.Match(Ok, BadRequest);

    public static Task<ActionResult> ToActionResult<TL, TR>(this EitherAsync<TL, TR> either,
        Func<TL, ActionResult> left) =>
        either.Match(Ok, left);

    public static Task<ActionResult> ToActionResult<TL, TR>(this EitherAsync<TL, TR> either,
        Func<TR, ActionResult> right, Func<TL, ActionResult> left) =>
        either.Match(right, left);

    public static async Task<ActionResult> ToActionResult<TL, TR>(this Task<Either<TL, TR>> either) =>
        (await either).ToActionResult();

    public static async Task<ActionResult> ToActionResult<TL, TR>(this Task<Either<TL, TR>> either,
        Func<TL, ActionResult> left) =>
        (await either).ToActionResult(left);

    public static async Task<ActionResult> ToActionResult<TL, TR>(this Task<Either<TL, TR>> either,
        Func<TR, ActionResult> right, Func<TL, ActionResult> left) =>
        (await either).ToActionResult(right, left);

    private static ActionResult Ok<T>(T value) => new OkObjectResult(value);

    private static ActionResult BadRequest<T>(T value) =>
        new BadRequestObjectResult("An internal server error occurred");
}