using System.Linq;

public class PickupItemAction : Action
{
    public PickupItemAction(Actor actor) : base(actor)
    {
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        // validate components
        var inventoryComponent = actor.entity.gameObject.GetComponent<Inventory>();

        if (inventoryComponent != null)
        {
            var targets = mapData.EntityFloorMap.GetEntities(actor.entity.position).Where(e => e.gameObject.GetComponent<Item>() != null).Select(e => e.gameObject.GetComponent<Item>());
            if (targets.Count() == 0)
            {
                // Nothing there!
                result.AppendMessage(new Message($"There is nothing on the ground.", null));
            }
            else
            {
                var item = targets.FirstOrDefault();
                var action = new AddItemToInventoryAction(actor, item);
                result.NextAction = action;
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