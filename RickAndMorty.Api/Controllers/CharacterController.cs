using Microsoft.AspNetCore.Mvc;
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

        public CharacterController(IGetCharactersQuery getCharactersQuery, ISaveCharacterCommand saveCharacterCommand) 
        {
            _getCharactersQuery = getCharactersQuery ?? throw new ArgumentNullException(nameof(getCharactersQuery));
            _saveCharacterCommand = saveCharacterCommand ?? throw new ArgumentNullException(nameof(saveCharacterCommand));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _getCharactersQuery.ExecuteAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Post(Models.Character character)
        {
          return Ok(await _saveCharacterCommand.ExecuteAsync(character));
        }
    }
}
