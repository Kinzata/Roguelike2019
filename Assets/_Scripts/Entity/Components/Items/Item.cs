using System.Collections.Generic;
using System.Linq;

public class Item : EntityComponent
{
    public int Charges = 1;

    public List<Operation> Operations;
    public string Description = "";
    public List<Message> FlavorMessages = new List<Message>();

    public int range = 0;
    /// <summary>
    /// Radius is tiles beyond the tile targeted.  So a radius of 1 would target every tile surrounding the targeted tile
    /// </summary>
    public int radius = 0;

    void Start()
    {
        componentName = "Item";
    }

    public Item(){
        Operations = new List<Operation>();
    }

    /**
    * Returns True if there are charges remaining
    */
    public bool Use(){
        Charges--;

        return Charges > 0;
    }

    public override string ToString() {
        return $"{owner.GetColoredName()}";
    }

    public override object SaveGameState()
    {
        return new SaveData
        {
            range = range,
            radius = radius,
            operations = Operations.ToDictionary(o => o.name, o => o.SaveGameState()),
            description = Description,
            flavorMessages = FlavorMessages.Select(m => m.SaveGameState()).ToList()
        };
    }

    public static bool LoadGameState(Entity entity, SaveData data)
    {
        var component = entity.gameObject.AddComponent<Item>();
        component.owner = entity;

        component.Description = data.description;
        component.range = data.range;
        component.radius = data.radius;

        // Load operations
        foreach (var kvp in data.operations)
        {
            OperationLoader.LoadOperation(kvp.Key, component, kvp.Value);
        }

        component.FlavorMessages = data.flavorMessages.Select(m => Message.LoadGameState(m)).ToList();

        return true;
    }

    public class SaveData
    {
        public string description;
        public int range;
        public int radius;
        public Dictionary<string, object> operations;
        public List<Message.SaveData> flavorMessages;
    }
}