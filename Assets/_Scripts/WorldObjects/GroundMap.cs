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

    // TODO: Spritesheet singleton
    public int wallSpriteId = 553;
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

    public bool isTileVisible(int x, int y)
    {
        return tiles[x, y].isVisible;
    }
    public bool isTileValid(int x, int y)
    {
        return x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1);
    }

    public Vector2Int MakeMap(int maxRooms, IntRange roomSizeRange, int mapWidth, int mapHeight)
    {
        rooms = new List<Room>();
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

    public IList<Entity> FillRoomsWithEnemies(IList<Entity> entities, int maxMonstersPerRoom)
    {

        foreach (Room room in rooms)
        {
            entities = FillRoomWithEnemies(entities, room, maxMonstersPerRoom);
        }

        return entities;
    }

    public IList<Entity> FillRoomWithEnemies(IList<Entity> entities, Room room, int maxMonsters)
    {
        var numMonsters = Random.Range(0, maxMonsters + 1);

        foreach (int i in 1.To(numMonsters))
        {
            var position = room.GetRandomLocation();

            var entityExistsAtPosition = entities
                                            .Where(e => e.position.x == position.x && e.position.y == position.y)
                                            .Select(e => e)
                                            .Any();

            if (!entityExistsAtPosition)
            {
                var entity = GenerateEnemy(position);
                entities.Add(entity);
            }
        }

        return entities;
    }

    public Entity GenerateEnemy((int x, int y) position)
    {
        Sprite sprite;
        Color color;
        string name;
        Fighter fighter;
        BasicMonsterAi ai;

        if (Random.value <= .8f)
        {
            // Generate Orc
            sprite = SpriteLoader.instance.LoadSprite(SpriteType.Monster_Orc);
            color = new Color(.8f, 0, 0, 1f);
            name = "orc";
            fighter = new Fighter(10, 0, 3);
            ai = new BasicMonsterAi();
        }
        else
        {
            // Generate Troll
            sprite = SpriteLoader.instance.LoadSprite(SpriteType.Monster_Troll);
            color = new Color(.8f, 0, 0, 1f);
            name = "troll";
            fighter = new Fighter(16, 1, 4);
            ai = new BasicMonsterAi();
        }

        return new Entity(
            new Vector3Int(position.x, position.y, 0),
            sprite,
            color,
            blocks: true,
            name: name,
            enemy: true,
            fighter: fighter,
            ai: ai);
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
        tile.sprite = SpriteLoader.instance.LoadSprite(SpriteType.Floor_Grass);
        tile.colorLight = new Color(0.250f, 0.466f, 0.270f, .5f);
        tile.blocked = false;
        tile.blockSight = false;
    }
}
