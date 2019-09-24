using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{

    public Sprite sprite;
    private Tile tile;
    private Tilemap map;
    private GameEventListener moveListener;

    void Start()
    {
        map = FindObjectOfType<Tilemap>();
        transform.position = new Vector2(0, 0);
        tile = Tile.CreateInstance<Tile>();
        tile.sprite = sprite;
        map.SetTile(map.WorldToCell(transform.position), tile);
        InitEventListeners();
    }

    void Update()
    {

    }

    private void InitEventListeners(){
        moveListener = gameObject.AddComponent<GameEventListener>();
        moveListener.Event = EventManager.instance.GetGameEvent(EventManager.EventType.PlayerMove);
        moveListener.Register();
        moveListener.Response = new UnityEventWithCoords();
        moveListener.Response.AddListener(Move);
    }

    void Move(Vector2Int direction)
    {
        map.SetTile(map.WorldToCell(transform.position), null);

        transform.position = new Vector2(transform.position.x + direction.x, transform.position.y + direction.y);

        map.SetTile(map.WorldToCell(transform.position), tile);
    }
}
