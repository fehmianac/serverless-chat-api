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
            [FromServices] IUserBanRepository banRepository,
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

            if (!room.IsGroup)
            {
                var otherAttender = room.Attenders.FirstOrDefault(q => q != apiContext.CurrentUserId);
                if (otherAttender != null)
                {
                    var banInfo = await banRepository.GetBannedInfoAsync(apiContext.CurrentUserId, otherAttender,
                        cancellationToken);
                    if (banInfo.Any())
                        return Results.Forbid();
                }
            }

            var utcNow = DateTime.UtcNow;

            var messageId = utcNow.ToUnixTimeMilliseconds().ToString();
            var messageEntity = new MessageEntity
            {
                Id = messageId,
                Body = request.Body,
                CreatedAt = utcNow,
                MessageReactions = new List<MessageEntity.MessageReactionDataModel>(),
                MessageStatus = room.Attenders.Where(q => q != apiContext.CurrentUserId).Select(q =>
                    new MessageEntity.MessageStatusDataModel
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

            var lastMessages = room.LastMessageInfo.TakeLast(2).ToList();
            lastMessages.Add(messageId);
            room.LastMessageInfo = lastMessages;
            room.LastActivityAt = utcNow;
            await roomRepository.SaveRoomAsync(room, cancellationToken);
            var roomLastActivity =
                await roomLastActivityRepository.GetRoomLastActivityAsync(room.Id, cancellationToken);

            await userRoomRepository.SaveBatchAsync(room.Id, room.Attenders, roomLastActivity?.LastActivityAt, utcNow,
                cancellationToken);

            await eventPublisher.PublishAsync(new RoomChangedEvent
            {
                RoomId = id,
                ActivityAt = utcNow,
                HasNewMessage = true,
                MessageId = messageId,
                Message = messageEntity.ToDto()
            }, cancellationToken);
            await eventBusManager.RoomMessageAddedAsync(room.ToDto(), messageEntity.ToDto(), cancellationToken);
            return Results.Ok(messageId);
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/v1/rooms/{id}/messages", Handler)
                .Produces<string>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithTags("Room");
        }
    }
}