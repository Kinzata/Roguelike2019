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
    public int floorSpriteId = 4;

    public GroundMap Init(int width, int height)
    {
        this.width = width;
        this.height = height;
        InitializeTiles();
        return this;
    }

    public void UpdateTiles(Tilemap map)
    {
        map.ClearAllTiles();

        var spriteSheet = Resources.LoadAll<Sprite>("spritesheet");
        var wallSprite = spriteSheet[wallSpriteId];
        var floorSprite = spriteSheet[floorSpriteId];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = tiles[x, y];
                if (tile.blockSight)
                {
                    tile.sprite = wallSprite;
                    tile.color = Color.gray;
                    map.SetTile(new Vector3Int(x, y, 0), tile);
                }
                else
                {
                    tile.sprite = floorSprite;
                    tile.color = new Color(0.250f, 0.466f, 0.270f, 1);
                    map.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
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

    public Vector2Int MakeMap(int maxRooms, IntRange roomSizeRange, int mapWidth, int mapHeight)
    {
        var rooms = new List<Room>();
        var counter = 0;
        var roomCounter = 0;
        while (counter++ < maxRooms)
        {
            var width = roomSizeRange.RandomValue();
            var height = roomSizeRange.RandomValue();

            var x = Random.Range(0, mapWidth - width );
            var y = Random.Range(0, mapHeight - height);

            var rect = new Rect(x, y, width, height);
            var newRoom = new Room(rect, this);

            var intersectedRoom = rooms.Select(room => {
                return room.Intersects(newRoom) ? room : null;
            }).FirstOrDefault();

            if( intersectedRoom != null ) { continue; }

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
            tiles[x, y].blocked = false;
            tiles[x, y].blockSight = false;
        }
    }

    private void CreateVerticalTunnel(int y1, int y2, int x)
    {
        int min = Mathf.Min(y1, y2);
        int max = Mathf.Max(y1, y2);
        for (int y = min; y <= max; y++)
        {
            tiles[x, y].blocked = false;
            tiles[x, y].blockSight = false;
        }
    }

    void InitializeTiles()
    {
        tiles = new WorldTile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = ScriptableObject.CreateInstance<WorldTile>().Init(true);
            }
        }

    }
}
