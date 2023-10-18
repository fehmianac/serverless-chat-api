namespace Domain.Enum
{
    
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public enum MessageStatus
    {
        Pending = 0,
        Delivered = 1,
        Read = 2,
        Deleted = 3
    }
}