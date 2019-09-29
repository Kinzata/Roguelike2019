using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundMap : ScriptableObject
{
    public int width;
    public int height;

    public WorldTile[,] tiles;

    // TODO: Spritesheet singleton
    public int wallSpriteId = 553;
    public Sprite wallSprite;
    public int floorSpriteId = 4;
    public Sprite floorSprite;

    public GroundMap Init(int width, int height)
    {
        this.width = width;
        this.height = height;

        var spriteSheet = Resources.LoadAll<Sprite>("spritesheet");
        wallSprite = spriteSheet[wallSpriteId];
        floorSprite = spriteSheet[floorSpriteId];

        InitializeTiles();
        return this;
    }

    public void UpdateTiles(Tilemap map)
    {
        map.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = tiles[x, y];
                tile.color = tile.GetColor();

                map.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
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

    public bool isTileValid(int x, int y)
    {
        return x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1);
    }

    public Vector2Int MakeMap(int maxRooms, IntRange roomSizeRange, int mapWidth, int mapHeight)
    {
        var rooms = new List<Room>();
        var counter = 0;
        var roomCounter = 0;
        while (counter++ < maxRooms)
        {
            var width = roomSizeRange.RandomValue();
            var height = roomSizeRange.RandomValue();

            var x = Random.Range(0, mapWidth - width - 1);
            var y = Random.Range(0, mapHeight - height - 1);

            var rect = new Rect(x, y, width, height);
            var newRoom = new Room(rect, this);

            var intersectedRoom = rooms.Where(room =>
            {
                return newRoom.Intersects(room);
            }).Select(room => room).FirstOrDefault();

            if (intersectedRoom != null) { continue; }

            newRoom.BuildRoom();

            if (rooms.Count != 0)
            {
                var prevCenter = rooms.ElementAt(roomCounter - 1).center;
                if (Random.value >= .5f)
                {
                    CreateHorizontalTunnel(prevCenter.x, newRoom.center.x, prevCenter.y);
                    CreateVerticalTunnel(prevCenter.y, newRoom.center.y, newRoom.center.x);
                }
                else
                {
                    CreateVerticalTunnel(prevCenter.y, newRoom.center.y, prevCenter.x);
                    CreateHorizontalTunnel(prevCenter.x, newRoom.center.x, newRoom.center.y);
                }
            }

            rooms.Add(newRoom);
            roomCounter++;
        }
        Debug.Log("Rooms: " + rooms.Count());
        return rooms.First().center;
    }

    private Room CreateRoom(Rect rect)
    {
        return new Room(rect, this).BuildRoom();
    }

    private void CreateHorizontalTunnel(int x1, int x2, int y)
    {
        int min = Mathf.Min(x1, x2);
        int max = Mathf.Max(x1, x2);
        for (int x = min; x <= max; x++)
        {
            SetTileToFloor(tiles[x, y]);
        }
    }

    private void CreateVerticalTunnel(int y1, int y2, int x)
    {
        int min = Mathf.Min(y1, y2);
        int max = Mathf.Max(y1, y2);
        for (int y = min; y <= max; y++)
        {
            SetTileToFloor(tiles[x, y]);
        }
    }

    void InitializeTiles()
    {
        tiles = new WorldTile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = ScriptableObject.CreateInstance<WorldTile>().Init(true);
                tile.sprite = wallSprite;
                tile.colorLight = Color.gray;
                tiles[x, y] = tile;
            }
        }
    }

    public void SetTileToFloor(WorldTile tile)
    {
        tile.sprite = floorSprite;
        tile.colorLight = new Color(0.250f, 0.466f, 0.270f, 1);
        tile.blocked = false;
        tile.blockSight = false;
    }
}
