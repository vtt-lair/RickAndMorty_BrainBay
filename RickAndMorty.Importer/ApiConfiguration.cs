namespace RickAndMorty.Importer.Configuration
{
    public class ApiConfiguration
    {
        public required Uri RemoteApi { get; set; }
        public required Uri LocalApi { get; set; }
    }
}