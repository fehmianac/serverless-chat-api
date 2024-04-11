using Api.Infrastructure.Context;
using Api.Infrastructure.Contract;
using Domain.Dto.Room;
using Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.V1.Room
{
    public class Get : IEndpoint
    {
        private static async Task<IResult> Handler(
            [FromRoute] string id,
            [FromServices] IApiContext apiContext,
            [FromServices] IRoomRepository roomRepository,
            [FromServices] IUserBanRepository userBanRepository,
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
            
            if (!room.IsGroup)
            {
                var otherAttender = room.Attenders.FirstOrDefault(q => q != apiContext.CurrentUserId);
                if (otherAttender != null)
                {
                    var banInfo = await userBanRepository.GetBannedInfoAsync(apiContext.CurrentUserId, otherAttender,
                        cancellationToken);
                    if (banInfo.Any())
                    {
                        return Results.Ok(room.ToDto(banInfo));
                    }
                }
            }
            return Results.Ok(room.ToDto());
        }

        public void MapEndpoint(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("v1/rooms/{id}", Handler)
                .Produces<RoomDto>()
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .ProducesProblem(StatusCodes.Status403Forbidden)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status500InternalServerError)
                .WithTags("Room");
        }
    }
}