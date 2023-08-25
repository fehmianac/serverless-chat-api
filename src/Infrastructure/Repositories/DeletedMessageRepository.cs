using Amazon.DynamoDBv2;
using Domain.Constant;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class DeletedMessageRepository : DynamoRepository, IDeletedMessageRepository
    {
        public DeletedMessageRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
        {
        }

        protected override string GetTableName() => TableNames.TableName;

        public async Task<bool> SaveDeletedMessageAsync(DeletedMessageEntity deletedMessage, CancellationToken cancellationToken = default)
        {
            return await SaveAsync(deletedMessage, cancellationToken);
        }

        public async Task<IEnumerable<DeletedMessageEntity>> GetDeletedMessagesAsync(string userId, string roomId, CancellationToken cancellationToken = default)
        {
            return await GetAllAsync<DeletedMessageEntity>($"deletedMessage#{userId}#{roomId}", cancellationToken);
        }
    }
}