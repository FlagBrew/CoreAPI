using coreconsole.handlers;
using PKHeX.Core;

namespace coreconsole.Models;

public class PublicEntitySummary : EntitySummary
{
    public PublicEntitySummary(PKM p, GameStrings strings) : base(p, strings)
    {
    }
}

public struct Pokemon
{
    public Pokemon(PKM pkmn, EntitySummary summary)
    {
        Nickname = summary.Nickname;
        Species = summary.Species;
        Nature = summary.Nature;
        Gender = summary.Gender;
        Esv = summary.ESV;
        HpType = summary.HP_Type;
        Ability = summary.Ability;
        HeldItem = pkmn.HeldItem == 0 ? "None" : summary.HeldItem;
        Ball = summary.Ball;
        Version = summary.Version;
        OtLang = summary.OTLang;
        Ec = summary.EC;
        Pid = summary.PID;
        Exp = summary.EXP;
        Level = summary.Level;
        Markings = summary.Markings;
        Ht = summary.NotOT;
        AbilityNum = summary.AbilityNum;
        GenderFlag = summary.GenderFlag;
        FormNum = summary.Form;
        PkrsStrain = summary.PKRS_Strain;
        PkrsDays = summary.PKRS_Days;
        FatefulEncounter = summary.FatefulEncounter;
        IsEgg = summary.IsEgg;
        IsNicknamed = summary.IsNicknamed;
        IsShiny = summary.IsShiny;
        Tid = summary.TID16;
        Sid = summary.SID16;
        Tsv = summary.TSV;
        Checksum = summary.Checksum;
        Friendship = summary.Friendship;

        Stats = new List<Stat>
        {
            new("HP", summary.HP_IV, summary.HP_EV, summary.HP),
            new("Attack", summary.ATK_IV, summary.ATK_EV, summary.ATK),
            new("Defense", summary.DEF_IV, summary.DEF_EV, summary.DEF),
            new("Special Attack", summary.SPA_IV, summary.SPA_EV, summary.SPA),
            new("Special Defense", summary.SPD_IV, summary.SPD_EV, summary.SPD),
            new("Speed", summary.SPE_IV, summary.SPE_EV, summary.SPE)
        };

        Moves = new List<Move>
        {
            new(pkmn.Move1, summary.Move1, pkmn.Context, pkmn.Move1_PP, pkmn.Move1_PPUps),
            new(pkmn.Move2, summary.Move2, pkmn.Context, pkmn.Move2_PP, pkmn.Move2_PPUps),
            new(pkmn.Move3, summary.Move3, pkmn.Context, pkmn.Move3_PP, pkmn.Move3_PPUps),
            new(pkmn.Move4, summary.Move4, pkmn.Context, pkmn.Move4_PP, pkmn.Move4_PPUps)
        };
        MoveInfo.GetType(pkmn.Move1, pkmn.Context);
        RelearnMoves = new List<Move>
        {
            new(pkmn.RelearnMove1, summary.Relearn1, pkmn.Context, null, null),
            new(pkmn.RelearnMove2, summary.Relearn2, pkmn.Context, null, null),
            new(pkmn.RelearnMove3, summary.Relearn3, pkmn.Context, null, null),
            new(pkmn.RelearnMove4, summary.Relearn4, pkmn.Context, null, null)
        };

        ContestStats = new List<ContestStat>
        {
            new("Beauty", summary.Beauty),
            new("Cool", summary.Cool),
            new("Cute", summary.Cute),
            new("Sheen", summary.Sheen),
            new("Smart", summary.Smart),
            new("Tough", summary.Tough)
        };

        var tmpRibbons = new List<string>();
        RibbonInfo.GetRibbonInfo(pkmn).ForEach(info =>
        {
            if (info.HasRibbon) tmpRibbons.Add(info.Name);
        });

        Ribbons = tmpRibbons;
        MetData = new MetData(summary.MetLoc, summary.MetLevel, summary.Met_Year, summary.Met_Month, summary.Met_Day);
        EggData = new EggData(summary.EggLoc, summary.Egg_Year, summary.Egg_Month, summary.Egg_Day);
        OtGender = GameInfo.GenderSymbolASCII[pkmn.OT_Gender];
        DexNumber = pkmn.Species;
        StoredSize = pkmn.SIZE_STORED;
        PartySize = pkmn.SIZE_PARTY;
        ItemNum = pkmn.HeldItem;
        Generation = pkmn.Generation;
        VersionNum = pkmn.Version;
        Base64 = Convert.ToBase64String(PartySize > StoredSize ? pkmn.DecryptedPartyData : pkmn.DecryptedBoxData);

        if (Version == "") Version = Enum.GetName(typeof(GameVersion), VersionNum) ?? "???";

        var legality = Legality.CheckLegality(pkmn);

        IsLegal = legality.Valid;
        Report = legality.Report();
        SpriteSet = new Sprites(summary, pkmn);
    }

    public Sprites SpriteSet { get; set; }

    public bool IsLegal { get; set; }

    public string Report { get; set; }

    public string Base64 { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global
    public int StoredSize { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global
    public int PartySize { get; set; }

    public EggData EggData { get; set; }
    public List<ContestStat> ContestStats { get; set; }
    public List<Move> Moves { get; set; }
    public List<Move> RelearnMoves { get; set; }
    public List<Stat> Stats { get; set; }
    public List<string> Ribbons { get; set; }
    public MetData MetData { get; set; }
    public bool FatefulEncounter { get; set; }
    public bool IsEgg { get; set; }
    public bool IsNicknamed { get; set; }
    public bool IsShiny { get; set; }
    public byte FormNum { get; set; }
    public int AbilityNum { get; set; }
    public int Friendship { get; set; }
    public int GenderFlag { get; set; }
    public int Generation { get; set; }
    public int ItemNum { get; set; }
    public int Level { get; set; }
    public int Markings { get; set; }

    // ReSharper disable once IdentifierTypo
    public int PkrsDays { get; set; }

    // ReSharper disable once IdentifierTypo
    public int PkrsStrain { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global
    public int VersionNum { get; set; }
    public string Ability { get; set; }
    public string Ball { get; set; }
    public string Ec { get; set; }
    public string Esv { get; set; }
    public string Gender { get; set; }
    public string HeldItem { get; set; }
    public string HpType { get; set; }
    public string Ht { get; set; }
    public string Nature { get; set; }
    public string Nickname { get; set; }
    public string OtGender { get; set; }
    public string OtLang { get; set; }
    public string Pid { get; set; }
    public string Species { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global
    public string Version { get; set; }
    public uint Exp { get; set; }
    public uint Tsv { get; set; }
    public ushort Checksum { get; set; }
    public ushort DexNumber { get; set; }
    public ushort Sid { get; set; }
    public ushort Tid { get; set; }
}