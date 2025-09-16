using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RickAndMorty.Services.Character.Commands;
using RickAndMorty.Services.Character.Queries;
using Models = RickAndMorty.Services.Models;

namespace RickAndMorty.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly IGetCharactersQuery _getCharactersQuery;
        private readonly ISaveCharacterCommand _saveCharacterCommand;
        private readonly IMemoryCache _cache;
        
        private const string cacheKey = "characters";

        public CharacterController(IGetCharactersQuery getCharactersQuery, ISaveCharacterCommand saveCharacterCommand, IMemoryCache cache) 
        {
            _getCharactersQuery = getCharactersQuery ?? throw new ArgumentNullException(nameof(getCharactersQuery));
            _saveCharacterCommand = saveCharacterCommand ?? throw new ArgumentNullException(nameof(saveCharacterCommand));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {            
            bool fromDatabase = false;
            IEnumerable<Models.Character>? characters;

            if (!_cache.TryGetValue(cacheKey, out characters))
            {
                characters = await _getCharactersQuery.ExecuteAsync();
                _cache.Set(cacheKey, characters, TimeSpan.FromMinutes(5));
                fromDatabase = true;
            }

            Response.Headers.Append("from-database", fromDatabase ? "true" : "false");
            return Ok(characters);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Models.Character character)
        {
            var updated = await _saveCharacterCommand.ExecuteAsync(character);
            if (updated) _cache.Remove(cacheKey);
            
            return Ok(updated);
        }
    }
}
