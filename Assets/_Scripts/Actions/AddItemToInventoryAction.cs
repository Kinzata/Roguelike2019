

public class AddItemToInventoryAction : Action
{
    private Item item;

    public AddItemToInventoryAction(Actor actor, Item item) : base(actor)
    {
        this.item = item;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        // validate components
        var inventoryComponent = actor.entity.gameObject.GetComponent<Inventory>();

        if (inventoryComponent != null)
        {
            if( inventoryComponent.HasRoom() )
            {
                // Well this is interesting... Do we delete the game object?  Disable it?  Store the entire Entity?
                // For now, lets just disable it.  Maybe if we drop an item we want to enable it again and teleport to where it was dropped
                item.owner.gameObject.SetActive(false);
                item.owner.position = new CellPosition(100000,100000); // Teleport item to far away square.  So it can't be picked up while disabled!

                var addItemResult = inventoryComponent.AddItem(item);
                result.AppendMessage(new Message($"{actor.entity.GetColoredName()} picks up {item.owner.GetColoredName()}.", null));
                result.Append(addItemResult);
                result.Success = true;
            }
            else {
                // Inventory is full
                result.AppendMessage(new Message($"{actor.entity.GetColoredName()} cannot hold any more items.", null));
            }
        }
        else
        {
            result.AppendMessage(new Message($"{actor.entity.GetColoredName()} can not carry things.", null));
        }

        return result;
    }
}