
using System.Collections.Generic;

public class Inventory : EntityComponent
{
    public int capacity = 0;
    public List<Item> heldItems = new List<Item>();

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
}