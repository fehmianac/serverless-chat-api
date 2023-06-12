namespace Domain.Entities;

public class BlockedUserEntity
{
    public string Pk => $"blockedUser#{FromUserId}";

    public string Sk => ToUserId;

    public string FromUserId { get; set; } = default!;
    public string ToUserId { get; set; } = default!;

    public DateTime CreatedUtc { get; set; }
}