using System.Linq;

public class DropItemAction : Action
{
    private Item _item;

    public DropItemAction(Actor actor, Item item) : base(actor)
    {
        _item = item;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        // validate components
        var inventoryComponent = actor.entity.gameObject.GetComponent<Inventory>();

        if (inventoryComponent != null)
        {
            _item.owner.gameObject.SetActive(true);
            _item.owner.SetPosition(actor.entity.position.Clone());

            mapData.EntityFloorMap.AddEntity(_item.owner);

            inventoryComponent.RemoveItem(_item);

            result.AppendMessage(new Message($"{actor.entity.GetColoredName()} drops {_item.owner.GetColoredName()}.", null));
            result.status = ActionResultType.Success;
        }

        result.TransitionToStateOnSuccess = GameState.Global_LevelScene;
        
        return result;
    }

    public override bool UpdateHandler(MapDTO mapData)
    {
        return true;
    }
}