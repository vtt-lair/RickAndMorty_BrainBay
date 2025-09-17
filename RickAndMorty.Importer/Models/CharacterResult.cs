using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMorty.Importer.Models
{
    public class CharacterResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Species { get; set; }
        public string Type { get; set; }
        public string Gender { get; set; }
        public PlanetResult Origin { get; set; }
        public PlanetResult Location { get; set; }
        public string Image { get; set; }
        public List<string> Episode { get; set; }
        public string Url { get; set; }
        public string Created { get; set; }
    }
}
