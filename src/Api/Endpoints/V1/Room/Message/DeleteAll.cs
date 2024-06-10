using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Entities;
using Domain.Extensions;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room.Message
{
    public class DeleteAll : IEndpoint
    {
        private static async Task<IResult> Handler([FromRoute] string id,
            [FromServices] IApiContext apiContext,
            [FromServices] IRoomRepository roomRepository,
            [FromServices] IMessageRepository messageRepository,
            [FromServices] IClearRoomRepository clearRoomRepository,
            [FromServices] IRoomNotificationRepository roomNotificationRepository,
            CancellationToken cancellationToken)
        {
            var room = await roomRepository.GetRoomAsync(id, cancellationToken);
            if (room == null)
            {
                return Results.NotFound();
            }

            if (!room.Attenders.Contains(apiContext.CurrentUserId))
            {
                return Results.Forbid();
            }

            await clearRoomRepository.SaveAsync(new ClearRoomEntity
            {
                RoomId = id,
                UserId = apiContext.CurrentUserId,
                Time = DateTime.UtcNow.ToUnixTimeMilliseconds()
            }, cancellationToken);
            
            var unreadMessages = await roomNotificationRepository.GetRoomNotificationAsync(apiContext.CurrentUserId,id,cancellationToken);
            
            if (unreadMessages == null) 
                return Results.Ok();
            
            unreadMessages.MessageCount = 0;
            unreadMessages.MessageIds.Clear();
            await roomNotificationRepository.SaveRoomNotificationAsync(unreadMessages, cancellationToken);

            return Results.Ok();
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("v1/rooms/{id}/messages", Handler)
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithTags("Room");
            
        }
    }
}