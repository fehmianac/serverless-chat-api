using System.Net;
using System.Text.Json;
using Amazon.SimpleNotificationService;
using Domain.Dto;
using Domain.Dto.Room;
using Domain.Options;
using Domain.Services;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class EventBusManager : IEventBusManager
{
    private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
    private readonly IOptionsSnapshot<EventBusSettings> _eventBusSettingsOptions;

    public EventBusManager(IAmazonSimpleNotificationService amazonSimpleNotificationService, IOptionsSnapshot<EventBusSettings> eventBusSettingsOptions)
    {
        _amazonSimpleNotificationService = amazonSimpleNotificationService;
        _eventBusSettingsOptions = eventBusSettingsOptions;
    }

    public async Task<bool> RoomCreatedAsync(RoomDto room, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("RoomCreated", room), cancellationToken);
    }

    public async Task<bool> RoomModifiedAsync(RoomDto room, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("RoomModified", room), cancellationToken);
    }

    public async Task<bool> RoomAttenderAddedAsync(RoomDto room, string userId, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("RoomAttenderAdded", new {Room = room, UserId = userId}), cancellationToken);
    }

    public async Task<bool> RoomAttenderRemovedAsync(RoomDto room, string userId, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("RoomAttenderRemoved", new {RoomId = room, UserId = userId}), cancellationToken);
    }

    public async Task<bool> RoomAdminAddedAsync(RoomDto room, string userId, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("RoomAdminAdded", new {RoomId = room, UserId = userId}), cancellationToken);
    }

    public async Task<bool> RoomAdminRemovedAsync(RoomDto room, string userId, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("RoomAdminRemoved", new {Room = room, UserId = userId}), cancellationToken);
    }

    public async Task<bool> RoomMessageAddedAsync(RoomDto room, MessageDto message, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("RoomMessageAdded", new {Room = room, Message = message}), cancellationToken);
    }

    public async Task<bool> RoomMessageReactionAddedAsync(RoomDto room, string messageId, MessageDto.MessageReactionDto messageReaction, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("RoomMessageReactionAdded", new {Room = room, Reaction = messageReaction, MessageId = messageId}), cancellationToken);
    }

    public async Task<bool> RoomMessageReactionRemovedAsync(RoomDto room, string messageId, string reaction, CancellationToken cancellationToken = default)
    {
        return await PublishAsync(new EventModel<object>("RoomMessageReactionRemoved", new {Room = room, ReactioType = reaction, MessageId = messageId}), cancellationToken);
    }


    private async Task<bool> PublishAsync(EventModel<object> eventModel, CancellationToken cancellationToken = default)
    {
        if (!_eventBusSettingsOptions.Value.IsEnabled)
            return true;

        var message = JsonSerializer.Serialize(eventModel);
        var snsResponse = await _amazonSimpleNotificationService.PublishAsync(_eventBusSettingsOptions.Value.TopicArn, message, cancellationToken);
        return snsResponse.HttpStatusCode is HttpStatusCode.OK or HttpStatusCode.Accepted or HttpStatusCode.Created;
    }
}