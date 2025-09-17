using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RickAndMorty.Importer.Configuration;
using RickAndMorty.Importer.Models;
using RickAndMorty.Services.Models;
using System.Net.Http.Json;
using System.Text;

namespace RickAndMorty.Importer
{
    public class PlanetProcessor : IPlanetProcessor
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PlanetProcessor> _logger;
        private readonly List<Planet> _planets;
        private readonly ApiConfiguration _apiConfiguration;

        public PlanetProcessor(
            IHttpClientFactory httpClientFactory, 
            ILogger<PlanetProcessor> logger,
            ApiConfiguration apiConfiguration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _planets = new List<Planet>();
            _apiConfiguration = apiConfiguration;
        }

        public async Task<Planet?> GetPlanetAsync(PlanetResult planetResult)
        {
            // Check if we have pulled this planet from the API before
            var existingPlanet = _planets.FirstOrDefault(p =>
                string.Equals(p.Name?.Trim(), planetResult.Name?.Trim(), StringComparison.OrdinalIgnoreCase));

            if (existingPlanet != null)
            {
                return existingPlanet;
            }

            if (string.IsNullOrWhiteSpace(planetResult.Url))
            {
                return null;
            }

            _logger.LogInformation("Getting Planet Data For '{Name}'", planetResult.Name);

            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(planetResult.Url);

            try
            {
                var response = await client.GetAsync(planetResult.Url);
                if (response.IsSuccessStatusCode)
                {
                    var location = await response.Content.ReadFromJsonAsync<PlanetResult>();
                    if (location == null)
                    {
                        _logger.LogWarning("Planet data for '{Name}' is null", planetResult.Name);
                        return null;
                    }

                    var planet = ParsePlanet(location);

                    planet = await SavePlanetAsync(planet);
                    if (planet != null)
                    {
                        _planets.Add(planet);
                    }
                    return planet;
                }
                else
                {
                    _logger.LogWarning("Failed to get planet data for '{Name}'. Status: {Status}",
                        planetResult.Name, response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting planet data for '{Name}'", planetResult.Name);
                throw;
            }
        }

        public async Task<bool> ClearPlanetsAsync()
        {
            using var client = _httpClientFactory.CreateClient();
            if (client.BaseAddress == null)
            {
                client.BaseAddress = _apiConfiguration.LocalApi;
            }

            try
            {
                _logger.LogInformation("Deleting All Planets Data");
                var response = await client.DeleteAsync("Planet/DeleteAllPlanets");

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Cleared Planets Data");
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to clear Planets Data. Status: {Status}", response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing planets");
                throw;
            }
        }

        private static Planet ParsePlanet(PlanetResult info)
        {
            return new Planet()
            {
                Id = info.Id,
                Name = info.Name,
                Type = info.Type,
                Dimension = info.Dimension
            };
        }

        private async Task<Planet?> SavePlanetAsync(Planet planet)
        {
            using var client = _httpClientFactory.CreateClient();
            if (client.BaseAddress == null)
            {
                client.BaseAddress = _apiConfiguration.LocalApi;
            }

            try
            {
                using var httpContent = new StringContent(
                    JsonConvert.SerializeObject(planet),
                    Encoding.UTF8,
                    "application/json");

                _logger.LogInformation("Saving Planet Data For '{Name}'", planet.Name);
                var response = await client.PostAsync("Planet", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Saved Planet Data For '{Name}'", planet.Name);
                    return await response.Content.ReadFromJsonAsync<Planet>();
                }
                else
                {
                    _logger.LogWarning("Failed to save Planet Data For '{Name}'. Status: {Status}",
                        planet.Name, response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving planet '{Name}'", planet.Name);
                throw;
            }
        }
    }
}
