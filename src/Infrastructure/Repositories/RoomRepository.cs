using Amazon.DynamoDBv2;
using Domain.Constant;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories
{
    public class RoomRepository : DynamoRepository, IRoomRepository
    {
        public RoomRepository(IAmazonDynamoDB dynamoDb) : base(dynamoDb)
        {
        }

        public async Task<IList<RoomEntity>> GetUserRoomsAsync(IEnumerable<string> roomIds, CancellationToken cancellationToken = default)
        {
            var items = roomIds.Select(q => new RoomEntity
            {
                Id = q
            }).ToList();
            var rooms = await BatchGetAsync(items, cancellationToken);
            return rooms;
        }

        public async Task<bool> SaveRoomAsync(RoomEntity room, CancellationToken cancellationToken = default)
        {
            return await SaveAsync(room, cancellationToken);
        }

        public async Task<RoomEntity?> GetRoomAsync(string roomId, CancellationToken cancellationToken = default)
        {
            return await GetAsync<RoomEntity>("rooms", roomId, cancellationToken);
        }

        public async Task<bool> DeleteRoomAsync(string roomId, CancellationToken cancellationToken = default)
        {
            return await DeleteAsync("rooms", roomId, cancellationToken);
        }

        public async Task<bool> UpdateLastActivityAtAsync(string roomId, DateTime lastActivityAt, CancellationToken cancellationToken = default)
        {
            var room = await GetAsync<RoomEntity>("rooms", roomId, cancellationToken);
            if(room == null)
            {
                return false;
            }
            room.LastActivityAt = lastActivityAt;
            return await SaveAsync(room, cancellationToken);
        }

        protected override string GetTableName() => TableNames.TableName;
    }
}