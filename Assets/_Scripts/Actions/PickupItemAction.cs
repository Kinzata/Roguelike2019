using System.Linq;

public class PickupItemAction : Action
{
    public PickupItemAction(Actor actor, EntityMap eMap, GroundMap gMap) : base(actor, eMap, gMap)
    {
    }

    public override ActionResult PerformAction()
    {
        // validate components
        var inventoryComponent = actor.entity.gameObject.GetComponent<Inventory>();

        if (inventoryComponent != null)
        {
            var targets = eMap.GetEntities(actor.entity.position).Where(e => e.gameObject.GetComponent<Item>() != null).Select(e => e.gameObject.GetComponent<Item>());
            if (targets.Count() == 0)
            {
                // Nothing there!
                result.AppendMessage(new Message($"There is nothing on the ground.", null));
            }
            else
            {
                var item = targets.FirstOrDefault();
                var action = new AddItemToInventoryAction(actor, eMap, gMap, item);
                result.nextAction = action;
            }
        }
        else
        {
            result.AppendMessage(new Message($"{actor.entity.GetColoredName()} can not carry things.", null));
        }

        
        return result;
    }
}