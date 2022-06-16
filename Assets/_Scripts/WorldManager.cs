
using System.Collections.Generic;
using UnityEngine;

public class WorldManager
{
    public Level currentLevel;
    public LevelDataScriptableObject levelData;

    private FieldOfViewSystem fovSystem;

    public int playerViewDistance = 10;

    public WorldManager(LevelDataScriptableObject levelData)
    {
        this.levelData = levelData;
    }

    public void InitNewGame()
    {
        currentLevel = new Level();
        currentLevel.BuildLevel(levelData);

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

    public void OnPlayerEndTurnHandler()
    {
        RunFovSystem();
    }

    public void OnTurnSuccessHandler()
    {
        currentLevel.OnTurnSuccess();
        RunVisibilitySystem();
    }

    public SaveData SaveGameState()
    {
        return new SaveData
        {
            currentLevel = currentLevel.SaveGameState()
        };
    }

    public void LoadGameState(SaveData data)
    {
        currentLevel = new Level();
        currentLevel.LoadGameState(data.currentLevel, levelData);

        fovSystem = new FieldOfViewSystem(currentLevel.GetMapDTO().GroundMap);
        RunFovSystem();

        currentLevel.FinalSetup();

        EventManager.Instance.onPlayerEndTurn += OnPlayerEndTurnHandler;
        EventManager.Instance.onTurnSuccess += OnTurnSuccessHandler;
    }

    public class SaveData
    {
        public Level.SaveData currentLevel;
    }
}
