using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room.Message
{
    public class Delete : IEndpoint
    {
        private static async Task<IResult> Handler([FromRoute] string id,
            [FromRoute] string messageId,
            [FromBody] DeleteMessageRequestModel model,
            [FromServices] IApiContext apiContext,
            [FromServices] IRoomRepository roomRepository,
            [FromServices] IMessageRepository messageRepository,
            [FromServices] IDeletedMessageRepository deletedMessageRepository,
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

            if(message.SenderId == apiContext.CurrentUserId && model.DeleteForAll)
            {
                foreach (var attender in room.Attenders)
                {
                    await deletedMessageRepository.SaveDeletedMessageAsync(new DeletedMessageEntity
                    {
                        MessageId = messageId,
                        RoomId = id,
                        UserId = attender
                    }, cancellationToken);
                }
            }
            else
            {
                await deletedMessageRepository.SaveDeletedMessageAsync(new DeletedMessageEntity
                {
                    MessageId = messageId,
                    RoomId = id,
                    UserId = apiContext.CurrentUserId
                }, cancellationToken); 
            }
            
            return Results.Ok();
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("v1/rooms/{id}/messages/{messageId}", Handler)
                .Produces(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .WithTags("Room");
        }
    }
    public record DeleteMessageRequestModel(bool DeleteForAll);
}