using System.Web;
using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto;
using Domain.Dto.Room;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room.Message
{
    public class GetPaged : IEndpoint
    {
        private static async Task<IResult> Handler(
            [FromRoute] string id,
            [FromQuery] string? nextToken,
            [FromQuery] int limit,
            [FromServices] IApiContext apiContext,
            [FromServices] IRoomRepository roomRepository,
            [FromServices] IMessageRepository messageRepository,
            [FromServices] IDeletedMessageRepository deletedMessageRepository,
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

            var deletedMessagesResult =
                await deletedMessageRepository.GetDeletedMessagesAsync(apiContext.CurrentUserId, id, cancellationToken);
            var deletedMessagesHashSet = new HashSet<string>(deletedMessagesResult.Select(q => q.MessageId));
            var clearRoomResult = await clearRoomRepository.GetAsync(apiContext.CurrentUserId, id, cancellationToken);
            long? lastClearTime = null;
            if (clearRoomResult != null)
            {
                lastClearTime = clearRoomResult.Time;
            }

            var (messages, nextTokenResult) =
                await messageRepository.GetMessagePagedAsync(id, nextToken, limit, lastClearTime, cancellationToken);

            var messageResult = messages.Select(q => q.ToDto()).ToList();
            foreach (var messageDto in messageResult)
            {
                if (!deletedMessagesHashSet.Contains(messageDto.Id))
                {
                    continue;
                }

                messageDto.Body = "Message deleted";
                messageDto.IsDeleted = true;
                if (messageDto.MessageAttachment != null)
                {
                    messageDto.MessageAttachment.Payload = "attachment deleted";
                }
            }
            
            return Results.Ok(new PagedResponse<MessageDto>
            {
                Data = messageResult,
                Limit = limit,
                NextToken = nextTokenResult,
                PreviousToken = HttpUtility.UrlEncode(nextToken)
            });
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("v1/rooms/{id}/messages", Handler)
                .Produces<PagedResponse<MessageDto>>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithTags("Room");
        }
    }
}