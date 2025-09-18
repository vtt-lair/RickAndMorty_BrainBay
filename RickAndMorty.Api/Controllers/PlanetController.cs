using Microsoft.AspNetCore.Mvc;
using RickAndMorty.Services.Character.Queries;
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
        private readonly IGetPlanetsQuery _getPlanetsQuery;

        public PlanetController(ISavePlanetCommand savePlanetCommand, IDeleteAllPlanetsCommand deleteAllPlanetsCommand, IGetPlanetsQuery getPlanetsQuery)
        {
            _savePlanetCommand = savePlanetCommand ?? throw new ArgumentNullException(nameof(savePlanetCommand));
            _deleteAllPlanetsCommand = deleteAllPlanetsCommand ?? throw new ArgumentNullException(nameof(deleteAllPlanetsCommand));
            _getPlanetsQuery = getPlanetsQuery ?? throw new ArgumentNullException(nameof(getPlanetsQuery));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var planets = await _getPlanetsQuery.ExecuteAsync();
            return Ok(planets);
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
