namespace RickAndMorty.Services.Models
{
    public class Character
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Species { get; set; }

        public string? Type { get; set; }

        public string? Gender { get; set; }

        public int? OriginId { get; set; }

        public Planet? Origin { get; set; }

        public int? LocationId { get; set; }

        public Planet? Location { get; set; }

        public string? Image { get; set; }
    }
}
