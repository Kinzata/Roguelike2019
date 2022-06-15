using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelBuilder
{
    public GroundMap groundMap;
    public EntityMap entityMap;
    public EntityMap passiveEntityMap;
    public MiscMap miscMap;
    public List<Actor> actors;

    public void GenerateMap(LevelDataScriptableObject data, System.Random ranGen)
    {
        MakeMap(data, ranGen);

        var entityTileMap = GameObject.Find(TileMapType.EntityMap.Name()).GetComponent<Tilemap>();
        entityMap = new EntityMap().Init(entityTileMap, groundMap);

        var entityBackgroundTileMap = GameObject.Find(TileMapType.EntityMap_Background.Name()).GetComponent<Tilemap>();
        passiveEntityMap = new EntityMap().Init(entityBackgroundTileMap, groundMap);
        
        MakeMiscMap(data);

        PlaceStairsDown(data, ranGen);
    }

    public void GenerateEntities(LevelDataScriptableObject data, System.Random ranGen)
    {
        actors = new List<Actor>();

        // TODO: Seeded entities

        MakeEntities(data, entityMap);
        MakePassiveEntities(data, passiveEntityMap);
    }

    public void MakeMap(LevelDataScriptableObject data, System.Random ranGen)
    {
        var groundTileMap = GameObject.Find(TileMapType.GroundMap.Name()).GetComponent<Tilemap>();
        groundMap = new GroundMap().Init(data.mapWidth, data.mapHeight, groundTileMap);

        var rooms = MakeRooms(data.maxRooms, data.roomSizeRange, data.mapWidth, data.mapHeight, ranGen);
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

    public void MakeMiscMap(LevelDataScriptableObject data)
    {
        var miscTileMap = GameObject.Find(TileMapType.MiscMap.Name()).GetComponent<Tilemap>();
        miscMap = new MiscMap().Init(data.mapWidth, data.mapHeight, miscTileMap);
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

    public MiscMap GetMiscMap()
    {
        return miscMap;
    }

    public List<Actor> GetActors()
    {
        return actors;
    }

    private IList<Room> MakeRooms(int maxRooms, IntRange roomSizeRange, int mapWidth, int mapHeight, System.Random ranGen)
    {
        var rooms = new List<Room>();
        var counter = 0;
        var roomCounter = 0;

        while (counter++ < maxRooms)
        {
            var width = roomSizeRange.RandomValue(ranGen);
            var height = roomSizeRange.RandomValue(ranGen);

            var x = ranGen.Next(0, mapWidth - width - 1);
            var y = ranGen.Next(0, mapHeight - height - 1);

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
                if (ranGen.NextDouble() >= .5f)
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

    private void PlaceStairsDown(LevelDataScriptableObject data, System.Random ranGen)
    {
        var position = groundMap.rooms.First().GetRandomLocation(ranGen);
        // Create stairs

        var entity = Entity.CreateEntity().Init(
            position,
            SpriteType.Object_Stairs_Down,
            new Color32(122,67, 12, 255),
            blocks: false,
            name: "stairs down"
        );

        entity.gameObject.AddComponent<Stairs>().owner = entity;

        passiveEntityMap.AddEntity(entity);

        // Remove grass at position
        groundMap.GetTileAt(position).sprite = SpriteLoader.instance.LoadSprite(SpriteType.Nothing);
        groundMap.GetTileAt(position).colorLight = Color.black;
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
            var position = room.GetRandomLocation(new System.Random());

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
                var position = room.GetRandomLocation(new System.Random());

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
                var position = room.GetRandomLocation(new System.Random());

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
        var chance = Random.value;
        if (chance <= 0.5f)
        {
            return GenerateItem(position, "Potion");
        }
        else if (chance <= 0.75f)
        {
            return GenerateItem(position, "LightningScroll");
        }
        else if(chance <= 0.85f)
        {
            return GenerateItem(position, "ConfusionScroll");
        }
        else
        {
            return GenerateItem(position, "FireballScroll");
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
            case "ConfusionScroll":
                return GenerateConfusionScroll(position);
            case "FireballScroll":
                return GenerateFireballScroll(position);
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

        var item = entity.gameObject.AddComponent<Item>();
        item.owner = entity;
        item.Description = "A scroll pulsing with the power of electricity.";
        item.FlavorMessages.Add(new Message(
            "A streak of " + "lightning".ColorMe(Color.blue) + " zaps from the scroll", null
        ));
        item.range = 10;
        item.radius = 0;

        item.Operations.Add(
            new ModifyHealthOperation(
                ScriptableObject.CreateInstance<IntRange>().Init(-16, -9)
            ));


        return entity;
    }

    private Entity GenerateConfusionScroll(CellPosition position)
    {
        SpriteType spriteType;
        Color color;
        string name;

        spriteType = SpriteType.Item_Scroll_One;
        color = new Color32(160, 34, 201, 255);
        name = "confusion scroll";

        var entity = Entity.CreateEntity().Init(
            position,
            spriteType,
            color,
            blocks: false,
            name: name
        );

        var item = entity.gameObject.AddComponent<Item>();
        item.owner = entity;
        item.Description = "A scroll pulsing with the power of purple dust.";
        item.FlavorMessages.Add(new Message(
            $"A cloud of " + "purple dust".ColorMe(color) + " swirls around the head of target.", null
        ));
        item.range = 10;
        item.radius = 0;

        var behavior = new ConfusedAi(5);
        behavior.switchTo = delegate (Entity delEntity, ActionResult result)
        {
            result.AppendMessage(new Message(
                $"The {delEntity.GetColoredName()} is now {"confused".ColorMe(color)}", null
                ));
        };

        behavior.switchFrom = delegate (Entity delEntity, ActionResult result)
        {
            result.AppendMessage(new Message(
                $"The {delEntity.GetColoredName()} is no longer {"confused".ColorMe(color)}", null
                ));
        };

        item.Operations.Add(
            new ApplyAiBehavorOperation(
                behavior
            ));

        return entity;
    }

    private Entity GenerateFireballScroll(CellPosition position)
    {
        SpriteType spriteType;
        Color color;
        string name;

        spriteType = SpriteType.Item_Scroll_Two;
        color = new Color32(255, 107, 33, 255);
        name = "fireball scroll";

        var entity = Entity.CreateEntity().Init(
            position,
            spriteType,
            color,
            blocks: false,
            name: name
        );

        var item = entity.gameObject.AddComponent<Item>();
        item.owner = entity;
        item.Description = "A scroll pulsing with the power of chaotic fire.";
        item.FlavorMessages.Add(new Message(
            $"A " + "ball of fire".ColorMe(color) + " hurls out from the scroll at the target", null
        ));

        item.range = 10;
        item.radius = 1;

        item.Operations.Add(
           new ModifyHealthOperation(
               ScriptableObject.CreateInstance<IntRange>().Init(-10, -16)
           ));

        return entity;
    }

}