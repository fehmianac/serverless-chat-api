using Api.Endpoints.V1.Models.Room.Message;
using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto.Room;
using Domain.Entities;
using Domain.Events.Contracts;
using Domain.Events.Room;
using Domain.Repositories;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room.Message.Reaction
{
    public class Post : IEndpoint
    {
        private static async Task<IResult> Handler([FromRoute] string id,
            [FromRoute] string messageId,
            [FromBody] ReactionRequestModel request,
            [FromServices] IApiContext apiContext,
            [FromServices] IRoomRepository roomRepository,
            [FromServices] IMessageRepository messageRepository,
            [FromServices] IEventPublisher eventPublisher,
            [FromServices] IEventBusManager eventBusManager,
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

            var message = await messageRepository.GetMessageAsync(id, messageId, cancellationToken);
            if (message == null)
            {
                return Results.NotFound();
            }

            if (message.MessageReactions.Any(q => q.UserId == apiContext.CurrentUserId && q.Reaction == request.Reaction))
            {
                return Results.Ok();
            }

            var utcNow = DateTime.UtcNow;
            var reactionEntity = new MessageEntity.MessageReactionDataModel
            {
                Reaction = request.Reaction,
                UserId = apiContext.CurrentUserId,
                Time = utcNow
            };
            message.MessageReactions.Add(reactionEntity);
            await messageRepository.SaveMessageAsync(message, cancellationToken);

            await eventPublisher.PublishAsync(new RoomChangedEvent
            {
                RoomId = room.Id,
                ActivityAt = utcNow,
                Message = message.ToDto()
            }, cancellationToken);

            await eventBusManager.RoomMessageReactionAddedAsync(room.ToDto(), message.Id, new MessageDto.MessageReactionDto
            {
                Reaction = request.Reaction,
                Time = utcNow,
                UserId = apiContext.CurrentUserId
            }, cancellationToken);
            return Results.Ok();
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/v1/rooms/{id}/messages/{messageId}/reactions", Handler)
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithTags("Room");
        }
    }
}