using UnityEngine;

namespace EncounterSystem.MapEnums
{
    public enum TileAreaType
    {
        Circle,
        Ring,
        Cone,
        Cross,
        Line
    }

    public enum TargetSelectionType
    {
        Auto,
        Targeted,
    }
    static class MapConstants
    {
        public static readonly Color MoveTileColor = Color.blue;
        public static readonly Color ActionTileColor = Color.red;
        public static readonly Color ActionAOETileColor = Color.yellow;
        public static readonly Color CursorTileColor = Color.white;
        public static readonly Color UnitPlacementTileColor = Color.blue;
    }
   
}
