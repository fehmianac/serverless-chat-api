using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Entities;
using Domain.Options;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Endpoints.V1.Room.Video.Attender.Join;

public class Post: IEndpoint
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

        if (room.VideoCallAttender.All(q => q.UserId != apiContext.CurrentUserId))
        {
            room.VideoCallAttender.Add(new RoomEntity.VideoCallAttenderDataModel
            {
                UserId = apiContext.CurrentUserId,
                IsCreator = false
            });
            await roomRepository.SaveRoomAsync(room, cancellationToken);
        }
        var token = new AgoraIO.Rtc.RtcTokenBuilder().BuildToken(agoraSettings.Value.AppId, agoraSettings.Value.AppCertificate, id, true, 0);
        return Results.Ok(token);
    }
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/v1/room/{id}/video/join", Handler)
            .Produces<string>()
            .WithTags("Video");
    }
}