using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Options;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Endpoints.V1.Room.Video.Attender.Left;

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

        room.VideoCallAttender.RemoveAll(q => q.UserId == apiContext.CurrentUserId);
        room.VideoCallIsActive = room.VideoCallAttender.Any();

        await roomRepository.SaveRoomAsync(room, cancellationToken);
        return Results.Ok();
    }

    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/v1/room/{id}/video/left", Handler)
            .Produces<string>()
            .WithTags("Video");
    }
}