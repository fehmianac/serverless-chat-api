namespace Api.Endpoints.V1.Models.Room.Message;

public class MessageCreateRequestModel
{
    public string Body { get; set; } = default!;
    public string? ParentId { get; set; }

    public SendMessageAttachmentRequest? Attachment { get; set; }

    public class SendMessageAttachmentRequest
    {
        public string Type { get; set; } = default!;
        public Dictionary<string, string> AdditionalData { get; set; } = new();
        public string Payload { get; set; } = default!;
    }
}