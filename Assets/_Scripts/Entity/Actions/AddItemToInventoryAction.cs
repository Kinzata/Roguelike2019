

public class AddItemToInventoryAction : Action
{
    private Item _item;

    public AddItemToInventoryAction(Actor actor, Item item) : base(actor)
    {
        _item = item;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        // validate components
        var inventoryComponent = actor.entity.gameObject.GetComponent<Inventory>();

        if (inventoryComponent != null)
        {
            if( inventoryComponent.HasRoom() )
            {
                _item.owner.gameObject.SetActive(false);
                _item.owner.position = new CellPosition(100000,100000); // Teleport item to far away square.  So it can't be picked up while disabled!

                mapData.EntityFloorMap.RemoveEntity(_item.owner);

                var addItemResult = inventoryComponent.AddItem(_item);
                result.AppendMessage(new Message($"{actor.entity.GetColoredName()} picks up {_item.owner.GetColoredName()}.", null));
                result.Append(addItemResult);
                result.status = ActionResultType.Success;
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

    public override bool UpdateHandler(MapDTO mapData)
    {
        return true;
    }
}