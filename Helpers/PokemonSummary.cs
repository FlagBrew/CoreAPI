using PKHeX.Core;
namespace CoreAPI.Helpers
{
    public class PokemonSummary : PKMSummary
    {
        public string SpeciesSpriteURL { get; }
        public string HeldItemSpriteURL { get; }
        public string Move1_Type { get; }
        public string Move2_Type { get; }
        public string Move3_Type { get; }
        public string Move4_Type { get; }
        public string Form { get; }
        public string Generation { get; }
        public int Size { get; }
        public int ItemNum { get; }
        public string IllegalReasons { get; }
        public string HT { get; }
        public PokemonSummary(PKM pkm, GameStrings strings) : base(pkm, strings)
        {
            Move1_Type = MoveType.MT[pkm.Move1].Type;
            Move2_Type = MoveType.MT[pkm.Move2].Type;
            Move3_Type = MoveType.MT[pkm.Move3].Type;
            Move4_Type = MoveType.MT[pkm.Move4].Type;
            Generation = Utils.GetGeneration(pkm);
            Form = Utils.GetForm(pkm, pkm.AltForm);
            SpeciesSpriteURL = Utils.GetPokeSprite(pkm.Species, Species, Gender, Form, Generation, IsShiny);
            HeldItemSpriteURL = "";
            HT = pkm.HT_Name;
            Size = pkm.SIZE_STORED;
            ItemNum = pkm.HeldItem;
            var LC = new LegalityAnalysis(pkm);
            IllegalReasons = LC.Report();
        }

    }
}
