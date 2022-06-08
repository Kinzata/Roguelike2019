using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundMap : ScriptableObject
{
    public int width;
    public int height;
    public Tilemap map;

    public WorldTile[,] tiles;
    public List<Room> rooms;
    public Sprite wallSprite;

    public GroundMap Init(int width, int height, Tilemap map)
    {
        this.width = width;
        this.height = height;
        this.map = map;

        wallSprite = SpriteLoader.instance.LoadSprite(SpriteType.Wall_Stone);

        InitializeTiles();
        return this;
    }

    public void UpdateTiles()
    {
        map.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = tiles[x, y];
                if (!tile.isExplored) { continue; }
                tile.color = tile.GetColor();

                map.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    public WorldTile GetTileAt(CellPosition pos){
        if( isTileValid(pos.x, pos.y) ){
            return tiles[pos.x,pos.y];
        }
        return null;
    }

    public void ClearVisibility()
    {
        foreach (WorldTile tile in tiles)
        {
            tile.isVisible = false;
        }
    }

    public bool IsBlocked(int x, int y)
    {
        if (tiles[x, y].blocked)
        {
            return true;
        }

        return false;
    }

    public bool isTileVisible(CellPosition pos)
    {
        if( isTileValid(pos.x,pos.y) ){
            return tiles[pos.x, pos.y].isVisible;
        }
        else {
            return false;
        }
    }
    public bool isTileValid(int x, int y)
    {
        return x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1);
    }

    public WorldTile GetTileInDirection(Navigation direction, (int x, int y) position)
    {
        var x = 0; var y = 0;
        switch (direction)
        {
            case Navigation.N:
                { x = position.x; y = position.y + 1; break; }
            case Navigation.NE:
                { x = position.x + 1; y = position.y + 1; break; }
            case Navigation.E:
                { x = position.x + 1; y = position.y; break; }
            case Navigation.SE:
                { x = position.x + 1; y = position.y - 1; break; }
            case Navigation.S:
                { x = position.x; y = position.y - 1; break; }
            case Navigation.SW:
                { x = position.x - 1; y = position.y - 1; break; }
            case Navigation.W:
                { x = position.x - 1; y = position.y; break; }
            default:
                { x = position.x - 1; y = position.y + 1; break; }
        }
        if (isTileValid(x, y))
        {
            return tiles[x, y];
        }
        else
        {
            return null;
        }
    }

    public List<WorldTile> GetTraversableNeighbors(int x, int y)
    {
        var tile = tiles[x, y];
        var neighbors = new List<WorldTile>();
        if ((tile.navMask & Navigation.N) == Navigation.N)
        {
            neighbors.Add(tiles[x, y + 1]);
        }
        if ((tile.navMask & Navigation.NE) == Navigation.NE)
        {
            neighbors.Add(tiles[x + 1, y + 1]);
        }
        if ((tile.navMask & Navigation.E) == Navigation.E)
        {
            neighbors.Add(tiles[x + 1, y]);
        }
        if ((tile.navMask & Navigation.SE) == Navigation.SE)
        {
            neighbors.Add(tiles[x + 1, y - 1]);
        }
        if ((tile.navMask & Navigation.S) == Navigation.S)
        {
            neighbors.Add(tiles[x, y - 1]);
        }
        if ((tile.navMask & Navigation.SW) == Navigation.SW)
        {
            neighbors.Add(tiles[x - 1, y - 1]);
        }
        if ((tile.navMask & Navigation.W) == Navigation.W)
        {
            neighbors.Add(tiles[x - 1, y]);
        }
        if ((tile.navMask & Navigation.NW) == Navigation.NW)
        {
            neighbors.Add(tiles[x - 1, y + 1]);
        }
        return neighbors;
    }

    public List<WorldTile> GetTraversableNeighbors(CellPosition pos)
    {
        return GetTraversableNeighbors(pos.x, pos.y);
    }



    void InitializeTiles()
    {
        tiles = new WorldTile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = ScriptableObject.CreateInstance<WorldTile>().Init(x, y, true);
                tile.sprite = wallSprite;
                tile.colorLight = Color.gray;
                tiles[x, y] = tile;
            }
        }
    }

    public void SetTileToFloor(int x, int y)
    {
        var tile = tiles[x,y];
        tile.sprite = SpriteLoader.instance.LoadSprite(SpriteType.Floor_Grass);
        tile.colorLight = new Color(0.250f, 0.466f, 0.270f, .5f);
        tile.blocked = false;
        tile.blockSight = false;
    }

    public void UpdateNavigationMasks()
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                var tile = tiles[x, y];

                // If tile is a wall, we don't care... yet
                if (tile.blocked) { continue; }

                // N
                if (isTileValid(x, y + 1) && !tiles[x, y + 1].blocked)
                { tile.navMask = tile.navMask | Navigation.N; }
                // NE
                if (isTileValid(x + 1, y + 1) && !tiles[x + 1, y + 1].blocked)
                { tile.navMask = tile.navMask | Navigation.NE; }
                // E 
                if (isTileValid(x + 1, y) && !tiles[x + 1, y].blocked)
                { tile.navMask = tile.navMask | Navigation.E; }
                // SE 
                if (isTileValid(x + 1, y - 1) && !tiles[x + 1, y - 1].blocked)
                { tile.navMask = tile.navMask | Navigation.SE; }
                // S
                if (isTileValid(x, y - 1) && !tiles[x, y - 1].blocked)
                { tile.navMask = tile.navMask | Navigation.S; }
                // SW
                if (isTileValid(x - 1, y - 1) && !tiles[x - 1, y - 1].blocked)
                { tile.navMask = tile.navMask | Navigation.SW; }
                // W
                if (isTileValid(x - 1, y) && !tiles[x - 1, y].blocked)
                { tile.navMask = tile.navMask | Navigation.W; }
                // NW
                if (isTileValid(x - 1, y + 1) && !tiles[x - 1, y + 1].blocked)
                { tile.navMask = tile.navMask | Navigation.NW; }
            }
        }
    }
}
