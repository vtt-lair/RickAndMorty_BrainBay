namespace RickAndMorty.Storage.Dtos
{
    public class Planet
    {
        public int Id { get; set; }

        public required DateTime DateModified { get; set; }

        public required bool IsDeleted { get; set; }

        public required string Name { get; set; }

        public string? Type { get; set; }

        public string? Dimension { get; set; }
    }
}
