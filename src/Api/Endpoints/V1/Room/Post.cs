using Api.Endpoints.V1.Models.Room;
using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto.Room;
using Domain.Entities;
using Domain.Events.Contracts;
using Domain.Events.Room;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room
{
    public class Post : IEndpoint
    {
        private static async Task<IResult> Handler(
            [FromBody] RoomCreateRequestModel request,
            [FromServices] IApiContext apiContext,
            [FromServices] IRoomRepository roomRepository,
            [FromServices] IUserRoomRepository userRoomRepository,
            [FromServices] IRoomLastActivityRepository roomLastActivityRepository,
            [FromServices] IEventPublisher eventPublisher,
            [FromServices] IEventBusManager eventBusManager,
            CancellationToken cancellationToken)
        {
            request.Attenders.Add(apiContext.CurrentUserId);
            request.Attenders = request.Attenders.Distinct().ToList();
            if (request.Attenders.Count > 2)
            {
                request.IsGroup = true;
            }
            
            var utcNow = DateTime.UtcNow;
            if (!request.IsGroup)
            {
                var roomId = await roomRepository.FindAndDeletePrivateRoomUserMappingAsync(request.Attenders, cancellationToken);
                if (!string.IsNullOrEmpty(roomId))
                {
                    foreach (var user in request.Attenders)
                    {
                        await userRoomRepository.DeleteUserRoomAsync(user, roomId, utcNow, cancellationToken);
                    }
                }
            }
          
            var room = new RoomEntity
            {
                Id = Guid.NewGuid().ToString("N"),
                Attenders = request.Attenders.Distinct().ToList(),
                Admins = new List<string> { apiContext.CurrentUserId },
                Description = request.Description,
                Name = request.Name,
                IsGroup = request.IsGroup,
                CreatedAt = utcNow,
                ImageUrl = request.ImageUrl,
                TypingAttenders = new List<RoomEntity.TypingAttenderDataModel>(),
                LastActivityAt = utcNow,
                LastMessageInfo = new List<string>()
            };

            var saveRoomTask = roomRepository.SaveRoomAsync(room, cancellationToken);
            var saveUserRoomTask =
                userRoomRepository.SaveBatchAsync(room.Id, room.Attenders,null, utcNow, cancellationToken);
            var saveRoomLastActivityTask =
                roomLastActivityRepository.SaveRoomLastActivityAsync(room.Id, utcNow, cancellationToken);

            var eventPublishTask = eventPublisher.PublishAsync(new RoomCreatedEvent
            {
                Room = room.ToDto()
            }, cancellationToken);

            await eventBusManager.RoomCreatedAsync(room.ToDto(), cancellationToken);
            await Task.WhenAll(saveRoomTask, saveUserRoomTask, saveRoomLastActivityTask, eventPublishTask);
            if (!request.IsGroup)
            {
                await roomRepository.SavePrivateRoomUserMappingAsync(room.Id, request.Attenders, cancellationToken);
            }

            return Results.Created($"/v1/rooms/{room.Id}", room.Id);
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("v1/rooms", Handler)
                .Produces<string>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithTags("Room");
        }
    }
}