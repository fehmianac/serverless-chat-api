namespace Domain.Dto.Notifier.Base;

public class WebSocketNotifierModel
{
    public string UserId { get; set; } = default!;
    public string Body { get; set; } = default!;
}