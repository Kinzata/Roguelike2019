using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class MiscMap
{
    public int width;
    public int height;
    public Tilemap map;

    public Sprite targetSprite;
    public List<TileWithPosition> targetedTiles;
    public Color color = new Color32(255, 255, 40, 200);

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

    public void TargetTile(int x, int y,  Color32 color, SpriteType sprite = SpriteType.Misc_Target_One)
    {
        var tile = ScriptableObject.CreateInstance<TileWithPosition>();
        tile.x = x;
        tile.y = y;
        tile.sprite = SpriteLoader.instance.LoadSprite(sprite);
        tile.color = color;
        targetedTiles.Add(tile);
    }

    public void TargetTile(int x, int y, SpriteType sprite = SpriteType.Misc_Target_One)
    {
        TargetTile(x, y, new Color32(255, 255, 40, 200), sprite);
    }

    public void TargetTile(int x, int y)
    {
        TargetTile(x, y, new Color32(255, 255, 40, 200));
    }

    public void TargetTilesInRadius(int x, int y, int radius, Color32 color, SpriteType sprite = SpriteType.Misc_Target_One)
    {
        for (int i = x-radius; i <= x+radius; i++)
        {
            for (int j = y-radius; j <= y+radius; j++)
            {
                TargetTile(i, j, color, sprite);
            }
        }
    }

    public void TargetTilesInRadius(int x, int y, int radius, SpriteType sprite = SpriteType.Misc_Target_One)
    {
        for (int i = x - radius; i <= x + radius; i++)
        {
            for (int j = y - radius; j <= y + radius; j++)
            {
                TargetTile(i, j, sprite);
            }
        }
    }

    public void ClearTargets()
    {
        targetedTiles = new List<TileWithPosition>();
    }
}
