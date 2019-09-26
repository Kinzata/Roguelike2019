using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
The Entity is just that, an entity.

It has a position, sprite, color and knows how to update it's position
 */
public class Entity
{
    public Vector3Int position;
    public Sprite sprite;
    public Color color;

    private Tile tile;
    private Tilemap entityMap;

    public Entity(Vector3Int position, Sprite sprite, Color color, Tilemap entityMap)
    {
        this.position = position;
        this.sprite = sprite;
        this.color = color;
        
        tile = Tile.CreateInstance<Tile>();
        tile.sprite = sprite;
        tile.color = color;

        this.entityMap = entityMap;

        this.entityMap.SetTile(position, tile);
    }

    public Vector3Int Move(int dx, int dy)
    {
        entityMap.SetTile(entityMap.WorldToCell(position), null);
        
        position.x += dx;
        position.y += dy;

        entityMap.SetTile(entityMap.WorldToCell(position), tile);

        return position;
    }
}
