using PKHeX.Core;
using CoreAPI.Helpers;
using System.Collections.Generic;

namespace CoreAPI.Models
{
    public class Pokemon
    {
        public PokemonSummary Summary { get; }
        public Pokemon(PKM pk)
        {
           Summary = new PokemonSummary(pk, GameInfo.Strings);
        }

    }

    public class BasePokemon
    {
        public int HP { get; set; }
        public int ATK { get; set; }
        public int DEF { get; set; }
        public int SPE { get; set; }
        public int SPA { get; set; }
        public int SPD { get; set; }
        public int CatchRate { get; set; }
        public int EvoStage { get; set; }
        public int Gender { get; set; }
        public int HatchCycles { get; set; }
        public int BaseFriendship { get; set; }
        public int EXPGrowth { get; set; }
        public string Ability1 { get; set; }
        public string Ability2 { get; set; }
        public string AbilityH { get; set; }
        public string Color { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public bool HasHiddenAbility { get; set; }
        public List<string> Types { get; set; }
        public List<string> EggGroups { get; set; }
        public bool IsDualGender { get; set; }
        public bool Genderless { get; set; }
        public bool OnlyFemale { get; set; }
        public bool OnlyMale { get; set; }
        public int BST { get; set; }
        public string SpriteURL { get; set; }
        public string SpeciesSpriteURL { get; set; }
    }
}
