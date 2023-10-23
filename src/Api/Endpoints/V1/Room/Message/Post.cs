using Api.Endpoints.V1.Models.Room.Message;
using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto.Room;
using Domain.Entities;
using Domain.Enum;
using Domain.Events.Contracts;
using Domain.Events.Room;
using Domain.Extensions;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room.Message
{
    public class Post : IEndpoint
    {
        private static async Task<IResult> Handler([FromRoute] string id,
            [FromBody] MessageCreateRequestModel request,
            [FromServices] IApiContext apiContext,
            [FromServices] IRoomRepository roomRepository,
            [FromServices] IMessageRepository messageRepository,
            [FromServices] IRoomLastActivityRepository roomLastActivityRepository,
            [FromServices] IUserRoomRepository userRoomRepository,
            [FromServices] IEventPublisher eventPublisher,
            [FromServices] IEventBusManager eventBusManager,
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

            var utcNow = DateTime.UtcNow;

            var messageId = utcNow.ToUnixTimeMilliseconds().ToString();
            var messageEntity = new MessageEntity
            {
                Id = messageId,
                Body = request.Body,
                CreatedAt = utcNow,
                MessageReactions = new List<MessageEntity.MessageReactionDataModel>(),
                MessageStatus = room.Attenders.Where(q => q != apiContext.CurrentUserId).Select(q => new MessageEntity.MessageStatusDataModel
                {
                    Status = MessageStatus.Delivered,
                    CreatedUtc = utcNow,
                    TargetId = q
                }).ToList(),
                RoomId = id,
                SenderId = apiContext.CurrentUserId,
                ThreadId = null,
                MessageAttachment = request.Attachment == null
                    ? null
                    : new MessageEntity.MessageAttachmentDataModel
                    {
                        Type = request.Attachment.Type,
                        Payload = request.Attachment.Payload,
                        AdditionalData = request.Attachment.AdditionalData
                    },
            };
            await messageRepository.SaveMessageAsync(messageEntity, cancellationToken);

            room.LastMessageInfo.Add(messageId);
            room.LastMessageInfo = room.LastMessageInfo.TakeLast(3).ToList();
            await roomRepository.SaveRoomAsync(room, cancellationToken);
            var roomLastActivity = await roomLastActivityRepository.GetRoomLastActivityAsync(room.Id, cancellationToken);
            var lastActivity = roomLastActivity?.LastActivityAt ?? utcNow;
            await userRoomRepository.SaveBatchAsync(room.Id, room.Attenders, lastActivity, cancellationToken);

            await eventPublisher.PublishAsync(new RoomChangedEvent
            {
                RoomId = id,
                ActivityAt = utcNow,
                HasNewMessage = true
            }, cancellationToken);
            await eventBusManager.RoomMessageAddedAsync(room.ToDto(), messageEntity.ToDto(), cancellationToken);
            return Results.Ok();
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/v1/rooms/{id}/messages", Handler)
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithTags("Room");
        }
    }
}