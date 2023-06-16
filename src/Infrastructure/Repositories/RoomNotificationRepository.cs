using Amazon.DynamoDBv2;
using Domain.Constant;
using Domain.Entities;
using Domain.Entities.Base;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class RoomNotificationRepository : DynamoRepository, IRoomNotificationRepository
{
    public RoomNotificationRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    protected override string GetTableName() => TableNames.TableName;

    public async Task<IList<RoomNotificationEntity>> GetRoomNotificationAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await base.GetAllAsync<RoomNotificationEntity>($"roomNotification#{userId}", cancellationToken);
    }

    public async Task<RoomNotificationEntity?> GetRoomNotificationAsync(string userId, string roomId, CancellationToken cancellationToken = default)
    {
        return await base.GetAsync<RoomNotificationEntity>($"roomNotification#{userId}", roomId, cancellationToken);
    }

    public async Task<bool> SaveRoomNotificationAsync(RoomNotificationEntity roomNotification, CancellationToken cancellationToken = default)
    {
        return await base.SaveAsync(roomNotification, cancellationToken);
    }

    public async Task<bool> DeleteRoomNotificationAsync(string userId, string roomId, CancellationToken cancellationToken = default)
    {
        return await base.DeleteAsync($"roomNotification#{userId}", roomId, cancellationToken);
    }

    public async Task<List<RoomNotificationEntity>> GetBatchRoomNotificationAsync(List<string> userIds, string roomId, CancellationToken cancellationToken = default)
    {
        return await base.BatchGetAsync(userIds.Select(q => new RoomNotificationEntity
        {
            UserId = q,
            RoomId = roomId
        }).ToList(), cancellationToken);
    }

    public async Task<bool> SaveBatchRoomNotificationAsync(List<RoomNotificationEntity> roomNotification, CancellationToken cancellationToken = default)
    {
        await base.BatchWriteAsync(new List<IEntity>(roomNotification), new List<IEntity>(), cancellationToken);
        return true;
    }
}