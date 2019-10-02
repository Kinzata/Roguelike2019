using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private Entity player;
    private GameEventListener moveListener;
    private Vector2Int playerNextMoveDirection;
    private GameState gameState;

    [Header("Entites")]
    private EntityMap entityMap;


    [Header("Floor")]
    private GroundMap groundMap;

    [Header("World Properties")]
    public int mapWidth = 80;
    public int mapHeight = 60;
    public IntRange roomSizeRange;
    public int maxRooms = 30;

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

        var spriteLoader = SpriteLoader.instance;
        var playerSprite = spriteLoader.LoadSprite(SpriteType.Soldier_Sword);

        var fighter = new Fighter(30, 2, 5);
        var playerComponent = new Player();
        player = new Entity(new Vector3Int(startLocation.x, startLocation.y, 0), playerSprite, Color.green, player: playerComponent, fighter: fighter);

        Camera.main.transform.position = new Vector3(player.position.x, player.position.y, Camera.main.transform.position.z);

        fovSystem = new FieldOfViewSystem(groundMap);
        fovSystem.Run(new Vector2Int(player.position.x, player.position.y), 10);

        groundMap.FillRoomsWithEnemies(entityMap.GetEntities(), 3);
        entityMap.AddEntity(player);

        InitEventListeners();
        groundMap.UpdateTiles();

        gameState = GameState.Turn_Player;
    }

    // Update is called once per frame
    void Update()
    {
        ClearAll();

        PlayerMove();
        EnemyMove();

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

    void EnemyMove()
    {
        if (gameState == GameState.Turn_Enemy)
        {
            foreach (BasicMonsterAi enemy in entityMap.GetEnemies())
            {
                enemy.TakeTurn(entityMap, groundMap);
            }

            gameState = GameState.Turn_Player;
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

    void PlayerMove()
    {
        if (gameState == GameState.Turn_Player && playerNextMoveDirection != Vector2Int.zero)
        {
            (int x, int y) newPosition = (player.position.x + playerNextMoveDirection.x, player.position.y + playerNextMoveDirection.y);

            if (!groundMap.IsBlocked(newPosition.x, newPosition.y))
            {
                var target = entityMap.GetBlockingEntityAtPosition(newPosition.x, newPosition.y);

                if (target != null) { Debug.Log("You kick the " + target.name + " in the shins!"); }
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
    }
}
