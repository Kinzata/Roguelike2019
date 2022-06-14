using System.Collections.Generic;
using System.Linq;

public class Inventory : EntityComponent
{
    public int capacity = 0;
    public List<Item> heldItems = new List<Item>();

    void Start()
    {
        componentName = "Inventory";
    }

    public Inventory Init(int capacity = 2){
        this.capacity = capacity;

        return this;
    }

    public bool HasRoom() {
        return heldItems.Count < capacity;
    }

    public ActionResult AddItem(Item item){
        var result = new ActionResult();

        heldItems.Add(item);

        result.AppendMessage(new Message($"{item.owner.GetColoredName()} added to {owner.GetColoredName()}'s inventory.", null));
        return result;
    }

    public bool RemoveItem(Item item){
        var removed = heldItems.Remove(item);

        return removed;
    }

    public Item GetItem(int index)
    {
        if( heldItems.Count > index)
        {
            return heldItems[index];
        }
        else
        {
            return null;
        }
    }

    public override object SaveGameState()
    {
        return new SaveData
        {
            capacity = capacity,
            heldItems = heldItems.Select(i => i.owner.SaveGameState()).ToList()
        };
    }

    public class SaveData
    {
        public int capacity;
        public List<Entity.SaveData> heldItems;
    }
}