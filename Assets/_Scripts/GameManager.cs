using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private Entity player;
    public PlayerStatInterface statText;
    private GameState gameState;
    private MessageLog log;
    private int currentActorId = 0;

    [Header("Entites")]
    private EntityMap entityMap;
    private EntityMap entityMapBackground;


    [Header("Floor")]
    private GroundMap groundMap;

    [Header("World Properties")]
    public int mapWidth = 80;
    public int mapHeight = 60;
    public IntRange roomSizeRange;
    public int maxRooms = 30;
    public int maxEnemiesInRoom = 3;
    public int maxItemsInRoom = 2;

    [Header("Systems")]
    private FieldOfViewSystem fovSystem;

    private List<Actor> actors;

    void Start()
    {
        roomSizeRange = IntRange.CreateInstance<IntRange>();
        roomSizeRange.min = 6;
        roomSizeRange.max = 10;

        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;

        var groundTileMap = GameObject.Find(TileMapType.GroundMap.Name()).GetComponent<Tilemap>();
        var levelBuilder = new LevelBuilder();
        groundMap = levelBuilder.MakeMap(maxRooms, roomSizeRange, mapWidth, mapHeight, groundTileMap);
        var startLocation = levelBuilder.GetStartPosition();

        var entityTileMap = GameObject.Find(TileMapType.EntityMap.Name()).GetComponent<Tilemap>();
        entityMap = ScriptableObject.CreateInstance<EntityMap>().Init(entityTileMap, groundMap);

        var entityBackgroundTileMap = GameObject.Find(TileMapType.EntityMap_Background.Name()).GetComponent<Tilemap>();
        entityMapBackground = ScriptableObject.CreateInstance<EntityMap>().Init(entityBackgroundTileMap, groundMap);

        actors = new List<Actor>();

        // Build Player
        player = Entity.CreateEntity().Init(startLocation.Clone(), spriteType: SpriteType.Soldier_Sword, color: Color.green, name: "player");
        player.gameObject.AddComponent<Player>().owner = player;
        player.gameObject.AddComponent<Fighter>().Init(30, 2, 5).owner = player;
        player.gameObject.AddComponent<Inventory>().Init(capacity: 10).owner = player;
        actors.Add(new Actor(player));

        Camera.main.transform.position = new Vector3(player.position.x, player.position.y, Camera.main.transform.position.z);

        // Build Enemies
        var newEntities = levelBuilder.FillRoomsWithEntityActors(entityMap.GetEntities(), maxEnemiesInRoom, maxItemsInRoom);
        foreach (var enemy in newEntities)
        {
            actors.Add(new Actor(enemy));
        }

        levelBuilder.FillRoomsWithPassiveEntities(entityMapBackground.GetEntities(), maxEnemiesInRoom, maxItemsInRoom);

        entityMap.AddEntity(player);

        // Setup Systems
        fovSystem = new FieldOfViewSystem(groundMap);
        fovSystem.Run(new Vector2Int(player.position.x, player.position.y), 10);

        RunVisibilitySystem();

        // Final Setup
        groundMap.UpdateTiles();

        statText.SetPlayer(player);
        gameState = GameState.Turn_Player;

        log = FindObjectOfType<MessageLog>();
    }

    void Update()
    {
        // Handle User Input (yes we're doing this elsewhere too, plan on fixing that)
        HandleUserInput();

        var turnResults = ProcessTurn();
        ProcessTurnResults(turnResults);

    }

    ActionResult ProcessTurn()
    {
        var actionResult = new ActionResult();
        var actor = actors.ElementAt(currentActorId);
        var action = actor.GetAction(entityMap, groundMap);
        var actionToTake = action;
        if (action == null) { return new ActionResult(); }

        do
        {
            actionResult = actionToTake.PerformAction();

            // Cleanup to handle after player potentially changes position
            Camera.main.transform.position = new Vector3(player.position.x, player.position.y, Camera.main.transform.position.z);

            actionToTake = actionResult.nextAction;
        }
        while (actionResult.nextAction != null);


        if (actionResult.success)
        {
            currentActorId = (currentActorId + 1) % actors.Count();
            if (actor.entity == player)
            {
                fovSystem.Run(new Vector2Int(player.position.x, player.position.y), 10);
                groundMap.UpdateTiles();
            }
        }

        ProcessNewState();

        return actionResult;
    }

    void ProcessTurnResults(ActionResult results)
    {
        foreach (var message in results.GetMessages()) { log.AddMessage(message); }
        var deadEntities = results.GetEntityEvent("dead");
        if (deadEntities.Count() > 0)
        {
            var actionResult = new ActionResult();
            foreach (var dead in deadEntities)
            {
                if (dead == player)
                {
                    actionResult.Append(dead.ConvertToDeadPlayer());
                    gameState = GameState.Global_PlayerDead;
                }
                else
                {
                    actionResult.Append(dead.ConvertToDeadMonster());
                }

                entityMap.SwapEntityToMap(dead, entityMapBackground);
                actors.Remove(dead.actor);
            }
            foreach (var message in actionResult.GetMessages()) { log.AddMessage(message); }
        }
    }

    void ProcessNewState()
    {
        RunVisibilitySystem();
    }

    void RunVisibilitySystem()
    {
        entityMapBackground.RenderAll();
        entityMap.RenderAll();
    }

    private void ReportObjectsAtPosition(CellPosition pos)
    {
        var entityNames = entityMap.GetEntities().Where(e => e.position == pos).Select(e => e.GetColoredName());
        var backgroundNames = entityMapBackground.GetEntities().Where(e => e.position == pos).Select(e => e.GetColoredName());

        var entitiesToLog = entityNames.Concat(backgroundNames);
        var message = "There is nothing there.";

        if (entitiesToLog.Count() > 0)
        {
            var names = string.Join(", ", entitiesToLog);
            message = $"You see: {names}";
        }

        log.AddMessage(new Message(message, null));
    }

    void SetMoveDirection(Vector2Int direction)
    {
        CellPosition newPosition = new CellPosition(player.position.x + direction.x, player.position.y + direction.y);
        var action = new WalkAction(player.actor, entityMap, groundMap, newPosition);
        // playerNextAction = action;
        player.actor.SetNextAction(action);
    }

    void HandleUserInput()
    {
        // Look Action
        if (Input.GetMouseButtonDown(0))
        {
            var mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mPos.x += 0.5f;
            mPos.y += 0.5f;
            var tilePos = groundMap.map.WorldToCell(mPos);
            ReportObjectsAtPosition(new CellPosition(tilePos));
        }

        Vector2Int direction = Vector2Int.zero;
        // Cardinals
        if (Input.GetKeyDown(KeyCode.Keypad4))
            direction.x = -1;
        if (Input.GetKeyDown(KeyCode.Keypad6))
            direction.x = 1;
        if (Input.GetKeyDown(KeyCode.Keypad2))
            direction.y = -1;
        if (Input.GetKeyDown(KeyCode.Keypad8))
            direction.y = 1;

        // Diagonals
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            direction.x = -1; direction.y = 1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            direction.x = -1; direction.y = -1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            direction.x = 1; direction.y = -1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            direction.x = 1; direction.y = 1;
        }

        if (direction.x != 0 || direction.y != 0)
        {
            SetMoveDirection(direction);
        }
    }
}
