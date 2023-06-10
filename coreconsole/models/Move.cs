using System.Text.Json.Serialization;
using PKHeX.Core;

namespace coreconsole.Models;

public struct Move
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("type")] public string Type { get; set; }
    [JsonPropertyName("pp")] public int Pp { get; set; }
    [JsonPropertyName("pp_ups")] public int PpUps { get; set; }

    public Move(ushort id, string name, EntityContext context, int? pp, int? ppUps)
    {
        Name = id == 0 ? "None" : name;
        Enum.TryParse(MoveInfo.GetType(id, context).ToString(), out MoveType type);

        Type = id == 0 ? "Normal" : type.ToString();
        Pp = pp ?? MoveInfo.GetPP(context, id);
        PpUps = ppUps ?? 0;
    }
}