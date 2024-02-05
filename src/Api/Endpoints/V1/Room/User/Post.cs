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
    public class Post : IEndpoint
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

            if (!room.IsGroup)
            {
                return Results.Forbid();
            }
            room.Attenders.Add(userId);
            await roomRepository.SaveRoomAsync(room, cancellationToken);
            var utcNow = DateTime.UtcNow;
            var roomLastActivity = await roomLastActivityRepository.GetRoomLastActivityAsync(room.Id, cancellationToken);
            var lastActivity = roomLastActivity?.LastActivityAt ?? utcNow;
            await userRoomRepository.SaveBatchAsync(room.Id, new List<string> {userId}, lastActivity, cancellationToken);
            
            await eventPublisher.PublishAsync(new RoomChangedEvent
            {
                RoomId = room.Id,
                ActivityAt = DateTime.UtcNow
            }, cancellationToken);
            
            await eventBusManager.RoomAttenderAddedAsync(room.ToDto(), userId, cancellationToken);
            return Results.Ok();
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/v1/rooms/{id}/users/{userId}", Handler)
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithTags("Room");
        }
    }
}