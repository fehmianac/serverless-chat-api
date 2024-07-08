using Amazon.DynamoDBv2;
using Domain.Constant;
using Domain.Entities;
using Domain.Entities.Base;
using Domain.Extensions;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class UserRoomRepository : DynamoRepository, IUserRoomRepository
{
    public UserRoomRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    protected override string GetTableName() => TableNames.TableName;

    public async Task<bool> SaveBatchAsync(string roomId, List<string> userIds, DateTime? oldActivity,
        DateTime lastActivity, CancellationToken cancellationToken = default)
    {
        var entities = userIds.Select(q => new UserRoomEntity
        {
            RoomId = roomId,
            UserId = q,
            LastActivityAt = lastActivity
        }).ToList();
        var oldEntities = new List<UserRoomEntity>();
        if (oldActivity.HasValue)
        {
            oldEntities = userIds.Select(q => new UserRoomEntity
            {
                RoomId = roomId,
                UserId = q,
                LastActivityAt = oldActivity.Value
            }).ToList();
        }

        await base.BatchWriteAsync(new List<IEntity>(entities), new List<IEntity>(oldEntities), cancellationToken);
        return true;
    }

    public async Task<(List<UserRoomEntity> userRooms, string token)> GetPagedAsync(string userId, int limit,
        string? nextToken, CancellationToken cancellationToken)
    {
        var (userRooms, token, _) =
            await GetPagedAsync<UserRoomEntity>($"userRoom#{userId}", nextToken, limit, cancellationToken);
        return (userRooms, token);
    }

    public new async Task BatchWriteAsync(List<IEntity> putItemList, List<IEntity> deleteItemList,
        CancellationToken cancellationToken)
    {
        await base.BatchWriteAsync(putItemList, deleteItemList, cancellationToken);
    }

    public async Task<bool> DeleteUserRoomAsync(string userId, string roomId, DateTime lastActivity,
        CancellationToken cancellationToken)
    {
        
        return await DeleteAsync($"userRoom#{userId}", $"{lastActivity.ToUnixTimeMilliseconds()}#{roomId}",
            cancellationToken);
    }
}