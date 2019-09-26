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
    public Tile tile;

    public Entity(Vector3Int position, Sprite sprite, Color color)
    {
        this.position = position;
        this.sprite = sprite;
        this.color = color;
        
        tile = Tile.CreateInstance<Tile>();
        tile.sprite = sprite;
        tile.color = color;
    }

    public Vector3Int Move(int dx, int dy)
    {        
        position.x += dx;
        position.y += dy;

        return position;
    }
}
