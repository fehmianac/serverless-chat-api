using Domain.Entities;

namespace Domain.Repositories
{
    public interface IUserBanRepository
    {
        Task<bool> IsBannedAsync(string fromUserId, string toUserId, CancellationToken cancellationToken = default);
    
        Task<bool> SaveBanAsync(string fromUserId, string toUserId, CancellationToken cancellationToken = default);
    
        Task<bool> DeleteBanAsync(string fromUserId, string toUserId, CancellationToken cancellationToken = default);
    
        Task<IEnumerable<string>> GetBannedUsersAsync(string fromUserId, CancellationToken cancellationToken = default);
        
        Task<List<UserBanEntity>> GetBannedInfoAsync(string from,string to, CancellationToken cancellationToken = default);
    }
}