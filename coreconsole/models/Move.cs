using PKHeX.Core;

namespace coreconsole.Models;

public struct Move
{
    // public static List<MoveType>? MoveTypes;

    public string Name { get; set; }
    public string Type { get; set; }
    public int Pp { get; set; }
    public int PpUps { get; set; }

    public Move(ushort id, string name, EntityContext context, int? pp, int? ppUps)
    {
        Name = id == 0 ? "None" : name;
        Enum.TryParse(MoveInfo.GetType(id, context).ToString(), out MoveType type);

        Type = id == 0 ? "Normal" : type.ToString();
        Pp = pp ?? MoveInfo.GetPP(context, id);
        PpUps = ppUps ?? 0;
    }
}

// public struct MoveType
// {
//     // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
//     [JsonPropertyName("move_id")] public int MoveId { set; get; }
//
//     // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
//     [JsonPropertyName("type_id")] public int TypeId { set; get; }
//
//     public MoveType(int moveId, int typeId)
//     {
//         MoveId = moveId;
//         TypeId = typeId;
//     }
// }