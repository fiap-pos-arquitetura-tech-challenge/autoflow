using AutoFlow.Application.Services;
using AutoFlow.Application.Services.Enums;

namespace AutoFlow.Api.Extensions
{
    public static class ResultExtensions
    {
        public static IResult ToCreatedHttpResult<T>(
            this Result<T> result,
            Func<T, string> location)
        {
            if (!result.IsSuccess)
                return ToErrorResult(result);

            return Results.Created(location(result.Value!), result.Value);
        }

        public static IResult ToHttpResult<T>(this Result<T> result)
        {
            if (!result.IsSuccess)
                return ToErrorResult(result);

            return Results.Ok(result.Value);
        }

        public static IResult ToHttpResult(this Result result)
        {
            if (!result.IsSuccess)
                return ToErrorResult(result);

            return Results.NoContent();
        }

        private static IResult ToErrorResult(Result result)
        {
            var statusCode = result.ErrorType switch
            {
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError
            };

            return Results.Problem(detail: result.Error, statusCode: statusCode);
        }
    }
}
