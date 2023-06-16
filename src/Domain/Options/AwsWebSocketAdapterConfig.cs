namespace Domain.Options;

public class AwsWebSocketAdapterConfig
{
    public bool Enabled { get; set; }
    public string QueueUrl { get; set; } = default!;
}