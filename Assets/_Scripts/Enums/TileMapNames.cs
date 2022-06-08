public enum TileMapType
{
    EntityMap,
    EntityMap_Background,
    GroundMap,
    MiscMap
}

public static class TileMapNamesExtensions
{
    public static string Name(this TileMapType type)
    {
        switch (type)
        {
            case TileMapType.EntityMap:
                return "EntityMap";
            case TileMapType.EntityMap_Background:
                return "EntityMap-Background";
            case TileMapType.GroundMap:
                return "GroundMap";
            case TileMapType.MiscMap:
                return "MiscMap";
            default:
                return "";
        }
    }
}
