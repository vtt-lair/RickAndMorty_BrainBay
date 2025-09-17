using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RickAndMorty.Importer.Models
{
    public class PlanetResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Dimension { get; set; }
        public List<string> Residents { get; set; }
        public string Url { get; set; }
        public string Created { get; set; }
    }
}
