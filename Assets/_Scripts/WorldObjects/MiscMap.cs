using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MiscMap : ScriptableObject
{
    public int width;
    public int height;
    public Tilemap map;

    public Sprite targetSprite;
    public List<TileWithPosition> targetedTiles;

    public MiscMap Init(int width, int height, Tilemap map)
    {
        this.width = width;
        this.height = height;
        this.map = map;

        targetSprite = SpriteLoader.instance.LoadSprite(SpriteType.Misc_Target_One);
        ClearTargets();
        return this;
    }

    public class TileWithPosition : Tile
    {
        public int x;
        public int y;
    }

    public void UpdateTiles()
    {
        map.ClearAllTiles();

        foreach(var tile in targetedTiles)
        {
            map.SetTile(new Vector3Int(tile.x, tile.y, 0), tile);
        }

        ClearTargets();
    }

    public void TargetTile(int x, int y)
    {
        var tile = CreateInstance<TileWithPosition>();
        tile.x = x;
        tile.y = y;
        tile.sprite = targetSprite;
        tile.color = new Color32(255, 255, 40, 200);
        targetedTiles.Add(tile);
    }

    public void TargetTilesInRadius(int x, int y, int radius)
    {
        for (int i = x-radius; i <= x+radius; i++)
        {
            for (int j = y-radius; j <= y+radius; j++)
            {
                TargetTile(i, j);
            }
        }
    }

    public void ClearTargets()
    {
        targetedTiles = new List<TileWithPosition>();
    }
}
