using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Ban
{
    public class Delete : IEndpoint
    {
        private static async Task<IResult> Handler(
            [FromRoute] string id,
            [FromServices] IApiContext apiContext,
            [FromServices] IUserBanRepository userBanRepository,
            CancellationToken cancellationToken)
        {
            var isBanned = await userBanRepository.IsBannedAsync(apiContext.CurrentUserId, id, cancellationToken);
            if (!isBanned)
            {
                return Results.Ok();
            }

            await userBanRepository.DeleteBanAsync(apiContext.CurrentUserId, id, cancellationToken);
            return Results.Ok();
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("v1/user/{id}/ban", Handler)
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithTags("User");
        }
    }
}