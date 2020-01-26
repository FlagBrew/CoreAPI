using System;
using System.Collections.Generic;

namespace CoreAPI.Helpers
{
    public class MoveType
    {
        public static readonly string[] Types = { "Normal", "Fighting", "Flying", "Poison", "Ground", "Rock", "Bug", "Ghost", "Steel", "Fire", "Water", "Grass", "Electric", "Psychic", "Ice", "Dragon", "Dark", "Fairy" };
        public static string[] moveNames;
        public static List<MoveType> MT { get; set; }
        internal int Index;
        public string Type;
        internal int TypeIndex;

        public static MoveType ReadCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            MoveType moveType = new MoveType
            {
                Index = Convert.ToInt32(values[0]),
                TypeIndex = Convert.ToInt32(values[1]),
                Type = Types[Convert.ToInt32(values[1])]
            };
            return moveType;
        }
    }

}
