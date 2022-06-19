using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// LinkedList style class that represents a node in the dungeon layout
/// Stores all data needed for it's own generation
/// </summary>
public class DungeonLevelNode
{
    public string seed { get; set; }
    public DungeonLevelNode previousNode { get; set; }
    public DungeonLevelNode nextNode { get; set; }
    public int depth { get; set; }
    public LevelDataScriptableObject levelData { get; set; }

    public char levelType { get; set; }

    public DungeonLevelNode(string seed, int depth, char levelType)
    {
        this.seed = seed;
        this.depth = depth;
        this.levelType = levelType;
        levelData = LevelDataSelection.GetLevelDataFromGenerationString(levelType);
    }

    public SaveData SaveGameState(bool saveForward = false, bool saveBackward = false)
    {
        var saveData = new SaveData
        {
            seed = seed,
            depth = depth,
            levelType = levelType
        };

        if( saveForward )
        {
            saveData.nextNode = nextNode?.SaveGameState(saveForward: true);
        }

        if (saveBackward)
        {
            saveData.previousNode = previousNode?.SaveGameState(saveBackward: true);
        }

        return saveData;
    }

    public static DungeonLevelNode LoadGameState(SaveData data, DungeonLevelNode nextNode = null, DungeonLevelNode previousNode = null)
    {
        if( data == null ) { return null; }

        var node = new DungeonLevelNode(
            data.seed,
            data.depth,
            data.levelType
            );
        node.nextNode = nextNode != null ? nextNode : LoadGameState(data.nextNode, previousNode: node);
        node.previousNode = previousNode != null ? previousNode : LoadGameState(data.previousNode, nextNode: node);

        return node;
    }

    public class SaveData
    {
        public string seed;
        public int depth;
        public char levelType;
        public SaveData nextNode;
        public SaveData previousNode;
    }
}
