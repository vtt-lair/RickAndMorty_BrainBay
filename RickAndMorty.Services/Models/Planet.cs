namespace RickAndMorty.Services.Models
{
    public class Planet
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Type { get; set; }

        public string? Dimension { get; set; }
    }
}
