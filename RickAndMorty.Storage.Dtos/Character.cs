namespace RickAndMorty.Storage.Dtos
{
    public class Character
    {
        public int Id { get; set; }
        public required DateTime DateModified { get; set; }
        public required bool IsDeleted { get; set; }
        public required string Name { get; set; }
        public string? Species { get; set; }
        public string? Type { get; set; }
        public string? Gender { get; set; }
        public int? OriginId { get; set; }
        public string? OriginName { get; set; }
        public string? OriginType { get; set; }
        public string? OriginDimension { get; set; }
        public int? LocationId { get; set; }
        public string? LocationName { get; set; }
        public string? LocationType { get; set; }
        public string? LocationDimension { get; set; }
        public string? Image { get; set; }
    }
}
