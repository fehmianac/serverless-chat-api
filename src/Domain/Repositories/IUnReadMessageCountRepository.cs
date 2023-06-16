using Domain.Entities;

namespace Domain.Repositories
{
    public interface IRoomNotificationRepository
    {
        Task<IList<RoomNotificationEntity>> GetRoomNotificationAsync(string userId, CancellationToken cancellationToken = default);
        Task<RoomNotificationEntity?> GetRoomNotificationAsync(string userId, string roomId, CancellationToken cancellationToken = default);
        Task<bool> SaveRoomNotificationAsync(RoomNotificationEntity roomNotification, CancellationToken cancellationToken = default);
        Task<bool> DeleteRoomNotificationAsync(string userId, string roomId, CancellationToken cancellationToken = default);

        Task<List<RoomNotificationEntity>> GetBatchRoomNotificationAsync(List<string> userIds, string roomId, CancellationToken cancellationToken = default);

        Task<bool> SaveBatchRoomNotificationAsync(List<RoomNotificationEntity> roomNotification, CancellationToken cancellationToken = default);
    }
}