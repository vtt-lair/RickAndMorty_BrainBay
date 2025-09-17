using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RichardSzalay.MockHttp;
using RickAndMorty.Importer.Tests.Helpers;
using RickAndMorty.Importer.Tests.TestData;
using System.Net;

namespace RickAndMorty.Importer.Tests.IntegrationTests
{
    public class ImporterIntegrationTests
    {
        [Fact]
        public async Task FullImportProcess_ShouldProcessExpectedNumberOfCharacters()
        {
            // Arrange
            const int expectedCharacterCount = 60;
            const int charactersPerPage = 20;
            const int totalPages = 3;

            var mockHttpHandler = TestHelper.CreateMockHttpHandler();

            var services = new ServiceCollection();
            services.AddSingleton<IHttpClientFactory>(_ => TestHelper.CreateMockHttpClientFactory(mockHttpHandler));
            services.AddLogging(builder => builder.AddConsole());
            services.AddSingleton(TestHelper.CreateApiConfiguration());
            
            // Register interfaces and implementations correctly
            services.AddTransient<IPlanetProcessor, PlanetProcessor>();
            services.AddTransient<ICharacterProcessor, CharacterProcessor>();

            var serviceProvider = services.BuildServiceProvider();

            SetupMultiPageCharacterResponse(mockHttpHandler, charactersPerPage, totalPages);
            SetupPlanetResponses(mockHttpHandler);
            SetupLocalApiResponses(mockHttpHandler);

            // Act
            using var scope = serviceProvider.CreateScope();
            var characterProcessor = scope.ServiceProvider.GetRequiredService<ICharacterProcessor>();

            var characters = await characterProcessor.GetCharactersAsync();

            // Assert
            characters.Should().NotBeNull();
            characters.Should().HaveCount(expectedCharacterCount);
            characters!.All(c => c.Status.ToLower() == "alive").Should().BeTrue();
            characters!.Select(c => c.Id).Should().OnlyHaveUniqueItems();
        }

        private static void SetupMultiPageCharacterResponse(MockHttpMessageHandler mockHttpHandler, int charactersPerPage, int totalPages)
        {
            var totalCharacters = charactersPerPage * totalPages;

            for (int page = totalPages; page >= 1; page--)
            {
                var pageCharacters = SampleData.CreateCharacterResults(charactersPerPage);

                // Adjust IDs to ensure uniqueness across pages
                for (int i = 0; i < pageCharacters.Count; i++)
                {
                    pageCharacters[i].Id = ((page - 1) * charactersPerPage) + i + 1;
                    pageCharacters[i].Name = $"Character {pageCharacters[i].Id}";
                }

                var nextPageUrl = page < totalPages ? $"https://rickandmortyapi.com/api/character?page={page + 1}&status=Alive" : null;
                var prevPageUrl = page > 1 ? $"https://rickandmortyapi.com/api/character?page={page - 1}&status=Alive" : null;

                var pageInfo = SampleData.CreateInfo(totalCharacters, totalPages, nextPageUrl, prevPageUrl);
                var pageResponse = SampleData.CreateCharacterApiResponse(pageCharacters, pageInfo);

                if (page == 1)
                {
                    mockHttpHandler.When(HttpMethod.Get, "*character?status=Alive")
                                  .Respond("application/json", pageResponse);
                }
                else
                {
                    mockHttpHandler.When(HttpMethod.Get, $"*character?status=Alive&page={page}")
                                  .Respond("application/json", pageResponse);
                }
            }
        }

        private static void SetupPlanetResponses(MockHttpMessageHandler mockHttpHandler)
        {
            mockHttpHandler.When(HttpMethod.Get, "*location*")
                          .Respond("application/json", """
                          {
                              "id": 1,
                              "name": "Earth (C-137)",
                              "type": "Planet",
                              "dimension": "Dimension C-137",
                              "residents": [],
                              "url": "https://rickandmortyapi.com/api/location/1",
                              "created": "2017-11-04T18:48:46.250Z"
                          }
                          """);

            mockHttpHandler.When(HttpMethod.Post, "*Planet")
                          .Respond("application/json", """
                          {
                              "id": 1,
                              "name": "Earth (C-137)",
                              "type": "Planet",
                              "dimension": "Dimension C-137"
                          }
                          """);
        }

        private static void SetupLocalApiResponses(MockHttpMessageHandler mockHttpHandler)
        {
            mockHttpHandler.When(HttpMethod.Delete, "*Character/DeleteAllCharacters")
                          .Respond(HttpStatusCode.OK);

            mockHttpHandler.When(HttpMethod.Delete, "*Planet/DeleteAllPlanets")
                          .Respond(HttpStatusCode.OK);

            mockHttpHandler.When(HttpMethod.Post, "*Character/Bulk")
                          .Respond(HttpStatusCode.OK);
        }
    }
}