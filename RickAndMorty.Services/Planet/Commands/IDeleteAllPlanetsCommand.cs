using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMorty.Services.Planet.Commands
{
    public interface IDeleteAllPlanetsCommand
    {
        Task<bool> ExecuteAsync();
    }
}
