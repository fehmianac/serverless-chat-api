using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Entities;
using Domain.Extensions;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room;

public class Delete : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromQuery] string id,
        [FromServices] IApiContext apiContext,
        [FromServices] IRoomRepository roomRepository,
        [FromServices] IUserRoomRepository userRoomRepository,
        [FromServices] IRoomLastActivityRepository roomLastActivityRepository,
        [FromServices] IClearRoomRepository clearRoomRepository,
        CancellationToken cancellationToken)
    {
        var room = await roomRepository.GetRoomAsync(id, cancellationToken);
        if (room == null)
            return Results.NotFound();

        if (!room.Attenders.Contains(apiContext.CurrentUserId))
        {
            return Results.Forbid();
        }
        
        var roomLastActivity = await roomLastActivityRepository.GetRoomLastActivityAsync(id, cancellationToken);
        var utcNow = DateTime.UtcNow;
        var lastActivity = roomLastActivity?.LastActivityAt ?? utcNow;
        await userRoomRepository.DeleteUserRoomAsync(apiContext.CurrentUserId, id, lastActivity, cancellationToken);
        
        await clearRoomRepository.SaveAsync(new ClearRoomEntity
        {
            RoomId = id,
            UserId = apiContext.CurrentUserId,
            Time = DateTime.UtcNow.ToUnixTimeMilliseconds()
        }, cancellationToken);
        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("v1/rooms/{id}", Handler)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Room");
    }
}