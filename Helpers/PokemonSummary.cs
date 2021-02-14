using Newtonsoft.Json;
using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreAPI.Helpers
{
    public class Move
    {
        public string Move_Name { get; set; }
        public string Move_Type { get; set; }
        public int Move_PP { get; set; }
        public int Move_PP_Up { get; set; }
    }

    public class Stat
    {
        public string Stat_Name { get; set; }
        public int Stat_IV { get; set; }
        public int Stat_EV { get; set; }
        public string Stat_Total { get; set; }
    }
    public class PokemonSummary : PKMSummary
    {
        public string SpeciesSpriteURL { get; }
        public string HeldItemSpriteURL { get; }
        public string Form { get; }
        public string Generation { get; }
        public int Size { get; }
        public int ItemNum { get; }
        public string IllegalReasons { get; }
        public string HT { get; }
        public string QR { get; }
        public bool IsLegal { get; }
        public List<string> Ribbons { get; }
        public string Base64 { get;  }
        public List<Move> Moves { get; }
        public List<Stat> Stats { get; }
        public int DexNumber { get; }


        public PokemonSummary(PKM pkm, GameStrings strings) : base(pkm, strings)
        {
            Ribbons = new List<string>();
            foreach (var ribbon in RibbonInfo.GetRibbonInfo(pkm))
            {
                if (ribbon.HasRibbon)
                {
                    Ribbons.Add(RibbonStrings.GetName(ribbon.Name));
                }
            }
            Moves = new List<Move>
            {
                new Move() { Move_Name = Move1, Move_Type = MoveType.MT[pkm.Move1].Type, Move_PP = Move1_PP, Move_PP_Up = Move1_PPUp },
                new Move() { Move_Name = Move2, Move_Type = MoveType.MT[pkm.Move2].Type, Move_PP = Move2_PP, Move_PP_Up = Move2_PPUp },
                new Move() { Move_Name = Move3, Move_Type = MoveType.MT[pkm.Move3].Type, Move_PP = Move3_PP, Move_PP_Up = Move3_PPUp },
                new Move() { Move_Name = Move4, Move_Type = MoveType.MT[pkm.Move4].Type, Move_PP = Move4_PP, Move_PP_Up = Move4_PPUp }
            };

            Stats = new List<Stat>
            {
                new Stat() { Stat_Name = "HP", Stat_IV = HP_IV, Stat_EV = HP_EV, Stat_Total = HP },
                new Stat() { Stat_Name = "Attack", Stat_IV = ATK_IV, Stat_EV = ATK_EV, Stat_Total = ATK },
                new Stat() { Stat_Name = "Defense", Stat_IV = DEF_IV, Stat_EV = DEF_EV, Stat_Total = DEF },
                new Stat() { Stat_Name = "Speed", Stat_IV = SPE_IV, Stat_EV = SPE_EV, Stat_Total = SPE },
                new Stat() { Stat_Name = "Special Attack", Stat_IV = SPA_IV, Stat_EV = SPA_EV, Stat_Total = SPA },
                new Stat() { Stat_Name = "Special Defense", Stat_IV = SPD_IV, Stat_EV = SPD_EV, Stat_Total = SPD },
            };
            Generation = Utils.GetGeneration(pkm);
            Form = Utils.GetForm(pkm, pkm.Form);
            HeldItemSpriteURL = "";
            HT = pkm.HT_Name;
            DexNumber = pkm.Species;
            Size = pkm.SIZE_STORED;
            ItemNum = pkm.HeldItem;
            var LC = new LegalityAnalysis(pkm);
            IllegalReasons = LC.Report();
            IsLegal = LC.Valid;
            QR = Utils.GenerateQR(QRMessageUtil.GetMessage(pkm));
            Base64 = System.Convert.ToBase64String(pkm.DecryptedBoxData);
            SpeciesSpriteURL = Helpers.Sprite.getFormURL(DexNumber, Generation, Form, IsShiny, Gender, Species);
        }
    }
}
