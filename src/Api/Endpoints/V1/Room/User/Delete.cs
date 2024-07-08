using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto.Room;
using Domain.Events.Contracts;
using Domain.Events.Room;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room.User
{
    public class Delete : IEndpoint
    {
        private static async Task<IResult> Handler(
            [FromRoute] string id,
            [FromRoute] string userId,
            [FromServices] IApiContext apiContext,
            [FromServices] IRoomRepository roomRepository,
            [FromServices] IUserRoomRepository userRoomRepository,
            [FromServices] IRoomLastActivityRepository roomLastActivityRepository,
            [FromServices] IEventPublisher eventPublisher,
            [FromServices] IEventBusManager eventBusManager,
            [FromServices] IRoomNotificationRepository roomNotificationRepository,
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

            room.Admins.Remove(userId);
            room.Attenders.Remove(userId);
            await roomRepository.SaveRoomAsync(room, cancellationToken);
            var roomLastActivity = await roomLastActivityRepository.GetRoomLastActivityAsync(id, cancellationToken);
            var utcNow = DateTime.UtcNow;
            var lastActivity = roomLastActivity?.LastActivityAt ?? utcNow;
            await userRoomRepository.DeleteUserRoomAsync(userId, id, lastActivity, cancellationToken);
            await roomNotificationRepository.DeleteRoomNotificationAsync(apiContext.CurrentUserId, id, cancellationToken);
            await eventPublisher.PublishAsync(new RoomChangedEvent
            {
                RoomId = room.Id,
                SenderId = userId,
                ActivityAt = utcNow
            }, cancellationToken);
            await eventBusManager.RoomAttenderRemovedAsync(room.ToDto(), userId, cancellationToken);
            return Results.Ok();
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/v1/rooms/{id}/users/{userId}", Handler)
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithTags("Room");
        }
    }
}