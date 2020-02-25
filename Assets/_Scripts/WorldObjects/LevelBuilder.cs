

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelBuilder
{
    public GroundMap map;

    public GroundMap MakeMap(int maxRooms, IntRange roomSizeRange, int mapWidth, int mapHeight, Tilemap tileMap)
    {
        map = ScriptableObject.CreateInstance<GroundMap>().Init(mapWidth, mapHeight, tileMap);

        var rooms = MakeRooms(maxRooms, roomSizeRange, mapWidth, mapHeight, tileMap);
        map.rooms = rooms.ToList();
        map.UpdateNavigationMasks();
        Debug.Log("Rooms: " + rooms.Count());
        return map;
    }

    public CellPosition GetStartPosition()
    {
        return map.rooms.First().center;
    }

    private IList<Room> MakeRooms(int maxRooms, IntRange roomSizeRange, int mapWidth, int mapHeight, Tilemap tileMap)
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
            var newRoom = new Room(rect, map);

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

        return rooms;
    }

    private Room CreateRoom(Rect rect)
    {
        return new Room(rect, map).BuildRoom();
    }

    private void CreateHorizontalTunnel(int x1, int x2, int y)
    {
        int min = Mathf.Min(x1, x2);
        int max = Mathf.Max(x1, x2);
        for (int x = min; x <= max; x++)
        {
            map.SetTileToFloor(x, y);
        }
    }

    private void CreateVerticalTunnel(int y1, int y2, int x)
    {
        int min = Mathf.Min(y1, y2);
        int max = Mathf.Max(y1, y2);
        for (int y = min; y <= max; y++)
        {
            map.SetTileToFloor(x, y);
        }
    }

    public IList<Entity> FillRoomsWithEntityActors(IList<Entity> entities, int maxMonstersPerRoom, int maxItemsPerRoom)
    {
        IList<Entity> newEntities = new List<Entity>();
        foreach (Room room in map.rooms)
        {
            newEntities = FillRoomWithEnemies(newEntities, room, maxMonstersPerRoom);
        }

        return newEntities;
    }

    public IList<Entity> FillRoomsWithPassiveEntities(IList<Entity> entities, int maxMonstersPerRoom, int maxItemsPerRoom)
    {
        IList<Entity> newEntities = new List<Entity>();
        foreach (Room room in map.rooms)
        {
            newEntities = FillRoomWithItems(newEntities, room, maxItemsPerRoom);
        }

        return newEntities;
    }

    private IList<Entity> FillRoomWithEnemies(IList<Entity> entities, Room room, int maxMonsters)
    {
        var numMonsters = Random.Range(0, maxMonsters + 1);

        foreach (int i in 0.To(numMonsters))
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

    private Entity GenerateEnemy(CellPosition position)
    {
        SpriteType spriteType;
        Color color;
        string name;
        (int maxHp, int defense, int offense) figherValues;


        if (Random.value <= .8f)
        {
            // Generate Orc
            spriteType = SpriteType.Monster_Orc;
            color = new Color(.8f, 0, 0, 1f);
            name = "orc";
            figherValues = (10, 0, 3);
        }
        else
        {
            // Generate Troll
            spriteType = SpriteType.Monster_Troll;
            color = new Color(.8f, 0, 0, 1f);
            name = "troll";
            figherValues = (16, 1, 4);
        }

        var entity = Entity.CreateEntity().Init(
            position,
            spriteType,
            color,
            blocks: true,
            name: name
        );

        entity.gameObject.AddComponent<Fighter>().Init(figherValues.maxHp, figherValues.defense, figherValues.offense).owner = entity;
        entity.gameObject.AddComponent<BasicMonsterAi>().owner = entity;
        return entity;
    }

    private IList<Entity> FillRoomWithItems(IList<Entity> entities, Room room, int maxItems)
    {
        var numMonsters = Random.Range(0, maxItems + 1);

        foreach (int i in 0.To(numMonsters))
        {
            var position = room.GetRandomLocation();

            var entityExistsAtPosition = entities
                                            .Where(e => e.position.x == position.x && e.position.y == position.y)
                                            .Select(e => e)
                                            .Any();

            if (!entityExistsAtPosition)
            {
                var entity = GenerateItem(position);
                entities.Add(entity);
            }
        }

        return entities;
    }

    private Entity GenerateItem(CellPosition position)
    {
        SpriteType spriteType;
        Color color;
        string name;
        bool isItem = false;

        // if (Random.value <= 1f)
        // {
        // Generate Potion
        spriteType = SpriteType.Item_Potion_Full;
        color = new Color32(63, 191, 191, 255);
        name = "potion";
        isItem = true;
        // }

        var entity = Entity.CreateEntity().Init(
            position,
            spriteType,
            color,
            blocks: false,
            name: name
        );

        if( isItem ){
            var item = entity.gameObject.AddComponent<Item>();
            item.owner = entity;

            // This is temp, will eventually be loaded from an item file or something
            item.Operations.Add(new ModifyHealthOperation(new IntRange{min = 8, max = 12} ));
        }

        return entity;
    }

}