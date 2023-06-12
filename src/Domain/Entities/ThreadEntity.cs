using Domain.Extensions;

namespace Domain.Entities;

public class ThreadEntity
{
    public string Pk => $"thread#{RoomId}";
    public long Sk => LastActivityAt.ToUnixTimeMilliseconds();
    
    public string Id { get; set; } = default!;
    public string RoomId { get; set; } = default!;

    public Dictionary<string, string> AdditionalInfo { get; set; } = new();
    public List<LastMessageInfoDataModel>? LastMessageInfo { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }

    public class LastMessageInfoDataModel
    {
        public string Pk { get; set; } = default!;
        public string Sk { get; set; } = default!;
    }
}