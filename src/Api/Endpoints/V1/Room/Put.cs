using Api.Endpoints.V1.Models.Room;
using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Events.Contracts;
using Domain.Events.Room;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Endpoints.V1.Room
{
    public class Put : IEndpoint
    {
        private static async Task<IResult> Handler([FromRoute] string id,
            [FromBody] RoomUpdateRequestModel request,
            [FromServices] IApiContext apiContext,
            [FromServices] IRoomRepository roomRepository,
            [FromServices] IEventPublisher eventPublisher,
            CancellationToken cancellationToken)
        {
            var room = await roomRepository.GetRoomAsync(id, cancellationToken);
            if (room == null)
            {
                return Results.NotFound();
            }

            if (!room.Admins.Contains(apiContext.CurrentUserId))
            {
                return Results.Forbid();
            }

            room.Name = request.Name;
            room.Description = request.Description;
            room.ImageUrl = request.ImageUrl;

            await roomRepository.SaveRoomAsync(room, cancellationToken);
            await eventPublisher.PublishAsync(new RoomChangedEvent
            {
                ActivityAt = DateTime.UtcNow,
                RoomId = id
            }, cancellationToken);
            return Results.Ok();
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("v1/rooms/{id}", Handler)
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithTags("Room");
        }
    }
}