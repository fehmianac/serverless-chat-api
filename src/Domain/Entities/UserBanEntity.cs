using System.Text.Json.Serialization;
using Domain.Entities.Base;

namespace Domain.Entities
{
    public class UserBanEntity : IEntity
    {
        [JsonPropertyName("pk")]
        public string Pk => $"userBan#{FromUserId}";
        
        [JsonPropertyName("sk")]
        public string Sk => ToUserId;

        [JsonPropertyName("fromUserId")]
        public string FromUserId { get; set; } = default!;
        
        [JsonPropertyName("toUserId")]
        public string ToUserId { get; set; } = default!;
        
        [JsonPropertyName("createdUtc")]
        public DateTime CreatedUtc { get; set; }
    }
}