using Domain.Entities;

namespace Domain.Repositories;

public interface IClearRoomRepository
{
    Task<bool> SaveAsync(ClearRoomEntity clearRoom, CancellationToken cancellationToken = default);

    Task<ClearRoomEntity?> GetAsync(string userId, string roomId, CancellationToken cancellationToken = default);
}