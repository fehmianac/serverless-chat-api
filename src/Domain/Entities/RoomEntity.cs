namespace Domain.Entities;

public class RoomEntity
{
    public string Pk => $"rooms";
    public string Sk => Id;
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
    public List<string> Attenders { get; set; } = new();
    public List<string> Admins { get; set; } = new();

    public List<LastMessageInfoDataModel> LastMessageInfo { get; set; } = new();
    public List<TypingAttenderDataModel> TypingAttenders { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }

    public class LastMessageInfoDataModel
    {
        public string Pk { get; set; } = default!;
        public string Sk { get; set; } = default!;
    }

    public class TypingAttenderDataModel
    {
        public string UserId { get; set; } = default!;
        public DateTime TypingAt { get; set; }
    }
}