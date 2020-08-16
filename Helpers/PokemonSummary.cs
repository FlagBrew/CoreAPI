using Newtonsoft.Json;
using PKHeX.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        public new string Legal { get;  }
        public int ItemNum { get; }
        public string IllegalReasons { get; }
        public string HT { get; }
        public string QR { get; }
        public bool IsLegal { get; }
        public List<string> Ribbons { get; }
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
            IsLegal = LC.Valid;
            QR = Utils.GenerateQR(QRMessageUtil.GetMessage(pkm));
        }

    }
}
