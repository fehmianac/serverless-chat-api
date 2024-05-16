using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto;
using Domain.Dto.Room;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room
{
    public class GetPaged : IEndpoint
    {
        private static async Task<IResult> Handler(
            [FromQuery] int limit,
            [FromQuery] string? nextToken,
            [FromServices] IApiContext apiContext,
            [FromServices] IUserRoomRepository userRoomRepository,
            [FromServices] IRoomRepository roomRepository,
            [FromServices] IMessageRepository messageRepository,
            [FromServices] IRoomNotificationRepository roomNotificationRepository,
            CancellationToken cancellationToken)
        {
            var userId = apiContext.CurrentUserId;
            var (userRooms, token) =
                await userRoomRepository.GetPagedAsync(userId, limit, nextToken, cancellationToken);
            if (!userRooms.Any())
            {
                return Results.Ok(new PagedResponse<RoomDto>
                {
                    Data = new List<RoomDto>(),
                    Limit = limit,
                    NextToken = null,
                    PreviousToken = nextToken
                });
            }

            var rooms = await roomRepository.GetUserRoomsAsync(userRooms.Select(x => x.RoomId).Distinct().ToList(),
                cancellationToken);

            var messageEntities = rooms.SelectMany(q => q.LastMessageInfo.Select(x => new MessageEntity
            {
                RoomId = q.Id,
                Id = x
            }));


            var messages = await messageRepository.GetBatchAsync(messageEntities, cancellationToken);
            var roomResult = rooms.Select(x => x.ToDto()).ToList();
            foreach (var roomDto in roomResult)
            {
                var roomMessages = messages.Where(q => q.RoomId == roomDto.Id).ToList();
                if (!roomMessages.Any())
                    continue;

                roomDto.LastMessageInfo.AddRange(
                    roomMessages.Select(q => q.ToDto()).OrderByDescending(q => q.CreatedAt));
            }

            var roomNotificationList =
                await roomNotificationRepository.GetRoomNotificationAsync(userId, cancellationToken);
            foreach (var roomDto in roomResult)
            {
                var notification = roomNotificationList.FirstOrDefault(q => q.RoomId == roomDto.Id);
                if (notification == null)
                    continue;

                roomDto.UnReadMessageCount = notification.MessageCount;
                roomDto.HasNotification = notification.HasNotification;
            }

            return Results.Ok(new PagedResponse<RoomDto>
            {
                Data = roomResult
                    .OrderByDescending(q => userRooms.Where(x => x.RoomId == q.Id).Max(x => x.LastActivityAt)).ToList(),
                Limit = limit,
                NextToken = token,
                PreviousToken = nextToken
            });
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("v1/rooms", Handler)
                .Produces<PagedResponse<RoomDto>>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithTags("Room");
        }
    }
}