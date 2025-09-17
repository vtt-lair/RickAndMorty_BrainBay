using RickAndMorty.Importer.Models;
using RickAndMorty.Services.Models;
using System.Linq;

namespace RickAndMorty.Importer.Tests.TestData
{
    public static class SampleData
    {
        public static CharacterResult CreateCharacterResult(int id = 1, string name = "Rick Sanchez", string status = "Alive")
        {
            return new CharacterResult
            {
                Id = id,
                Name = name,
                Status = status,
                Species = "Human",
                Type = "",
                Gender = "Male",
                Origin = new PlanetResult
                {
                    Id = 1,
                    Name = "Earth (C-137)",
                    Type = "Planet",
                    Dimension = "Dimension C-137",
                    Url = "https://rickandmortyapi.com/api/location/1"
                },
                Location = new PlanetResult
                {
                    Id = 3,
                    Name = "Citadel of Ricks",
                    Type = "Space station",
                    Dimension = "unknown",
                    Url = "https://rickandmortyapi.com/api/location/3"
                },
                Image = "https://rickandmortyapi.com/api/character/avatar/1.jpeg",
                Episode = new List<string>(),
                Url = "https://rickandmortyapi.com/api/character/1",
                Created = "2017-11-04T18:48:46.250Z"
            };
        }

        public static List<CharacterResult> CreateCharacterResults(int count = 5)
        {
            var characters = new List<CharacterResult>();
            for (int i = 1; i <= count; i++)
            {
                characters.Add(CreateCharacterResult(i, $"Character {i}"));
            }
            return characters;
        }

        public static PlanetResult CreatePlanetResult(int id = 1, string name = "Earth")
        {
            return new PlanetResult
            {
                Id = id,
                Name = name,
                Type = "Planet",
                Dimension = "Dimension C-137",
                Residents = new List<string>(),
                Url = $"https://rickandmortyapi.com/api/location/{id}",
                Created = "2017-11-04T18:48:46.250Z"
            };
        }

        public static Planet CreatePlanet(int id = 1, string name = "Earth")
        {
            return new Planet
            {
                Id = id,
                Name = name,
                Type = "Planet",
                Dimension = "Dimension C-137"
            };
        }

        public static Info CreateInfo(int count = 826, int pages = 42, string? next = null, string? prev = null)
        {
            return new Info
            {
                Count = count,
                Pages = pages,
                Next = next,
                Prev = prev
            };
        }

        public static string CreateCharacterApiResponse(List<CharacterResult> results, Info info)
        {
            return $$"""
            {
                "info": {
                    "count": {{info.Count}},
                    "pages": {{info.Pages}},
                    "next": {{(info.Next != null ? $"\"{info.Next}\"" : "null")}},
                    "prev": {{(info.Prev != null ? $"\"{info.Prev}\"" : "null")}}
                },
                "results": [
                    {{string.Join(",", results.Select(r => $@"
                    {{
                        ""id"": {r.Id},
                        ""name"": ""{r.Name}"",
                        ""status"": ""{r.Status}"",
                        ""species"": ""{r.Species}"",
                        ""type"": ""{r.Type}"",
                        ""gender"": ""{r.Gender}"",
                        ""origin"": {{
                            ""name"": ""{r.Origin.Name}"",
                            ""url"": ""{r.Origin.Url}""
                        }},
                        ""location"": {{
                            ""name"": ""{r.Location.Name}"",
                            ""url"": ""{r.Location.Url}""
                        }},
                        ""image"": ""{r.Image}"",
                        ""episode"": [],
                        ""url"": ""{r.Url}"",
                        ""created"": ""{r.Created}"",
                    }}"))}}
                ]
            }
            """;
        }
    }
}