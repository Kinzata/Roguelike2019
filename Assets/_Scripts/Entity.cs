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
    public WorldTile tile;
    public bool blocks;
    public string name;
    public bool enemy;
    public Fighter fighterComponent;
    public BasicMonsterAi aiComponent;

    public Entity(Vector3Int position, Sprite sprite, Color color, bool blocks = false, string name = "mysterious enemy", bool enemy = false,
        Fighter fighter = null, BasicMonsterAi ai = null)
    {
        this.position = position;
        this.sprite = sprite;
        this.color = color;
        this.blocks = blocks;
        this.name = name;
        this.enemy = enemy;
        this.fighterComponent = fighter;
        this.aiComponent = ai;
        
        tile = Tile.CreateInstance<WorldTile>();
        tile.sprite = sprite;
        tile.color = color;

        if( fighter != null ) {
            fighter.owner = this;
        }

        if( ai != null ){
            ai.owner = this;
        }
    }

    public Vector3Int Move(int dx, int dy)
    {        
        position.x += dx;
        position.y += dy;

        return position;
    }
}
