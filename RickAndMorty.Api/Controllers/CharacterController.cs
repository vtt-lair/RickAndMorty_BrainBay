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
        private readonly IBulkSaveCharacterCommand _bulkSaveCharacterCommand;
        private readonly IDeleteAllCharactersCommand _deleteAllCharactersCommand;
        private readonly IMemoryCache _cache;
        
        private const string cacheKey = "characters";

        public CharacterController(IGetCharactersQuery getCharactersQuery, ISaveCharacterCommand saveCharacterCommand, IBulkSaveCharacterCommand bulkSaveCharacterCommand, IDeleteAllCharactersCommand deleteAllCharactersCommand, IMemoryCache cache) 
        {
            _getCharactersQuery = getCharactersQuery ?? throw new ArgumentNullException(nameof(getCharactersQuery));
            _saveCharacterCommand = saveCharacterCommand ?? throw new ArgumentNullException(nameof(saveCharacterCommand));
            _bulkSaveCharacterCommand = bulkSaveCharacterCommand ?? throw new ArgumentNullException(nameof(bulkSaveCharacterCommand));
            _deleteAllCharactersCommand = deleteAllCharactersCommand ?? throw new ArgumentNullException(nameof(deleteAllCharactersCommand));
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
            if (characters == null) return NotFound("No characters found.");

            Response.Headers.Append("from-database", fromDatabase ? "true" : "false");
            return Ok(characters);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Models.Character character)
        {
            var c = await _saveCharacterCommand.ExecuteAsync(character);
            if (c == null) return BadRequest("Character could not be saved.");
            _cache.Remove(cacheKey);
            
            return Ok(c);
        }

        [HttpPost("Bulk")]
        public async Task<IActionResult> BulkPost(IEnumerable<Models.Character> characters)
        {
            var savedCharacters = await _bulkSaveCharacterCommand.ExecuteAsync(characters);
            if (!savedCharacters) return BadRequest("No characters were saved.");
            _cache.Remove(cacheKey);
            
            return Ok(savedCharacters);
        }

        [HttpDelete("DeleteAllCharacters")]
        public async Task<IActionResult> DeleteAllCharacters()
        {
            var deleted = await _deleteAllCharactersCommand.ExecuteAsync();
            if (deleted) _cache.Remove(cacheKey);
            
            return Ok(deleted);
        }
    }
}
