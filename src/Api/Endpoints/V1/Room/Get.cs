using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto.Room;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room
{
    public class Get : IEndpoint
    {
        private static async Task<IResult> Handler(
            [FromRoute] string id,
            [FromServices] IApiContext apiContext,
            [FromServices] IRoomRepository roomRepository,
            CancellationToken cancellationToken)
        {
            var room = await roomRepository.GetRoomAsync(id, cancellationToken);
            if (room == null)
            {
                return Results.NotFound();
            }

            if (!room.IsAttender(apiContext.CurrentUserId))
            {
                return Results.Forbid();
            }

            return Results.Ok(room.ToDto());
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("v1/rooms/{id}", Handler)
                .Produces<RoomDto>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithTags("Room");
            ;
        }
    }
}