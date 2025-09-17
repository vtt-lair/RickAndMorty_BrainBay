using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using RickAndMorty.Importer.Configuration;
using System.Net;

namespace RickAndMorty.Importer.Tests.Helpers
{
    public static class TestHelper
    {
        public static ApiConfiguration CreateApiConfiguration()
        {
            return new ApiConfiguration
            {
                RemoteApi = new Uri("https://rickandmortyapi.com/api/"),
                LocalApi = new Uri("https://localhost:5001/api/")
            };
        }

        public static ILogger<T> CreateFakeLogger<T>()
        {
            return NullLogger<T>.Instance;
        }

        public static MockHttpMessageHandler CreateMockHttpHandler()
        {
            return new MockHttpMessageHandler();
        }

        public static IHttpClientFactory CreateMockHttpClientFactory(MockHttpMessageHandler mockHandler)
        {
            var httpClient = mockHandler.ToHttpClient();
            var mockFactory = new Mock<IHttpClientFactory>();
            mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);
            return mockFactory.Object;
        }

        public static void SetupSuccessfulApiResponse(MockHttpMessageHandler mockHandler, string endpoint, string responseContent)
        {
            mockHandler.When(HttpMethod.Get, $"*{endpoint}*")
                      .Respond("application/json", responseContent);
        }

        public static void SetupApiResponse(MockHttpMessageHandler mockHandler, HttpMethod method, string endpoint, HttpStatusCode statusCode, string? responseContent = null)
        {
            var response = mockHandler.When(method, $"*{endpoint}*")
                                    .Respond(statusCode);
            
            if (!string.IsNullOrEmpty(responseContent))
            {
                response.Respond("application/json", responseContent);
            }
        }
    }
}