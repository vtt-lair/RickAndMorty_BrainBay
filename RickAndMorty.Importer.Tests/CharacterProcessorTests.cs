using FluentAssertions;
using Microsoft.Extensions.Logging;
using RichardSzalay.MockHttp;
using RickAndMorty.Importer.Configuration;
using RickAndMorty.Importer.Models;
using RickAndMorty.Importer.Tests.Helpers;
using RickAndMorty.Importer.Tests.TestData;
using System.Net;
using Xunit;

namespace RickAndMorty.Importer.Tests
{
    public class CharacterProcessorTests
    {
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CharacterProcessor> _logger;
        private readonly ApiConfiguration _apiConfiguration;
        private readonly Mock<IPlanetProcessor> _mockPlanetProcessor;

        public CharacterProcessorTests()
        {
            _mockHttpHandler = TestHelper.CreateMockHttpHandler();
            _httpClientFactory = TestHelper.CreateMockHttpClientFactory(_mockHttpHandler);
            _logger = TestHelper.CreateFakeLogger<CharacterProcessor>();
            _apiConfiguration = TestHelper.CreateApiConfiguration();
            
            _mockPlanetProcessor = new Mock<IPlanetProcessor>();
        }

        private CharacterProcessor CreateCharacterProcessor()
        {
            return new CharacterProcessor(
                _httpClientFactory,
                _mockPlanetProcessor.Object,
                _logger,
                _apiConfiguration);
        }

        [Fact]
        public async Task GetCharactersAsync_ShouldReturnExpectedNumberOfCharacters_WhenApiReturnsValidResponse()
        {
            // Arrange
            const int expectedCharacterCount = 20;
            var characters = SampleData.CreateCharacterResults(expectedCharacterCount);
            var info = SampleData.CreateInfo(expectedCharacterCount, 1, null, null);
            var responseJson = SampleData.CreateCharacterApiResponse(characters, info);
            
            TestHelper.SetupSuccessfulApiResponse(_mockHttpHandler, "character", responseJson);
            
            var processor = CreateCharacterProcessor();

            // Act
            var result = await processor.GetCharactersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(expectedCharacterCount);
            result!.All(c => c.Status.ToLower() == "alive").Should().BeTrue();
        }

        [Fact]
        public async Task GetCharactersAsync_ShouldReturnCorrectNumberOfCharacters_WhenMultiplePagesExist()
        {
            // Arrange
            const int charactersPerPage = 10;
            const int totalPages = 3;
            const int expectedTotalCharacters = charactersPerPage * totalPages;

            var page1Characters = SampleData.CreateCharacterResults(charactersPerPage);
            var page1Info = SampleData.CreateInfo(expectedTotalCharacters, totalPages, "https://rickandmortyapi.com/api/character?page=2&status=Alive", null);
            var page1Response = SampleData.CreateCharacterApiResponse(page1Characters, page1Info);

            var page2Characters = SampleData.CreateCharacterResults(charactersPerPage);
            for (int i = 0; i < page2Characters.Count; i++)
            {
                page2Characters[i].Id = i + 11;
                page2Characters[i].Name = $"Character {i + 11}";
            }
            var page2Info = SampleData.CreateInfo(expectedTotalCharacters, totalPages, "https://rickandmortyapi.com/api/character?page=3&status=Alive", "https://rickandmortyapi.com/api/character?page=1&status=Alive");
            var page2Response = SampleData.CreateCharacterApiResponse(page2Characters, page2Info);

            var page3Characters = SampleData.CreateCharacterResults(charactersPerPage);
            for (int i = 0; i < page3Characters.Count; i++)
            {
                page3Characters[i].Id = i + 21;
                page3Characters[i].Name = $"Character {i + 21}";
            }
            var page3Info = SampleData.CreateInfo(expectedTotalCharacters, totalPages, null, "https://rickandmortyapi.com/api/character?page=2&status=Alive");
            var page3Response = SampleData.CreateCharacterApiResponse(page3Characters, page3Info);

            _mockHttpHandler.When(HttpMethod.Get, "*character?status=Alive&page=3")
                          .Respond("application/json", page3Response);
            _mockHttpHandler.When(HttpMethod.Get, "*character?status=Alive&page=2")
                          .Respond("application/json", page2Response);
            _mockHttpHandler.When(HttpMethod.Get, "*character?status=Alive")
                          .Respond("application/json", page1Response);

            var processor = CreateCharacterProcessor();

            // Act
            var result = await processor.GetCharactersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(expectedTotalCharacters);
            result!.Select(c => c.Id).Should().BeInAscendingOrder();
            result.All(c => c.Status.ToLower() == "alive").Should().BeTrue();
        }

        [Fact]
        public async Task GetCharactersAsync_ShouldReturnEmptyList_WhenApiReturnsNoCharacters()
        {
            // Arrange
            var emptyCharacters = new List<CharacterResult>();
            var info = SampleData.CreateInfo(0, 0, null, null);
            var responseJson = SampleData.CreateCharacterApiResponse(emptyCharacters, info);
            
            TestHelper.SetupSuccessfulApiResponse(_mockHttpHandler, "character", responseJson);
            
            var processor = CreateCharacterProcessor();

            // Act
            var result = await processor.GetCharactersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCharactersAsync_ShouldReturnNull_WhenApiResponseHasNullInfo()
        {
            // Arrange
            var responseJson = """
            {
                "info": null,
                "results": []
            }
            """;
            
            TestHelper.SetupSuccessfulApiResponse(_mockHttpHandler, "character", responseJson);
            
            var processor = CreateCharacterProcessor();

            // Act
            var result = await processor.GetCharactersAsync();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCharactersAsync_ShouldReturnNull_WhenApiResponseHasNullResults()
        {
            // Arrange
            var responseJson = """
            {
                "info": {
                    "count": 0,
                    "pages": 0,
                    "next": null,
                    "prev": null
                },
                "results": null
            }
            """;
            
            TestHelper.SetupSuccessfulApiResponse(_mockHttpHandler, "character", responseJson);
            
            var processor = CreateCharacterProcessor();

            // Act
            var result = await processor.GetCharactersAsync();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCharactersAsync_ShouldHandleApiFailure_WhenStatusCodeIsNotSuccess()
        {
            // Arrange
            TestHelper.SetupApiResponse(_mockHttpHandler, HttpMethod.Get, "character", HttpStatusCode.InternalServerError);
            
            var processor = CreateCharacterProcessor();

            // Act
            var result = await processor.GetCharactersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty(); // Should return empty list for first page
        }

        [Fact]
        public async Task ProcessResultsAsync_ShouldProcessExpectedNumberOfCharacters()
        {
            // Arrange
            const int expectedCharacterCount = 15;
            var characters = SampleData.CreateCharacterResults(expectedCharacterCount);
            
            var planet = SampleData.CreatePlanet();
            _mockPlanetProcessor.Setup(p => p.GetPlanetAsync(It.IsAny<PlanetResult>()))
                              .ReturnsAsync(planet);

            TestHelper.SetupApiResponse(_mockHttpHandler, HttpMethod.Post, "Character/Bulk", HttpStatusCode.OK);
            
            var processor = CreateCharacterProcessor();

            // Act
            await processor.ProcessResultsAsync(characters);

            // Assert
            // Verify planet processor was called for each character's origin and location
            _mockPlanetProcessor.Verify(p => p.GetPlanetAsync(It.IsAny<PlanetResult>()), 
                Times.Exactly(expectedCharacterCount * 2)); // origin + location for each character
        }

        [Fact]
        public async Task ClearCharactersAsync_ShouldReturnTrue_WhenApiSuccessfullyDeletesCharacters()
        {
            // Arrange
            TestHelper.SetupApiResponse(_mockHttpHandler, HttpMethod.Delete, "Character/DeleteAllCharacters", HttpStatusCode.OK);
            
            var processor = CreateCharacterProcessor();

            // Act
            var result = await processor.ClearCharactersAsync();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ClearCharactersAsync_ShouldReturnFalse_WhenApiFailsToDeleteCharacters()
        {
            // Arrange
            TestHelper.SetupApiResponse(_mockHttpHandler, HttpMethod.Delete, "Character/DeleteAllCharacters", HttpStatusCode.InternalServerError);
            
            var processor = CreateCharacterProcessor();

            // Act
            var result = await processor.ClearCharactersAsync();

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(20)]
        [InlineData(100)]
        public async Task GetCharactersAsync_ShouldReturnExactNumberOfCharacters_ForVariousInputSizes(int expectedCount)
        {
            // Arrange
            var characters = SampleData.CreateCharacterResults(expectedCount);
            var info = SampleData.CreateInfo(expectedCount, 1, null, null);
            var responseJson = SampleData.CreateCharacterApiResponse(characters, info);
            
            TestHelper.SetupSuccessfulApiResponse(_mockHttpHandler, "character", responseJson);
            
            var processor = CreateCharacterProcessor();

            // Act
            var result = await processor.GetCharactersAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(expectedCount);
            result!.Should().AllSatisfy(character =>
            {
                character.Should().NotBeNull();
                character.Name.Should().NotBeNullOrEmpty();
                character.Status.Should().Be("Alive");
            });
        }
    }
}