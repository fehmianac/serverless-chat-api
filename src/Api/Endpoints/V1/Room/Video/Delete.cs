using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Entities;
using Domain.Options;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Endpoints.V1.Room.Video;

public class Delete : IEndpoint
{
    private static async Task<IResult> Handler(
        [FromRoute] string id,
        [FromServices] IApiContext apiContext,
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

        room.VideoCallIsActive = false;
        room.VideoCallAttender = new();

        await roomRepository.SaveRoomAsync(room, cancellationToken);

        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/v1/room/{id}/video", Handler)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags("Video");
    }
}