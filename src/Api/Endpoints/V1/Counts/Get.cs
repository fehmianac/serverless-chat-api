using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Counts
{
    public class Get : IEndpoint
    {
        private static async Task<IResult> Handler(
            [FromServices] IApiContext apiContext,
            [FromServices] IRoomNotificationRepository messageCountRepository,
            CancellationToken cancellationToken)
        {
            var result = await messageCountRepository.GetRoomNotificationAsync(apiContext.CurrentUserId, cancellationToken);
            
            return Results.Ok(result.Sum(q => q.MessageCount));
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("v1/rooms/messages/counts", Handler)
                .Produces<int>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithTags("Counts");
        }
    }
}