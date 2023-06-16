using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.User.Ban
{
    public class GetList : IEndpoint
    {
        private static async Task<IResult> Handler(
            [FromServices] IApiContext apiContext,
            [FromServices] IUserBanRepository userBanRepository,
            CancellationToken cancellationToken)
        {
            var bannedUsers = await userBanRepository.GetBannedUsersAsync(apiContext.CurrentUserId, cancellationToken);
            return Results.Ok(bannedUsers);
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("v1/user/ban", Handler)
                .Produces<List<string>>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithTags("User");
        }
    }
}