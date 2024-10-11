using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Entities;
using Domain.Enum;
using Domain.Extensions;
using Domain.Options;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Endpoints.V1.Room.Video;

public class Post : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromRoute] string id,
        [FromServices] IApiContext apiContext,
        [FromServices] IMessageRepository messageRepository,
        [FromServices] IRoomLastActivityRepository roomLastActivityRepository,
        [FromServices] IUserRoomRepository userRoomRepository,
        [FromServices] IRoomRepository roomRepository,
        [FromServices] IUserBanRepository userBanRepository,
        [FromServices] IOptionsSnapshot<AgoraSettings> agoraSettings,
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
        var token = new AgoraIO.Rtc.RtcTokenBuilder().BuildToken(agoraSettings.Value.AppId, agoraSettings.Value.AppCertificate, id, true, 0);
        room.VideoCallIsActive = true;
        room.VideoCallAttender =
        [
            new RoomEntity.VideoCallAttenderDataModel
            {
                UserId = apiContext.CurrentUserId,
                IsCreator = true
            }
        ];
        
        var utcNow = DateTime.UtcNow;
        var messageId = utcNow.ToUnixTimeMilliseconds().ToString();
        var messageEntity = new MessageEntity
        {
            Id = messageId,
            Body = "",
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
            MessageAttachment = new MessageEntity.MessageAttachmentDataModel
            {
                Type = "VideoCall"
            },
            ThreadId = null
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

        await roomRepository.SaveRoomAsync(room, cancellationToken);
        return Results.Ok(token);
    }
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/v1/room/{id}/video", Handler)
            .Produces<string>()
            .WithTags("Video");
    }
}