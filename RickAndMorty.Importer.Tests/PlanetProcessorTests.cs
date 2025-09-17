using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using RickAndMorty.Importer.Configuration;
using RickAndMorty.Importer.Tests.Helpers;
using RickAndMorty.Importer.Tests.TestData;
using System.Net;
using Xunit;

namespace RickAndMorty.Importer.Tests
{
    public class PlanetProcessorTests
    {
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PlanetProcessor> _logger;
        private readonly ApiConfiguration _apiConfiguration;

        public PlanetProcessorTests()
        {
            _mockHttpHandler = TestHelper.CreateMockHttpHandler();
            _httpClientFactory = TestHelper.CreateMockHttpClientFactory(_mockHttpHandler);
            _logger = TestHelper.CreateFakeLogger<PlanetProcessor>();
            _apiConfiguration = TestHelper.CreateApiConfiguration();
        }

        private PlanetProcessor CreatePlanetProcessor()
        {
            return new PlanetProcessor(_httpClientFactory, _logger, _apiConfiguration);
        }

        [Fact]
        public async Task GetPlanetAsync_ShouldReturnPlanet_WhenValidPlanetResultProvided()
        {
            // Arrange
            var planetResult = SampleData.CreatePlanetResult(1, "Earth (C-137)");
            var savedPlanet = SampleData.CreatePlanet(1, "Earth (C-137)");
            
            _mockHttpHandler.When(HttpMethod.Get, planetResult.Url)
                          .Respond("application/json", JsonConvert.SerializeObject(planetResult));
            
            _mockHttpHandler.When(HttpMethod.Post, "*Planet")
                          .Respond("application/json", JsonConvert.SerializeObject(savedPlanet));
            
            var processor = CreatePlanetProcessor();

            // Act
            var result = await processor.GetPlanetAsync(planetResult);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(savedPlanet.Id);
            result.Name.Should().Be(savedPlanet.Name);
            result.Type.Should().Be(savedPlanet.Type);
            result.Dimension.Should().Be(savedPlanet.Dimension);
        }

        [Fact]
        public async Task GetPlanetAsync_ShouldReturnCachedPlanet_WhenPlanetAlreadyRetrieved()
        {
            // Arrange
            var planetResult1 = SampleData.CreatePlanetResult(1, "Earth (C-137)");
            var planetResult2 = SampleData.CreatePlanetResult(1, "Earth (C-137)"); // Same planet
            var savedPlanet = SampleData.CreatePlanet(1, "Earth (C-137)");
            
            _mockHttpHandler.When(HttpMethod.Get, planetResult1.Url)
                          .Respond("application/json", JsonConvert.SerializeObject(planetResult1));
            
            _mockHttpHandler.When(HttpMethod.Post, "*Planet")
                          .Respond("application/json", JsonConvert.SerializeObject(savedPlanet));
            
            var processor = CreatePlanetProcessor();

            // Act
            var result1 = await processor.GetPlanetAsync(planetResult1);
            var result2 = await processor.GetPlanetAsync(planetResult2); // Should return cached version

            // Assert
            result1.Should().NotBeNull();
            result2.Should().NotBeNull();
            result1!.Id.Should().Be(result2!.Id);
            result1.Name.Should().Be(result2.Name);            
        }

        [Fact]
        public async Task GetPlanetAsync_ShouldReturnNull_WhenPlanetResultHasEmptyUrl()
        {
            // Arrange
            var planetResult = SampleData.CreatePlanetResult();
            planetResult.Url = ""; // Empty URL
            
            var processor = CreatePlanetProcessor();

            // Act
            var result = await processor.GetPlanetAsync(planetResult);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetPlanetAsync_ShouldReturnNull_WhenPlanetResultHasNullUrl()
        {
            // Arrange
            var planetResult = SampleData.CreatePlanetResult();
            planetResult.Url = null!; // Null URL
            
            var processor = CreatePlanetProcessor();

            // Act
            var result = await processor.GetPlanetAsync(planetResult);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetPlanetAsync_ShouldReturnNull_WhenApiReturnsNullPlanetData()
        {
            // Arrange
            var planetResult = SampleData.CreatePlanetResult();
            
            _mockHttpHandler.When(HttpMethod.Get, planetResult.Url)
                          .Respond("application/json", "null");
            
            var processor = CreatePlanetProcessor();

            // Act
            var result = await processor.GetPlanetAsync(planetResult);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetPlanetAsync_ShouldReturnNull_WhenApiReturnsFailureStatusCode()
        {
            // Arrange
            var planetResult = SampleData.CreatePlanetResult();
            
            _mockHttpHandler.When(HttpMethod.Get, planetResult.Url)
                          .Respond(HttpStatusCode.NotFound);
            
            var processor = CreatePlanetProcessor();

            // Act
            var result = await processor.GetPlanetAsync(planetResult);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetPlanetAsync_ShouldReturnNull_WhenSavePlanetFails()
        {
            // Arrange
            var planetResult = SampleData.CreatePlanetResult();
            
            _mockHttpHandler.When(HttpMethod.Get, planetResult.Url)
                          .Respond("application/json", JsonConvert.SerializeObject(planetResult));
            
            _mockHttpHandler.When(HttpMethod.Post, "*Planet")
                          .Respond(HttpStatusCode.InternalServerError);
            
            var processor = CreatePlanetProcessor();

            // Act
            var result = await processor.GetPlanetAsync(planetResult);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ClearPlanetsAsync_ShouldReturnTrue_WhenApiSuccessfullyDeletesPlanets()
        {
            // Arrange
            TestHelper.SetupApiResponse(_mockHttpHandler, HttpMethod.Delete, "Planet/DeleteAllPlanets", HttpStatusCode.OK);
            
            var processor = CreatePlanetProcessor();

            // Act
            var result = await processor.ClearPlanetsAsync();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ClearPlanetsAsync_ShouldReturnFalse_WhenApiFailsToDeletePlanets()
        {
            // Arrange
            TestHelper.SetupApiResponse(_mockHttpHandler, HttpMethod.Delete, "Planet/DeleteAllPlanets", HttpStatusCode.InternalServerError);
            
            var processor = CreatePlanetProcessor();

            // Act
            var result = await processor.ClearPlanetsAsync();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetPlanetAsync_ShouldThrowException_WhenHttpRequestFails()
        {
            // Arrange
            var planetResult = SampleData.CreatePlanetResult();
            
            _mockHttpHandler.When(HttpMethod.Get, planetResult.Url)
                          .Throw(new HttpRequestException("Network error"));
            
            var processor = CreatePlanetProcessor();

            // Act & Assert
            await processor.Invoking(p => p.GetPlanetAsync(planetResult))
                          .Should().ThrowAsync<HttpRequestException>()
                          .WithMessage("Network error");
        }

        [Theory]
        [InlineData("Earth (C-137)", "earth (c-137)", true)] 
        [InlineData("Earth (C-137)", " Earth (C-137) ", true)]
        public async Task GetPlanetAsync_ShouldHandlePlanetNameComparison_Correctly(string firstName, string secondName, bool shouldMatch)
        {
            // Arrange
            var planetResult1 = SampleData.CreatePlanetResult(1, firstName);
            var planetResult2 = SampleData.CreatePlanetResult(2, secondName);
            var savedPlanet = SampleData.CreatePlanet(1, firstName);
            
            _mockHttpHandler.When(HttpMethod.Get, planetResult1.Url)
                          .Respond("application/json", JsonConvert.SerializeObject(planetResult1));
            
            _mockHttpHandler.When(HttpMethod.Post, "*Planet")
                          .Respond("application/json", JsonConvert.SerializeObject(savedPlanet));
            
            if (!shouldMatch)
            {
                var savedPlanet2 = SampleData.CreatePlanet(2, secondName);
                _mockHttpHandler.When(HttpMethod.Get, planetResult2.Url)
                              .Respond("application/json", JsonConvert.SerializeObject(planetResult2));
                
                _mockHttpHandler.When(HttpMethod.Post, "*Planet")
                              .Respond("application/json", JsonConvert.SerializeObject(savedPlanet2));
            }
            
            var processor = CreatePlanetProcessor();

            // Act
            var result1 = await processor.GetPlanetAsync(planetResult1);
            var result2 = await processor.GetPlanetAsync(planetResult2);

            // Assert
            result1.Should().NotBeNull();
            result2.Should().NotBeNull();
            
            if (shouldMatch)
            {
                result1!.Id.Should().Be(result2!.Id);
                result1.Name.Should().Be(result2.Name);
            }
            else
            {
                result1!.Id.Should().NotBe(result2!.Id);
                result1.Name.Should().NotBe(result2.Name);
            }
        }
    }
}