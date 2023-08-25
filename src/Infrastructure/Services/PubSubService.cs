using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Domain.Dto.Notifier.Base;
using Domain.Options;
using Domain.Services;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class PubSubService : IPubSubServices
{
    private readonly IAmazonSQS _amazonSqs;
    private readonly IOptionsSnapshot<AwsWebSocketAdapterConfig> _awsWebSocketAdapterConfigOptions;

    public PubSubService(IAmazonSQS amazonSqs, IOptionsSnapshot<AwsWebSocketAdapterConfig> awsWebSocketAdapterConfigOptions)
    {
        _amazonSqs = amazonSqs;
        _awsWebSocketAdapterConfigOptions = awsWebSocketAdapterConfigOptions;
    }

    public async Task NotifyUser(List<string> userIds, object payload, CancellationToken cancellationToken = default)
    {
        var awsWebSocketAdapterConfig = _awsWebSocketAdapterConfigOptions.Value;
        if (!awsWebSocketAdapterConfig.Enabled)
        {
            return;
        }

        var body = JsonSerializer.Serialize(payload);
        var queueUrl = awsWebSocketAdapterConfig.QueueUrl;
        var entries = userIds.Select(q => new SendMessageBatchRequestEntry
        {
            Id = Guid.NewGuid().ToString("N"),
            MessageBody = JsonSerializer.Serialize(new WebSocketNotifierModel
            {
                UserId = q,
                Body = body
            }),
        }).ToList();

        var request = new SendMessageBatchRequest
        {
            QueueUrl = queueUrl,
            Entries = entries,
        };
        await _amazonSqs.SendMessageBatchAsync(request, cancellationToken);
    }
}