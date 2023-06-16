using Amazon.DynamoDBv2;
using Domain.Constant;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class RoomLastActivityRepository : DynamoRepository, IRoomLastActivityRepository
{
    public RoomLastActivityRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    protected override string GetTableName() => TableNames.TableName;

    public async Task<RoomLastActivityEntity?> GetRoomLastActivityAsync(string roomId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<RoomLastActivityEntity>("roomLastActivity", roomId, cancellationToken);
    }

    public async Task<bool> SaveRoomLastActivityAsync(string roomId, DateTime lastActivity, CancellationToken cancellationToken = default)
    {
        return await SaveAsync(new RoomLastActivityEntity
        {
            RoomId = roomId,
            LastActivityAt = lastActivity
        }, cancellationToken);
    }
}