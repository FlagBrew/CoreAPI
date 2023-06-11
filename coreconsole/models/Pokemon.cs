using System.Text.Json.Serialization;
using coreconsole.handlers;
using PKHeX.Core;
using Sentry;

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
        Ot = summary.OT;
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
        try
        {
            OtGender = GameInfo.GenderSymbolASCII[pkmn.OT_Gender];
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            OtGender = "";
        }
        DexNumber = pkmn.Species;
        StoredSize = pkmn.SIZE_STORED;
        PartySize = pkmn.SIZE_PARTY;
        ItemNum = pkmn.HeldItem;
        Generation = pkmn.Generation;
        VersionNum = pkmn.Version;
        try
        {
            Base64 = Convert.ToBase64String(PartySize > StoredSize ? pkmn.DecryptedPartyData : pkmn.DecryptedBoxData);
            if (Version == "") Version = Enum.GetName(typeof(GameVersion), VersionNum) ?? "???";
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            Base64 = "";
        }
        
        try
        {
            if (Version == "") Version = Enum.GetName(typeof(GameVersion), VersionNum) ?? "???";
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
            Version = "";
        }

        var legality = Legality.CheckLegality(pkmn);

        IsLegal = legality.Valid;
        Report = legality.Report();
        SpriteSet = new Sprites(summary, pkmn);
    }

    [JsonPropertyName("ot")] public string Ot { get; set; }

    [JsonPropertyName("sprites")] public Sprites SpriteSet { get; set; }

    [JsonPropertyName("is_legal")] public bool IsLegal { get; set; }
    [JsonPropertyName("illegal_reasons")] public string Report { get; set; }
    [JsonPropertyName("base64")] public string Base64 { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global
    [JsonPropertyName("stored_size")] public int StoredSize { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global
    [JsonPropertyName("party_size")] public int PartySize { get; set; }
    [JsonPropertyName("egg_data")] public EggData EggData { get; set; }
    [JsonPropertyName("contest_stats")] public List<ContestStat> ContestStats { get; set; }
    [JsonPropertyName("moves")] public List<Move> Moves { get; set; }
    [JsonPropertyName("relearn_moves")] public List<Move> RelearnMoves { get; set; }
    [JsonPropertyName("stats")] public List<Stat> Stats { get; set; }
    [JsonPropertyName("ribbons")] public List<string> Ribbons { get; set; }
    [JsonPropertyName("met_data")] public MetData MetData { get; set; }
    [JsonPropertyName("fateful_flag")] public bool FatefulEncounter { get; set; }
    [JsonPropertyName("is_egg")] public bool IsEgg { get; set; }
    [JsonPropertyName("is_nicknamed")] public bool IsNicknamed { get; set; }
    [JsonPropertyName("is_shiny")] public bool IsShiny { get; set; }

    [JsonPropertyName("form_num")] public byte FormNum { get; set; }
    [JsonPropertyName("ability_num")] public int AbilityNum { get; set; }
    [JsonPropertyName("friendship")] public int Friendship { get; set; }
    [JsonPropertyName("gender_flag")] public int GenderFlag { get; set; }
    [JsonPropertyName("generation")] public int Generation { get; set; }
    [JsonPropertyName("item_num")] public int ItemNum { get; set; }
    [JsonPropertyName("level")] public int Level { get; set; }
    [JsonPropertyName("markings")] public int Markings { get; set; }

    // ReSharper disable once IdentifierTypo
    [JsonPropertyName("pkrs_days")] public int PkrsDays { get; set; }

    // ReSharper disable once IdentifierTypo
    [JsonPropertyName("pkrs_strain")] public int PkrsStrain { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global
    [JsonPropertyName("version_num")] public int VersionNum { get; set; }
    [JsonPropertyName("ability")] public string Ability { get; set; }
    [JsonPropertyName("ball")] public string Ball { get; set; }
    [JsonPropertyName("ec")] public string Ec { get; set; }
    [JsonPropertyName("esv")] public string Esv { get; set; }
    [JsonPropertyName("gender")] public string Gender { get; set; }
    [JsonPropertyName("held_item")] public string HeldItem { get; set; }
    [JsonPropertyName("hp_type")] public string HpType { get; set; }
    [JsonPropertyName("ht")] public string Ht { get; set; }
    [JsonPropertyName("nature")] public string Nature { get; set; }
    [JsonPropertyName("nickname")] public string Nickname { get; set; }
    [JsonPropertyName("ot_gender")] public string OtGender { get; set; }
    [JsonPropertyName("ot_lang")] public string OtLang { get; set; }
    [JsonPropertyName("pid")] public string Pid { get; set; }
    [JsonPropertyName("species")] public string Species { get; set; }

    // ReSharper disable once MemberCanBePrivate.Global
    [JsonPropertyName("version")] public string Version { get; set; }
    [JsonPropertyName("exp")] public uint Exp { get; set; }
    [JsonPropertyName("tsv")] public uint Tsv { get; set; }
    [JsonPropertyName("checksum")] public ushort Checksum { get; set; }
    [JsonPropertyName("dex_number")] public ushort DexNumber { get; set; }
    [JsonPropertyName("sid")] public ushort Sid { get; set; }
    [JsonPropertyName("tid")] public ushort Tid { get; set; }
}