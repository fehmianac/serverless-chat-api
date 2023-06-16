using Amazon.DynamoDBv2;
using Domain.Constant;
using Domain.Entities;
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

        public async Task<(IList<MessageEntity>, string)> GetMessagePagedAsync(string roomId, string? nextToken, int? limit, long? ignoreBeforeTimeStamp, CancellationToken cancellationToken = default)
        {
            if (ignoreBeforeTimeStamp.HasValue)
            {
                var (userRooms, token, _) = await GetPagedAsync<MessageEntity>($"messages#{roomId}", SkOperator.GreaterThan, ignoreBeforeTimeStamp.Value.ToString(), nextToken, limit, cancellationToken);
                return (userRooms, token);
            }
            else
            {
                var (userRooms, token, _) = await GetPagedAsync<MessageEntity>($"messages#{roomId}", nextToken, limit, cancellationToken);
                return (userRooms, token);
            }
        }

        public async Task<bool> SaveMessageAsync(MessageEntity message, CancellationToken cancellationToken = default)
        {
            return await base.SaveAsync(message, cancellationToken);
        }

        public async Task<IList<MessageEntity>> GetBatchAsync(IEnumerable<MessageEntity> messageEntities, CancellationToken cancellationToken)
        {
            return await base.BatchGetAsync(messageEntities.ToList(), cancellationToken);
        }

        public async Task<MessageEntity?> GetMessageAsync(string id, string messageId, CancellationToken cancellationToken)
        {
            return await base.GetAsync<MessageEntity?>($"messages#{id}", messageId, cancellationToken);
        }
    }
}