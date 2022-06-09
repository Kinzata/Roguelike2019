using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTile : Tile
{
    public int x;
    public int y;
    public bool blocked;
    public bool blockSight;
    public bool isVisible;
    public bool isExplored;

    public CellPosition position { get { return new CellPosition(x, y); }}

    public float darkTileModifier = .3f;

    public Color colorLight;

    public Navigation navMask = 0;
    

    public WorldTile Init(int x, int y, bool blocked, bool? blockSight = null)
    {
        this.x = x;
        this.y = y;
        
        // Block sight by default only if tile is blocked, otherwise use input
        this.blocked = blocked;

        if( !blockSight.HasValue ){
            blockSight = blocked;
        }

        this.blockSight = blockSight.Value;
        return this;
    }

    public Color GetColor(){
        return isVisible
            ? colorLight
            : new Color(colorLight.r * darkTileModifier, colorLight.g * darkTileModifier, colorLight.b * darkTileModifier, colorLight.a);
    }
}
