using Domain.Entities;

namespace Domain.Repositories
{
    public interface IDeletedMessageRepository
    {
        Task<bool> SaveDeletedMessageAsync(DeletedMessageEntity deletedMessage, CancellationToken cancellationToken = default);
    
        Task<IEnumerable<DeletedMessageEntity>> GetDeletedMessagesAsync(string userId, string roomId, CancellationToken cancellationToken = default);
    }
}