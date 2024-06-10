using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto.Room;
using Domain.Events.Contracts;
using Domain.Events.Room;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room.User.Admin
{
    public class Delete : IEndpoint
    {
        private static async Task<IResult> Handler(
            [FromRoute] string id,
            [FromRoute] string userId,
            [FromServices] IApiContext apiContext,
            [FromServices] IRoomRepository roomRepository,
            [FromServices] IEventPublisher eventPublisher,
            [FromServices] IEventBusManager eventBusManager,
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

            if (!room.Attenders.Contains(userId))
            {
                return Results.NotFound();
            }

            if (!room.Admins.Contains(userId))
            {
                return Results.Ok();
            }

            room.Admins.Remove(userId);

            await roomRepository.SaveRoomAsync(room, cancellationToken);
            await eventPublisher.PublishAsync(new RoomChangedEvent
            {
                RoomId = room.Id,
                SenderId = userId,
                ActivityAt = DateTime.UtcNow
            }, cancellationToken);
            await eventBusManager.RoomAdminRemovedAsync(room.ToDto(), userId, cancellationToken);
            return Results.Ok();
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/v1/rooms/{id}/users/{userId}/admin", Handler)
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithTags("Room");
        }
    }
}