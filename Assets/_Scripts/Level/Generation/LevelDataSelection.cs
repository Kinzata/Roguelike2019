using UnityEngine;

public static class LevelDataSelection
{
    public static LevelDataScriptableObject GetLevelDataFromGenerationString(char rule)
    {
        switch(rule)
        {
            case 'A':
                return GenerateSmallLevel();
            case 'B':
                return GenerateMediumLevel();
            case 'C':
                return GenerateLargeLevel();
            default:
                return GenerateSmallLevel();
        }
    }

    private static LevelDataScriptableObject GenerateSmallLevel()
    {
        var data = ScriptableObject.CreateInstance<LevelDataScriptableObject>();
        data.mapWidth = 40;
        data.mapHeight = 30;
        data.minRoomSize = 4;
        data.maxRoomSize = 8;
        data.maxRooms = 5;
        data.minEnemiesOnFloor = 4;
        data.maxEnemiesOnFloor = 6;
        data.minItemsOnFloor = 2;
        data.maxItemsOnFloor = 6;

        return data;
    }

    private static LevelDataScriptableObject GenerateMediumLevel()
    {
        var data = ScriptableObject.CreateInstance<LevelDataScriptableObject>();
        data.mapWidth = 60;
        data.mapHeight = 50;
        data.minRoomSize = 6;
        data.maxRoomSize = 12;
        data.maxRooms = 9;
        data.minEnemiesOnFloor = 5;
        data.maxEnemiesOnFloor = 9;
        data.minItemsOnFloor = 4;
        data.maxItemsOnFloor = 8;

        return data;
    }

    private static LevelDataScriptableObject GenerateLargeLevel()
    {
        var data = ScriptableObject.CreateInstance<LevelDataScriptableObject>();
        data.mapWidth = 80;
        data.mapHeight = 70;
        data.minRoomSize = 6;
        data.maxRoomSize = 12;
        data.maxRooms = 15;
        data.minEnemiesOnFloor = 8;
        data.maxEnemiesOnFloor = 14;
        data.minItemsOnFloor = 6;
        data.maxItemsOnFloor = 10;

        return data;
    }
}
