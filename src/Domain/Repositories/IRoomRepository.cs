using Domain.Entities;

namespace Domain.Repositories
{
    public interface IRoomRepository
    {
        Task<IList<RoomEntity>> GetUserRoomsAsync(IEnumerable<string> roomIds, CancellationToken cancellationToken = default);

        Task<bool> SaveRoomAsync(RoomEntity room, CancellationToken cancellationToken = default);

        Task<RoomEntity?> GetRoomAsync(string roomId, CancellationToken cancellationToken = default);

        Task<bool> DeleteRoomAsync(string roomId, CancellationToken cancellationToken = default);

        Task<bool> UpdateLastActivityAtAsync(string roomId, DateTime lastActivityAt, CancellationToken cancellationToken = default);
        
        Task<string?> FindPrivateRoomUserMappingAsync(List<string> attenders, CancellationToken cancellationToken = default);
        
        Task<bool> SavePrivateRoomUserMappingAsync(string roomId, List<string> attenders, CancellationToken cancellationToken = default);

    }
}