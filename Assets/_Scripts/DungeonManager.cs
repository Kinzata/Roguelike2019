
using System;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager
{
    public Level currentLevel;
    public DungeonLevelNode currentLevelNode;

    private FieldOfViewSystem fovSystem;

    public string dungeonString;
    public int floorCounter;
    public string seed;
    public System.Random ranGen;

    public int playerViewDistance = 10;

    public DungeonManager()
    {
    }

    public string GenerateDungeonString()
    {
        // TODO: This needs to generate from the same seed.
        return RewriteRule.Generate();
    }

    public void InitNewGame(string dungeonString, string seed = "")
    {
        this.dungeonString = dungeonString;
        floorCounter = 1;

        if (string.IsNullOrWhiteSpace(seed))
        {
            seed = DateTime.UtcNow.Ticks.ToString();
        }

        this.seed = seed;

        currentLevelNode = BuildNodeMap(seed, dungeonString, 1);

        ranGen = new System.Random(seed.GetHashCode());

        currentLevel = new Level();
        currentLevel.BuildLevel(currentLevelNode);

        // Build Player
        var player = Entity.CreateEntity().Init(currentLevel.GetEntryPosition().Clone(), spriteType: SpriteType.Soldier_Sword, color: Color.green, name: "player", blocks: true);
        player.gameObject.AddComponent<Player>().owner = player;
        player.gameObject.AddComponent<Fighter>().Init(30, 2, 5).owner = player;
        player.gameObject.AddComponent<Inventory>().Init(capacity: 10).owner = player;

        currentLevel.SetPlayer(player);

        // Setup Systems
        fovSystem = new FieldOfViewSystem(currentLevel.GetMapDTO().GroundMap);
        fovSystem.Run(new Vector2Int(player.position.x, player.position.y), playerViewDistance);

        currentLevel.FinalSetup();

        EventManager.Instance.onPlayerEndTurn += OnPlayerEndTurnHandler;
        EventManager.Instance.onTurnSuccess += OnTurnSuccessHandler;
    }

    public void NewLevelFrom(Stairs stair)
    {
        currentLevel.TearDown();

        var player = currentLevel.GetPlayer();

        floorCounter++;
        currentLevelNode = currentLevelNode.nextNode;

        currentLevel = new Level();
        currentLevel.BuildLevel(currentLevelNode);

        player.position = currentLevel.GetEntryPosition();
        player.transform.position = player.position.ToVector3Int();
        currentLevel.SetPlayer(player);

        fovSystem = new FieldOfViewSystem(currentLevel.GetMapDTO().GroundMap);
        fovSystem.Run(new Vector2Int(player.position.x, player.position.y), playerViewDistance);

        currentLevel.FinalSetup();
    }

    public Entity GetPlayer()
    {
        return currentLevel.GetPlayer();
    }

    public MapDTO GetMapDTO()
    {
        return currentLevel.GetMapDTO();
    }

    public List<Actor> GetActors()
    {
        return currentLevel.GetActors();
    }

    // Replace with an event
    private void RunVisibilitySystem()
    {
        currentLevel.RunVisibilitySystem();
    }

    // Replace with an event
    private void RunFovSystem()
    {
        var player = currentLevel.GetPlayer();
        fovSystem.Run(new Vector2Int(player.position.x, player.position.y), playerViewDistance);
    }

    private void OnPlayerEndTurnHandler()
    {
        RunFovSystem();
    }

    private void OnTurnSuccessHandler()
    {
        currentLevel.OnTurnSuccess();
        RunVisibilitySystem();
    }

    public DungeonLevelNode BuildNodeMap(string seed, string dungeonString, int depth)
    {
        DungeonLevelNode firstNode = null;
        DungeonLevelNode currentNode = null;

        var nodeChar = this.dungeonString.ToCharArray()[depth - 1];
        while( nodeChar != 'X')
        {
            var nodeSeed = seed + dungeonString.Substring(0, depth);
            var node = new DungeonLevelNode(nodeSeed, depth, this.dungeonString.ToCharArray()[depth - 1]);
            if (currentNode == null)
            {
                node.previousNode = null;
                currentNode = node;
                firstNode = node;
            }
            else
            {
                currentNode.nextNode = node;
                node.previousNode = currentNode;
                currentNode = node;
            }
            depth++;
            nodeChar = this.dungeonString.ToCharArray()[depth - 1];
        }

        return firstNode;
    }

    public SaveData SaveGameState()
    {
        return new SaveData
        {
            seed = seed,
            dungeonString = dungeonString,
            floorCounter = floorCounter,
            currentLevelNode = currentLevelNode.SaveGameState(true, true),
            currentLevel = currentLevel.SaveGameState()
        };
    }

    public void LoadGameState(SaveData data)
    {
        seed = data.seed;
        dungeonString = data.dungeonString;
        floorCounter = data.floorCounter;

        currentLevelNode = DungeonLevelNode.LoadGameState(data.currentLevelNode);

        currentLevel = new Level();
        
        currentLevel.LoadGameState(data.currentLevel, currentLevelNode);

        fovSystem = new FieldOfViewSystem(currentLevel.GetMapDTO().GroundMap);
        RunFovSystem();

        currentLevel.FinalSetup();

        EventManager.Instance.onPlayerEndTurn += OnPlayerEndTurnHandler;
        EventManager.Instance.onTurnSuccess += OnTurnSuccessHandler;
    }

    public class SaveData
    {
        public string seed;
        public string dungeonString;
        public int floorCounter;
        public DungeonLevelNode.SaveData currentLevelNode;
        public Level.SaveData currentLevel;
    }
}
