using Amazon.DynamoDBv2;
using Domain.Constant;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class ClearRoomRepository : DynamoRepository, IClearRoomRepository
{
    public ClearRoomRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    protected override string GetTableName() => TableNames.TableName;

    public async Task<bool> SaveAsync(ClearRoomEntity clearRoom, CancellationToken cancellationToken = default)
    {
        return await base.SaveAsync(clearRoom, cancellationToken);
    }

    public async Task<ClearRoomEntity?> GetAsync(string userId, string roomId, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<ClearRoomEntity?>($"clearRoom#{userId}", roomId, cancellationToken);
    }
}