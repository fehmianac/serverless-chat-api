using Amazon.DynamoDBv2;
using Domain.Constant;
using Domain.Entities;
using Domain.Entities.Base;
using Domain.Enum;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class MessageRepository : DynamoRepository, IMessageRepository
    {
        public MessageRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
        {
        }

        protected override string GetTableName() => TableNames.TableName;

        public async Task<(IList<MessageEntity>, string)> GetMessagePagedAsync(string roomId, string? nextToken,
            int? limit, long? ignoreBeforeTimeStamp, CancellationToken cancellationToken = default)
        {
            if (ignoreBeforeTimeStamp.HasValue)
            {
                var (userRooms, token, _) = await GetPagedAsync<MessageEntity>($"messages#{roomId}",
                    SkOperator.GreaterThan, ignoreBeforeTimeStamp.Value.ToString(), nextToken, limit,
                    cancellationToken);
                return (userRooms, token);
            }
            else
            {
                var (userRooms, token, _) =
                    await GetPagedAsync<MessageEntity>($"messages#{roomId}", nextToken, limit, cancellationToken);
                return (userRooms, token);
            }
        }

        public async Task<bool> SaveMessageAsync(MessageEntity message, CancellationToken cancellationToken = default)
        {
            return await SaveAsync(message, cancellationToken);
        }

        public async Task<IList<MessageEntity>> GetBatchAsync(IEnumerable<MessageEntity> messageEntities,
            CancellationToken cancellationToken)
        {
            return await BatchGetAsync(messageEntities.ToList(), cancellationToken);
        }

        public async Task<MessageEntity?> GetMessageAsync(string id, string messageId,
            CancellationToken cancellationToken)
        {
            return await GetAsync<MessageEntity?>($"messages#{id}", messageId, cancellationToken);
        }

        public async Task<List<MessageEntity>> GetMessagesAsync(string id, List<string> messageIds, CancellationToken cancellationToken)
        {
            return await BatchGetAsync(messageIds.Select(q => new MessageEntity
            {
                RoomId = id,
                Id = q
            }).ToList(), cancellationToken);
        }

        public async Task<bool> SaveMessagesAsync(IList<MessageEntity> messages, CancellationToken cancellationToken)
        {
             await base.BatchWriteAsync(new List<IEntity>(messages), new List<IEntity>(), cancellationToken);
             return true;
        }
    }
}