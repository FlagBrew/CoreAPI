using System.Collections.Generic;
using CoreAPI.Helpers;
namespace CoreAPI.Models
{
    public class Encounter
    {
        public string Generation { get; }
        public List<GenerationLocation> Encounters { get; }

        public Encounter(string pokemon, string generation, string[] moves)
        {
            Generation = generation;
            Encounters = Utils.GetLocations(pokemon, generation, moves);
        }

        public class GenerationLocation
        {
            public string EncounterType { get; set; }
            public List<Location> Locations { get; set; }
        }

        public class Location
        {
            public string Name { get; set; }
            public string[] Games { get; set; }
        }
    }
}
