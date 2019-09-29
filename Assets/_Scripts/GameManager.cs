using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private Entity player;
    private GameEventListener moveListener;
    private Tilemap entityMap;
    private Vector2Int playerNextMoveDirection;

    private IList<Entity> entities = new List<Entity>();
    private Tilemap groundMap;
    private GroundMap groundMapObject;

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

        entityMap = GameObject.Find(TileMapType.EntityMap.Name()).GetComponent<Tilemap>();
        groundMap = GameObject.Find(TileMapType.GroundMap.Name()).GetComponent<Tilemap>();
        groundMapObject = ScriptableObject.CreateInstance<GroundMap>().Init(mapWidth, mapHeight);
        var startLocation = groundMapObject.MakeMap(maxRooms, roomSizeRange, mapWidth, mapHeight);

        var spriteLoader = SpriteLoader.instance;
        var playerSprite = spriteLoader.LoadSprite(SpriteType.Soldier_Sword);
        var npcSprite = spriteLoader.LoadSprite(SpriteType.Soldier_Spear);

        player = new Entity(new Vector3Int(startLocation.x, startLocation.y, 0), playerSprite, Color.green);
        var npc = new Entity(new Vector3Int(startLocation.x-1, startLocation.y-1, 0), npcSprite, Color.yellow);

        Camera.main.transform.position = new Vector3(player.position.x, player.position.y, Camera.main.transform.position.z);

        fovSystem = new FieldOfViewSystem(groundMapObject);
        fovSystem.Run(new Vector2Int(player.position.x, player.position.y), 10);

        entities.Add(npc);
        entities.Add(player);

        InitEventListeners();
        groundMapObject.UpdateTiles(groundMap);
    }

    // Update is called once per frame
    void Update()
    {
        ClearAll();

        PlayerMove();

        RenderAll();
    }

    void RenderAll()
    {
        foreach (var entity in entities)
        {
            DrawEntity(entity);
        }
    }

    void DrawEntity(Entity entity)
    {
        if( groundMapObject.isTileVisible(entity.position.x, entity.position.y)){
            entityMap.SetTile(entity.position, entity.tile);
        }
    }

    public void ClearAll()
    {
        foreach (var entity in entities)
        {
            ClearEntity(entity);
        }
    }

    void ClearEntity(Entity entity)
    {
        entityMap.SetTile(entity.position, null);
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

    void PlayerMove(){
        if( playerNextMoveDirection != Vector2Int.zero ){
            if( !groundMapObject.IsBlocked(player.position.x + playerNextMoveDirection.x, player.position.y + playerNextMoveDirection.y)){
                player.Move(playerNextMoveDirection.x, playerNextMoveDirection.y);
                Camera.main.transform.position = new Vector3(player.position.x, player.position.y, Camera.main.transform.position.z);
                fovSystem.Run(new Vector2Int(player.position.x, player.position.y), 10);
                groundMapObject.UpdateTiles(groundMap);
            }
            playerNextMoveDirection = Vector2Int.zero;
        }
    }
}
