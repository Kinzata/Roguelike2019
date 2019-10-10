using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

        var spriteLoader = SpriteLoader.instance;
        var playerSprite = spriteLoader.LoadSprite(SpriteType.Soldier_Sword);

        var fighter = new Fighter(30, 2, 5);
        var playerComponent = new Player();
        player = new Entity(new Vector3Int(startLocation.x, startLocation.y, 0), playerSprite, Color.green, player: playerComponent, fighter: fighter);

        Camera.main.transform.position = new Vector3(player.position.x, player.position.y, Camera.main.transform.position.z);

        fovSystem = new FieldOfViewSystem(groundMap);
        fovSystem.Run(new Vector2Int(player.position.x, player.position.y), 10);

        groundMap.FillRoomsWithEnemies(entityMap.GetEntities(), maxEnemiesInRoom);
        entityMap.AddEntity(player);

        InitEventListeners();
        groundMap.UpdateTiles();

        statText.SetPlayer(player);
        gameState = GameState.Turn_Player;

        log = FindObjectOfType<MessageLog>();
    }

    // Update is called once per frame
    void Update()
    {
        ClearAll();

        var playerTurnResults = PlayerMove();
        ProcessTurnResults(playerTurnResults);
        var enemyTurnResults = EnemyMove();
        ProcessTurnResults(enemyTurnResults);

        RenderAll();
    }

    void RenderAll()
    {
        entityMap.RenderAll();
    }

    public void ClearAll()
    {
        entityMap.ClearAll();
    }

    ActionResult EnemyMove()
    {
        var actionResult = new ActionResult();
        if (gameState == GameState.Turn_Enemy)
        {
            foreach (BasicMonsterAi enemy in entityMap.GetEnemies())
            {
                actionResult.Append(enemy.TakeTurn(entityMap, groundMap));
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
            foreach( var dead in deadEntities ){
                if( dead == player ){
                    actionResult.Append(dead.ConvertToDeadPlayer());
                    gameState = GameState.Global_PlayerDead;
                }
                else {
                    actionResult.Append(dead.ConvertToDeadMonster());
                }

                entityMap.SwapEntityToMap(dead, entityMapBackground);
            }
            foreach (var message in actionResult.GetMessages()) { log.AddMessage(message);  }
        }
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
