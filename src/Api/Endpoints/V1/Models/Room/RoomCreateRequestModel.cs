using Domain.Dto.Room;
using FluentValidation;

namespace Api.Endpoints.V1.Models.Room
{
    public class RoomCreateRequestModel
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public bool IsGroup { get; set; } = false;
        public List<string> Attenders { get; set; } = new();
    }

    public class RoomRequestModelValidator : AbstractValidator<RoomCreateRequestModel>
    {
        public RoomRequestModelValidator()
        {
            RuleFor(q => q.Attenders).NotEmpty();
            RuleFor(q => q.Name).NotEmpty();
        }
    }

    public static class RoomRequestModelMapper
    {
        public static RoomDto ToDto(this RoomCreateRequestModel input)
        {
            return new RoomDto
            {
                Admins = new List<string>(),
                Attenders = input.Attenders,
                Description = input.Description,
                ImageUrl = input.ImageUrl,
                Name = input.Name,
                CreatedAt = DateTime.Now,
                LastActivityAt = DateTime.UtcNow
            };
        }
    }
}