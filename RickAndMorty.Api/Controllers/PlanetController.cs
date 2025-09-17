using Microsoft.AspNetCore.Mvc;
using RickAndMorty.Services.Planet.Commands;
using Models = RickAndMorty.Services.Models;

namespace RickAndMorty.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanetController : ControllerBase
    {
        private readonly ISavePlanetCommand _savePlanetCommand;
        private readonly IDeleteAllPlanetsCommand _deleteAllPlanetsCommand;

        public PlanetController(ISavePlanetCommand savePlanetCommand, IDeleteAllPlanetsCommand deleteAllPlanetsCommand)
        {
            _savePlanetCommand = savePlanetCommand ?? throw new ArgumentNullException(nameof(savePlanetCommand));
            _deleteAllPlanetsCommand = deleteAllPlanetsCommand ?? throw new ArgumentNullException(nameof(deleteAllPlanetsCommand));
            _deleteAllPlanetsCommand = deleteAllPlanetsCommand;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Models.Planet planet)
        {
            var p = await _savePlanetCommand.ExecuteAsync(planet);
            if (p == null) return BadRequest("Planet could not be saved.");

            return Ok(p);
        }

        [HttpDelete("DeleteAllPlanets")]
        public async Task<IActionResult> DeleteAllPlanets()
        {
            var deleted = await _deleteAllPlanetsCommand.ExecuteAsync();
            return Ok(deleted);
        }
    }
}
