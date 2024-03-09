using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto.Notifier;
using Domain.Entities;
using Domain.Enum;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room.Message.Read;

public class Post : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromRoute] string id,
        [FromServices] IApiContext apiContext,
        [FromServices] IRoomNotificationRepository roomNotificationRepository,
        [FromServices] IMessageRepository messageRepository,
        [FromServices] IRoomRepository roomRepository,
        [FromServices] IPubSubServices pubSubServices,
        CancellationToken cancellationToken
    )
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

        
        var roomNotification =
            await roomNotificationRepository.GetRoomNotificationAsync(id, apiContext.CurrentUserId, cancellationToken);
        if (roomNotification == null)
        {
            return Results.Ok();
        }

        var messageEntities = roomNotification.MessageIds.Select(q => new MessageEntity
        {
            RoomId = id,
            Id = q
        });
        
        var messages = await messageRepository.GetBatchAsync(messageEntities, cancellationToken);
        foreach (var message in messages)
        {
            message.MessageStatus = message.MessageStatus.Select(q =>
            {
                if (q.TargetId != apiContext.CurrentUserId) return q;
                q.Status = MessageStatus.Read;
                q.CreatedUtc = DateTime.UtcNow;

                return q;
            }).ToList();
        }
        
        await messageRepository.SaveMessagesAsync(messages, cancellationToken);
        roomNotification.HasNotification = false;
        roomNotification.MessageCount = 0;
        roomNotification.MessageIds = new List<string>();

        var lastMessages = messages.MinBy(q => q.Id);
        await roomNotificationRepository.SaveRoomNotificationAsync(roomNotification, cancellationToken);
        await pubSubServices.NotifyUser(room.Attenders, new RoomMessageReadModel
        {
            RoomId = id,
            Type = "MessagesRead",
            LastMessageId = lastMessages?.Id??string.Empty,
        }, cancellationToken);
        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("v1/room/{id}/message/read", Handler)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithTags("Room");
    }
}