using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private Entity player;
    public PlayerStatInterface statText;
    private GameEventListener moveListener;
    private Vector2Int playerNextMoveDirection;
    private GameState gameState;
    private MessageLog log;

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
    public int maxEnemiesInRoom = 1;

    [Header("Systems")]
    private FieldOfViewSystem fovSystem;

    private IList<Actor> actors;

    void Start()
    {
        roomSizeRange = IntRange.CreateInstance<IntRange>();
        roomSizeRange.min = 6;
        roomSizeRange.max = 10;

        Application.targetFrameRate = 120;

        var groundTileMap = GameObject.Find(TileMapType.GroundMap.Name()).GetComponent<Tilemap>();
        groundMap = ScriptableObject.CreateInstance<GroundMap>().Init(mapWidth, mapHeight, groundTileMap);
        var startLocation = groundMap.MakeMap(maxRooms, roomSizeRange, mapWidth, mapHeight);

        var entityTileMap = GameObject.Find(TileMapType.EntityMap.Name()).GetComponent<Tilemap>();
        entityMap = ScriptableObject.CreateInstance<EntityMap>().Init(entityTileMap, groundMap);

        var entityBackgroundTileMap = GameObject.Find(TileMapType.EntityMap_Background.Name()).GetComponent<Tilemap>();
        entityMapBackground = ScriptableObject.CreateInstance<EntityMap>().Init(entityBackgroundTileMap, groundMap);

        // Test Item
        var potion = new Entity(startLocation, spriteType: SpriteType.Item_Potion_Full, name: "potion");
        entityMap.AddEntity(potion);

        actors = new List<Actor>();

        // Build Player
        var fighter = new Fighter(30, 2, 5);
        var playerComponent = new Player();
        player = new Entity(startLocation, spriteType: SpriteType.Soldier_Sword, color: Color.green, name: "player", player: playerComponent, fighter: fighter);
        actors.Add(new Actor(player));

        Camera.main.transform.position = new Vector3(player.position.x, player.position.y, Camera.main.transform.position.z);

        // Build Enemies
        var newEntities = groundMap.FillRoomsWithEnemies(entityMap.GetEntities(), maxEnemiesInRoom);
        foreach(var enemy in newEntities ){
            actors.Add(new Actor(enemy));
        }
        entityMap.AddEntity(player);

        // Setup Systems
        fovSystem = new FieldOfViewSystem(groundMap);
        fovSystem.Run(new Vector2Int(player.position.x, player.position.y), 10);

        // Final Setup
        InitEventListeners();
        groundMap.UpdateTiles();

        statText.SetPlayer(player);
        gameState = GameState.Turn_Player;

        log = FindObjectOfType<MessageLog>();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle User Input (yes we're doing this elsewhere too, plan on fixing that)
        if (Input.GetMouseButtonDown(0))
        {
            var tilePos = groundMap.map.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            ReportObjectsAtPosition(new CellPosition(tilePos));
        }

        ClearAll();

        var playerTurnResults = PlayerMove();
        ProcessTurnResults(playerTurnResults);
        var enemyTurnResults = EnemyMove();
        ProcessTurnResults(enemyTurnResults);

        RenderAll();
    }

    void RenderAll()
    {
        entityMapBackground.RenderAll();
        entityMap.RenderAll();
    }

    public void ClearAll()
    {
        entityMapBackground.ClearAll();
        entityMap.ClearAll();
    }

    ActionResult EnemyMove()
    {
        var actionResult = new ActionResult();
        if (gameState == GameState.Turn_Enemy)
        {
            foreach( Actor actor in actors ){
                var enemy = actor.entity.enemy && actor.entity.aiComponent != null ? actor.entity.aiComponent : null;
                if( enemy == null ) { continue; }

                var action = enemy.GetAction(entityMap, groundMap);
                actionResult.Append(action.PerformAction());

                // break processing if player dies
                if( actionResult.GetEntityEvent("dead").Where( e => e == player).Any() ){
                    break;
                }
            }

            gameState = GameState.Turn_Player;
        }

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
            }
            foreach (var message in actionResult.GetMessages()) { log.AddMessage(message); }
        }
    }

    private void ReportObjectsAtPosition(CellPosition pos){
        var entityNames = entityMap.GetEntities().Where(e => e.position == pos).Select(e => e.GetColoredName());
        var backgroundNames = entityMapBackground.GetEntities().Where(e => e.position == pos).Select(e => e.GetColoredName());

        var entitiesToLog = entityNames.Concat(backgroundNames);
        var message = "There is nothing there.";

        if( entitiesToLog.Count() > 0 ) {
            var names = string.Join(", ", entitiesToLog);
            message = $"You see: {names}";
        }
        
        log.AddMessage(new Message(message, null));
    }

    /** Player specific event functions */
    private void InitEventListeners()
    {
        moveListener = gameObject.AddComponent<GameEventListener>();
        moveListener.Event = EventManager.instance.GetGameEvent(EventManager.EventType.PlayerMove);
        moveListener.Register();
        moveListener.Response = new UnityEventWithCoords();
        moveListener.Response.AddListener(SetMoveDirection);
    }

    void SetMoveDirection(Vector2Int direction)
    {
        playerNextMoveDirection = direction;
    }

    ActionResult PlayerMove()
    {
        var actionResult = new ActionResult();
        if (gameState == GameState.Turn_Player && playerNextMoveDirection != Vector2Int.zero)
        {
            (int x, int y) newPosition = (player.position.x + playerNextMoveDirection.x, player.position.y + playerNextMoveDirection.y);

            if (!groundMap.IsBlocked(newPosition.x, newPosition.y))
            {
                var target = entityMap.GetBlockingEntityAtPosition(newPosition.x, newPosition.y);

                if (target != null) { actionResult.Append(player.fighterComponent.Attack(target)); }
                else
                {
                    player.Move(playerNextMoveDirection.x, playerNextMoveDirection.y);
                    Camera.main.transform.position = new Vector3(player.position.x, player.position.y, Camera.main.transform.position.z);
                    fovSystem.Run(new Vector2Int(player.position.x, player.position.y), 10);
                    groundMap.UpdateTiles();
                }

                // Player ends their turn ONLY IF THEY ACTUALLY DO SOMETHING
                // Attempting to move into a wall, should not waste a turn (unless they attack it)
                gameState = GameState.Turn_Enemy;
            }
            playerNextMoveDirection = Vector2Int.zero;
        }

        return actionResult;
    }
}
