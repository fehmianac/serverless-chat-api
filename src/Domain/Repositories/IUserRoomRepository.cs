using Domain.Entities;
using Domain.Entities.Base;

namespace Domain.Repositories
{
    public interface IUserRoomRepository
    {
        Task<bool> SaveBatchAsync(string roomId, List<string> userIds,DateTime? oldActivity, DateTime lastActivity, CancellationToken cancellationToken = default);
        Task<(List<UserRoomEntity> userRooms, string token)> GetPagedAsync(string userId, int limit, string? nextToken, CancellationToken cancellationToken);
        Task BatchWriteAsync(List<IEntity> putItemList, List<IEntity> deleteItemList, CancellationToken cancellationToken);
        Task<bool> DeleteUserRoomAsync(string userId, string id, DateTime lastActivity, CancellationToken cancellationToken);
    }
}