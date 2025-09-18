using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RickAndMorty.Importer.Configuration;
using RickAndMorty.Importer.Models;
using RickAndMorty.Services.Models;
using System.Text;

namespace RickAndMorty.Importer
{
    public class CharacterProcessor : ICharacterProcessor
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPlanetProcessor _planetProcessor;
        private readonly ILogger<CharacterProcessor> _logger;
        private readonly ApiConfiguration _apiConfiguration;

        public CharacterProcessor(
            IHttpClientFactory httpClientFactory,
            IPlanetProcessor planetProcessor,
            ILogger<CharacterProcessor> logger,
            ApiConfiguration apiConfiguration)
        {
            _httpClientFactory = httpClientFactory;
            _planetProcessor = planetProcessor;
            _logger = logger;
            _apiConfiguration = apiConfiguration;
        }

        public async Task<Info?> GetInfo()
        {
            using var client = _httpClientFactory.CreateClient();
            if (client.BaseAddress == null)
            {
                client.BaseAddress = _apiConfiguration.RemoteApi;
            }

            try
            {
                var response = await client.GetAsync($"character");
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject(jsonContent);
                    if (data != null)
                    {
                        return JsonConvert.DeserializeObject<Info>(JsonConvert.SerializeObject(((dynamic)data).info));
                    }
                }

                _logger.LogWarning($"Failed to get character info. Status: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting character info from {Uri}", _apiConfiguration.RemoteApi);
                throw;
            }
        }

        public async Task<List<CharacterResult>?> GetCharactersAsync(int page = 1, List<CharacterResult>? characterResults = null)
        {
            characterResults ??= new List<CharacterResult>();

            _logger.LogInformation("Getting Characters Page {Page}...", page);

            using var client = _httpClientFactory.CreateClient();
            if (client.BaseAddress == null)
            {
                client.BaseAddress = _apiConfiguration.RemoteApi;
            }

            try
            {
                var pageQuery = (page > 1) ? $"&page={page}" : "";
                var response = await client.GetAsync($"character?status=Alive{pageQuery}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();

                    var characterApiResponse = JsonConvert.DeserializeObject<CharacterApiResponse>(jsonContent);
                    if (characterApiResponse != null)
                    {
                        if (characterApiResponse.Info == null)
                        {
                            _logger.LogWarning("Character data had no info");
                            return null;
                        }
                        if (characterApiResponse.Results == null)
                        {
                            _logger.LogWarning("Character data had no results");
                            return null;
                        }

                        Info info = characterApiResponse.Info;
                        List<CharacterResult> result = characterApiResponse.Results;
                        characterResults.AddRange(result);

                        if (!string.IsNullOrWhiteSpace(info?.Next))
                        {
                            var next = info.Next;
                            var startPos = next.IndexOf("page=") + 5;
                            var endPos = next.IndexOf("&");
                            var length = endPos - startPos;
                            if (Int32.TryParse(next.Substring(startPos, length), out int nextPage))
                            {
                                await GetCharactersAsync(nextPage, characterResults);
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to get characters page {Page}. Status: {Status}", page, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting characters from {Uri}, page {Page}", _apiConfiguration.RemoteApi, page);
                throw;
            }

            return characterResults;
        }

        public async Task ProcessResultsAsync(List<CharacterResult> livingCharacters)
        {
            var charactersToSave = new List<Character>();
            foreach (var c in livingCharacters)
            {
                _logger.LogInformation("Parsing Character Data For '{Name}'", c.Name);
                var character = await ParseCharacterAsync(c);
                charactersToSave.Add(character);
            }

            if (charactersToSave.Count > 0)
            {
                _logger.LogInformation("Saving {Count} characters", charactersToSave.Count);
                var saved = await SaveCharactersAsync(charactersToSave);

                if (saved)
                {
                    _logger.LogInformation("Successfully saved character data");
                }
                else
                {
                    _logger.LogError("Failed to save character data");
                }
            }
        }

        public async Task<bool> ClearCharactersAsync()
        {
            using var client = _httpClientFactory.CreateClient();
            if (client.BaseAddress == null)
            {
                client.BaseAddress = _apiConfiguration.LocalApi;
            }

            try
            {
                _logger.LogInformation("Deleting All Character Data");
                var response = await client.DeleteAsync("Character/DeleteAllCharacters");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Cleared Character Data");
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to clear Character Data. Status: {Status}", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing characters");
                throw;
            }
        }

        private async Task<Character> ParseCharacterAsync(CharacterResult result)
        {
            var origin = await _planetProcessor.GetPlanetAsync(result.Origin);
            var location = await _planetProcessor.GetPlanetAsync(result.Location);

            return new Character()
            {
                ExternalId = result.Id,
                Name = result.Name,
                Species = result.Species,
                Type = result.Type,
                Gender = result.Gender,
                OriginId = origin?.Id,
                LocationId = location?.Id,
                Image = result.Image
            };
        }

        private async Task<bool> SaveCharactersAsync(List<Character> characters)
        {
            using var client = _httpClientFactory.CreateClient();
            if (client.BaseAddress == null)
            {
                client.BaseAddress = _apiConfiguration.LocalApi;
            }

            try
            {
                var content = JsonConvert.SerializeObject(characters);
                using var httpContent = new StringContent(content, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("Character/Bulk", httpContent);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving characters");
                throw;
            }
        }

        private class CharacterApiResponse
        {
            [JsonProperty("info")]
            public Info? Info { get; set; }

            [JsonProperty("results")]
            public List<CharacterResult>? Results { get; set; }
        }
    }
}
