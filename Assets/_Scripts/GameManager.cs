using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private Entity player;
    public int playerSpriteId = 27;
    public int npcSpriteId = 28;
    public int wallSpriteId = 417;
    private Sprite wallSprite;
    public int floorSpriteId = 60;
    private Sprite floorSprite;
    private GameEventListener moveListener;
    private Tilemap entityMap;
    private Vector2Int playerNextMoveDirection;

    private IList<Entity> entities = new List<Entity>();
    private Tilemap groundMap;
    private GroundMap groundMapObject;

    [Header("World Properties")]
    public int mapWidth = 80;
    public int mapHeight = 45;

    void Start()
    {
        Application.targetFrameRate = 120;

        entityMap = GameObject.Find(TileMapType.EntityMap.Name()).GetComponent<Tilemap>();
        groundMap = GameObject.Find(TileMapType.GroundMap.Name()).GetComponent<Tilemap>();
        groundMapObject = ScriptableObject.CreateInstance<GroundMap>().Init(mapWidth, mapHeight);

        var spriteSheet = Resources.LoadAll<Sprite>("spritesheet");
        var playerSprite = spriteSheet[playerSpriteId];
        var npcSprite = spriteSheet[npcSpriteId];
        wallSprite = spriteSheet[wallSpriteId];
        floorSprite = spriteSheet[floorSpriteId];

        player = new Entity(Vector3Int.zero, playerSprite, Color.green);
        var npc = new Entity(new Vector3Int(2, 2, 0), npcSprite, Color.yellow);

        entities.Add(npc);
        entities.Add(player);

        InitEventListeners();
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
        // There is no reason to do this each frame.  Once the tile is set, it is set.
        for(int x = 0; x < groundMapObject.width; x++){
            for(int y = 0; y < groundMapObject.height; y++){
                var tile = groundMapObject.tiles[x,y];
                if( tile.blockSight ){
                    tile.sprite = wallSprite;
                    tile.color = Color.gray;
                    groundMap.SetTile(new Vector3Int(x,y,0), tile);
                }
                else{
                    tile.sprite = floorSprite;
                    tile.color = new Color(0.250f, 0.466f, 0.270f ,1);
                    groundMap.SetTile(new Vector3Int(x,y,0), tile);
                }
            }
        }
        foreach (var entity in entities)
        {
            DrawEntity(entity);
        }
    }

    void DrawEntity(Entity entity)
    {
        entityMap.SetTile(entity.position, entity.tile);
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
            player.Move(playerNextMoveDirection.x, playerNextMoveDirection.y);
            playerNextMoveDirection = Vector2Int.zero;
        }
    }
}
