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
    private GameEventListener moveListener;
    private Tilemap entityMap;
    private Vector2Int playerNextMoveDirection;

    private IList<Entity> entities = new List<Entity>();

    void Start()
    {
        Application.targetFrameRate = 120;

        entityMap = GameObject.Find(TileMapType.EntityMap.Name()).GetComponent<Tilemap>();

        var spriteSheet = Resources.LoadAll<Sprite>("spritesheet");
        var playerSprite = spriteSheet[playerSpriteId];
        var npcSprite = spriteSheet[npcSpriteId];

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
