using Domain.Entities;

namespace Domain.Repositories
{
    public interface IMessageRepository
    {
        Task<(IList<MessageEntity>, string)> GetMessagePagedAsync(string roomId, string? nextToken, int? limit, long? ignoreBeforeTimeStamp, CancellationToken cancellationToken = default);
        Task<bool> SaveMessageAsync(MessageEntity message, CancellationToken cancellationToken = default);
        Task<IList<MessageEntity>> GetBatchAsync(IEnumerable<MessageEntity> messageEntities, CancellationToken cancellationToken);
        Task<MessageEntity?> GetMessageAsync(string id, string messageId, CancellationToken cancellationToken);
        Task<List<MessageEntity>> GetMessagesAsync(string id, List<string> messageIds, CancellationToken cancellationToken);
        Task<bool> SaveMessagesAsync(IList<MessageEntity> messages, CancellationToken cancellationToken);
    }
}