using RickAndMorty.Importer.Models;
using RickAndMorty.Services.Models;

namespace RickAndMorty.Importer
{
    public interface ICharacterProcessor
    {
        Task<Info?> GetInfo();
        Task<List<CharacterResult>?> GetCharactersAsync(int page = 1, List<CharacterResult>? characterResults = null);
        Task ProcessResultsAsync(List<CharacterResult> livingCharacters);
        Task<bool> ClearCharactersAsync();
    }
}