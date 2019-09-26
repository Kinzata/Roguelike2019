using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private Entity player;
    public Sprite playerSprite;
    private GameEventListener moveListener;
    private Tilemap entityMap;

    void Start()
    {
        entityMap = GameObject.Find(TileMapType.EntityMap.Name()).GetComponent<Tilemap>();

        player = new Entity(Vector3Int.zero, playerSprite, Color.green, entityMap);
        InitEventListeners();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitEventListeners()
    {
        moveListener = gameObject.AddComponent<GameEventListener>();
        moveListener.Event = EventManager.instance.GetGameEvent(EventManager.EventType.PlayerMove);
        moveListener.Register();
        moveListener.Response = new UnityEventWithCoords();
        moveListener.Response.AddListener(Move);
    }

    void Move(Vector2Int direction)
    {
        player.Move(direction.x, direction.y);
    }
}
