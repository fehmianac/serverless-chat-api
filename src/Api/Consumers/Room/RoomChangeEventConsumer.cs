using Domain.Dto.Notifier;
using Domain.Entities;
using Domain.Entities.Base;
using Domain.Events.Contracts;
using Domain.Events.Room;
using Domain.Repositories;
using Domain.Services;

namespace Api.Consumers.Room;

public class RoomChangeEventConsumer : IConsumer<RoomChangedEvent>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRoomRepository _userRoomRepository;
    private readonly IRoomLastActivityRepository _roomLastActivityRepository;
    private readonly IRoomNotificationRepository _roomNotificationRepository;
    private readonly IPubSubServices _pubSubServices;

    public RoomChangeEventConsumer(IRoomRepository roomRepository, IUserRoomRepository userRoomRepository,
        IRoomLastActivityRepository roomLastActivityRepository, IRoomNotificationRepository roomNotificationRepository,
        IPubSubServices pubSubServices)
    {
        _roomRepository = roomRepository;
        _userRoomRepository = userRoomRepository;
        _roomLastActivityRepository = roomLastActivityRepository;
        _roomNotificationRepository = roomNotificationRepository;
        _pubSubServices = pubSubServices;
    }

    public async Task Handler(RoomChangedEvent payload, CancellationToken cancellationToken = default)
    {
        var room = await _roomRepository.GetRoomAsync(payload.RoomId, cancellationToken);
        if (room == null)
        {
            return;
        }

        var roomLastActivity =
            await _roomLastActivityRepository.GetRoomLastActivityAsync(payload.RoomId, cancellationToken);

        var oldestUserRoomActivities = new List<IEntity>();
        var newestUserRoomActivities = new List<IEntity>();
        if (roomLastActivity != null)
        {
            var lastActivity = roomLastActivity.LastActivityAt;
            oldestUserRoomActivities.AddRange(room.Attenders.Select(q => new UserRoomEntity
            {
                UserId = q,
                RoomId = payload.RoomId,
                LastActivityAt = lastActivity
            }));
        }

        newestUserRoomActivities.AddRange(room.Attenders.Select(q => new UserRoomEntity
        {
            UserId = q,
            RoomId = payload.RoomId,
            LastActivityAt = payload.ActivityAt
        }));

        await _roomLastActivityRepository.SaveRoomLastActivityAsync(payload.RoomId, payload.ActivityAt,
            cancellationToken);
        await _userRoomRepository.BatchWriteAsync(newestUserRoomActivities, oldestUserRoomActivities,
            cancellationToken);

        var notificationEntities =
            await _roomNotificationRepository.GetBatchRoomNotificationAsync(room.Attenders, payload.RoomId,
                cancellationToken);
        if (!notificationEntities.Any())
        {
            await _roomNotificationRepository.SaveBatchRoomNotificationAsync(room.Attenders.Select(q =>
                new RoomNotificationEntity
                {
                    HasNotification = true,
                    MessageCount = 1,
                    RoomId = room.Id,
                    UserId = q,
                    MessageIds = payload is { MessageId: not null, HasNewMessage: true }
                        ? new List<string>
                        {
                            payload.MessageId
                        }
                        : new List<string>()
                }).ToList(), cancellationToken);
            return;
        }

        foreach (var notificationEntity in notificationEntities)
        {
            if (payload.HasNewMessage)
            {
                notificationEntity.MessageCount += 1;
                if (!string.IsNullOrWhiteSpace(payload.MessageId))
                {
                    notificationEntity.MessageIds.Add(payload.MessageId);
                }
            }

            notificationEntity.HasNotification = true;
        }

        await _roomNotificationRepository.SaveBatchRoomNotificationAsync(notificationEntities, cancellationToken);
        await _pubSubServices.NotifyUser(room.Attenders, new RoomChangedNotifyModel
        {
            RoomId = room.Id
        }, cancellationToken);
    }
}