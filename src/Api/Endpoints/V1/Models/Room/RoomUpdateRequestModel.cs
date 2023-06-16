namespace Api.Endpoints.V1.Models.Room
{
    public class RoomUpdateRequestModel
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
    }
}