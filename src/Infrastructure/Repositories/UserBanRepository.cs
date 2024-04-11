using Amazon.DynamoDBv2;
using Domain.Constant;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class UserBanRepository : DynamoRepository, IUserBanRepository
{
    public UserBanRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
    {
    }

    protected override string GetTableName() => TableNames.TableName;

    public async Task<bool> IsBannedAsync(string fromUserId, string toUserId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<UserBanEntity>($"userBan#{fromUserId}", toUserId, cancellationToken) != null;
    }

    public async Task<bool> SaveBanAsync(string fromUserId, string toUserId, CancellationToken cancellationToken = default)
    {
        return await SaveAsync(new UserBanEntity
        {
            FromUserId = fromUserId,
            ToUserId = toUserId,
            CreatedUtc = DateTime.UtcNow
        }, cancellationToken);
    }

    public async Task<bool> DeleteBanAsync(string fromUserId, string toUserId, CancellationToken cancellationToken = default)
    {
        return await DeleteAsync($"userBan#{fromUserId}", toUserId, cancellationToken);
    }

    public async Task<IEnumerable<string>> GetBannedUsersAsync(string fromUserId, CancellationToken cancellationToken = default)
    {
        var bans = await GetAllAsync<UserBanEntity>($"userBan#{fromUserId}", cancellationToken);
        return bans.Select(q => q.ToUserId);
    }

    public async Task<List<UserBanEntity>> GetBannedInfoAsync(string from, string to, CancellationToken cancellationToken = default)
    {
        return await BatchGetAsync(new List<UserBanEntity>
        {
            new()
            {
                FromUserId = from,
                ToUserId = to
            },
            new()
            {
                FromUserId = to,
                ToUserId = from
            }
        }, cancellationToken);
    }
}