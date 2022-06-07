using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelBuilder
{
    public GroundMap groundMap;
    public EntityMap entityMap;
    public EntityMap passiveEntityMap;
    public List<Actor> actors;

    public void Generate(LevelDataScriptableObject data)
    {
        actors = new List<Actor>();

        MakeMap(data);

        var entityTileMap = GameObject.Find(TileMapType.EntityMap.Name()).GetComponent<Tilemap>();
        entityMap = ScriptableObject.CreateInstance<EntityMap>().Init(entityTileMap, groundMap);

        MakeEntities(data, entityMap);

        var entityBackgroundTileMap = GameObject.Find(TileMapType.EntityMap_Background.Name()).GetComponent<Tilemap>();
        passiveEntityMap = ScriptableObject.CreateInstance<EntityMap>().Init(entityBackgroundTileMap, groundMap);

        MakePassiveEntities(data, passiveEntityMap);
    }

    public void MakeMap(LevelDataScriptableObject data)
    {
        var groundTileMap = GameObject.Find(TileMapType.GroundMap.Name()).GetComponent<Tilemap>();
        groundMap = ScriptableObject.CreateInstance<GroundMap>().Init(data.mapWidth, data.mapHeight, groundTileMap);

        var rooms = MakeRooms(data.maxRooms, data.roomSizeRange, data.mapWidth, data.mapHeight, groundTileMap);
        groundMap.rooms = rooms.ToList();
        groundMap.UpdateNavigationMasks();
        Debug.Log("Rooms: " + rooms.Count());
    }

    public void MakeEntities(LevelDataScriptableObject data, EntityMap entityMap)
    {
        // Build Enemies
        var newEntities = FillRoomsWithEntityActors(data);
        foreach (var enemy in newEntities)
        {
            actors.Add(new Actor(enemy));
            entityMap.AddEntity(enemy);
        }
    }

    public void MakePassiveEntities(LevelDataScriptableObject data, EntityMap passiveEntityMap)
    {
        var passiveEntities = FillRoomsWithPassiveEntities(data);
        foreach (var passiveEntity in passiveEntities)
        {
            passiveEntityMap.AddEntity(passiveEntity);
        }
    }

    public GroundMap GetGroundMap()
    {
        return groundMap;
    }

    public EntityMap GetEntityMap()
    {
        return entityMap;
    }

    public EntityMap GetPassiveEntityMap()
    {
        return passiveEntityMap;
    }

    public List<Actor> GetActors()
    {
        return actors;
    }

    public CellPosition GetStartPosition()
    {
        return groundMap.rooms.First().center;
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
            var newRoom = new Room(rect, groundMap);

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
        return new Room(rect, groundMap).BuildRoom();
    }

    private void CreateHorizontalTunnel(int x1, int x2, int y)
    {
        int min = Mathf.Min(x1, x2);
        int max = Mathf.Max(x1, x2);
        for (int x = min; x <= max; x++)
        {
            groundMap.SetTileToFloor(x, y);
        }
    }

    private void CreateVerticalTunnel(int y1, int y2, int x)
    {
        int min = Mathf.Min(y1, y2);
        int max = Mathf.Max(y1, y2);
        for (int y = min; y <= max; y++)
        {
            groundMap.SetTileToFloor(x, y);
        }
    }



    public IList<Entity> FillRoomsWithEntityActors(LevelDataScriptableObject data)
    {
        IList<Entity> newEntities = new List<Entity>();
        foreach (Room room in groundMap.rooms)
        {
            newEntities = FillRoomWithEnemies(newEntities, room, data);
        }

        return newEntities;
    }

    public IList<Entity> FillRoomsWithPassiveEntities(LevelDataScriptableObject data)
    {
        IList<Entity> newEntities = new List<Entity>();
        foreach (Room room in groundMap.rooms)
        {
            newEntities = FillRoomWithItems(newEntities, room, data);
        }

        return newEntities;
    }

    private IList<Entity> FillRoomWithEnemies(IList<Entity> entities, Room room, LevelDataScriptableObject data)
    {
        var numMonsters = Random.Range(data.minEnemiesInRoom, data.maxEnemiesInRoom + 1);

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
        var aiComponent = entity.gameObject.AddComponent<AiComponent>();
        aiComponent.owner = entity;
        aiComponent.AssignBehavior(new BasicMonsterAi());

        return entity;
    }

    private IList<Entity> FillRoomWithItems(IList<Entity> entities, Room room, LevelDataScriptableObject data)
    {
        var numItems = Random.Range(0, data.maxItemsInRoom + 1);

        if( data.GuaranteeItems.Count != 0)
        {
            numItems = data.GuaranteeItems.Count;
            foreach (int i in 0.To(numItems))
            {
                var position = room.GetRandomLocation();

                var entityExistsAtPosition = entities
                                                .Where(e => e.position.x == position.x && e.position.y == position.y)
                                                .Select(e => e)
                                                .Any();

                if (!entityExistsAtPosition)
                {
                    var entity = GenerateItem(position, data.GuaranteeItems[i]);
                    entities.Add(entity);
                }
            }
        }
        else
        {
            foreach (int i in 0.To(numItems))
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
        }

        return entities;
    }

    private Entity GenerateItem(CellPosition position)
    {
        if (Random.value <= 0.5f)
        {
            return GenerateItem(position, "Potion");
        }
        else {
            return GenerateItem(position, "LightningScroll");
        }
    }

    private Entity GenerateItem(CellPosition position, string name)
    {
        switch( name)
        {
            case "Potion":
                return GeneratePotion(position);
            case "LightningScroll":
                return GenerateLightningScroll(position);
            default:
                return GeneratePotion(position);
        }
    }

    private Entity GeneratePotion(CellPosition position)
    {
        SpriteType spriteType;
        Color color;
        string name;

        spriteType = SpriteType.Item_Potion_Full;
        color = new Color32(63, 191, 191, 255);
        name = "potion";

        var entity = Entity.CreateEntity().Init(
            position,
            spriteType,
            color,
            blocks: false,
            name: name
        );

        var item = entity.gameObject.AddComponent<Item>();
        item.owner = entity;

        // This is temp, will eventually be loaded from an item file or something
        item.Operations.Add(
            new ModifyHealthOperation(
                ScriptableObject.CreateInstance<IntRange>().Init(8, 12)
            ));


        return entity;

    }

    private Entity GenerateLightningScroll(CellPosition position)
    {
        SpriteType spriteType;
        Color color;
        string name;

        spriteType = SpriteType.Item_Scroll_One;
        color = new Color32(196, 250, 255, 255);
        name = "lightning scroll";

        var entity = Entity.CreateEntity().Init(
            position,
            spriteType,
            color,
            blocks: false,
            name: name
        );

        var item = entity.gameObject.AddComponent<RangedItem>();
        item.owner = entity;
        item.Description = "A scroll pulsing with the power of electricity.";
        item.FlavorMessages.Add(new Message(
            "A streak of " + "lightning".ColorMe(Color.blue) + " zaps from the scroll", null
        ));

        // This is temp, will eventually be loaded from an item file or something
        //item.Operations.Add(
        //    new ReTargetClosestActorOperation()
        //);

        item.Operations.Add(
            new ModifyHealthOperation(
                ScriptableObject.CreateInstance<IntRange>().Init(-10, -16)
            ));


        return entity;

    }

}