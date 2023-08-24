using Domain.Dto.Room;

namespace Domain.Services;

public interface IEventBusManager
{
    Task<bool> RoomCreatedAsync(RoomDto room, CancellationToken cancellationToken = default);
    Task<bool> RoomModifiedAsync(RoomDto room, CancellationToken cancellationToken = default);

    Task<bool> RoomAttenderAddedAsync(RoomDto room, string userId, CancellationToken cancellationToken = default);
    Task<bool> RoomAttenderRemovedAsync(RoomDto room, string userId, CancellationToken cancellationToken = default);

    Task<bool> RoomAdminAddedAsync(RoomDto room, string userId, CancellationToken cancellationToken = default);
    Task<bool> RoomAdminRemovedAsync(RoomDto room, string userId, CancellationToken cancellationToken = default);

    Task<bool> RoomMessageAddedAsync(RoomDto room, MessageDto message, CancellationToken cancellationToken = default);

    Task<bool> RoomMessageReactionAddedAsync(RoomDto room, string messageId, MessageDto.MessageReactionDto messageReaction, CancellationToken cancellationToken = default);
    Task<bool> RoomMessageReactionRemovedAsync(RoomDto room, string messageId, string messageReactionId, CancellationToken cancellationToken = default);
}