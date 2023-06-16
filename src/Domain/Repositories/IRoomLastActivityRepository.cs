using Domain.Entities;

namespace Domain.Repositories;

public interface IRoomLastActivityRepository
{
    Task<RoomLastActivityEntity?> GetRoomLastActivityAsync(string roomId, CancellationToken cancellationToken = default);
    Task<bool> SaveRoomLastActivityAsync(string roomId, DateTime lastActivity, CancellationToken cancellationToken = default);
}