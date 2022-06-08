using System.Linq;
using UnityEngine;

public class UseItemAction : Action
{
    private Item _item;

    public UseItemAction(Actor actor, Item item) : base(actor)
    {
        _item = item;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        foreach (var operation in _item.Operations)
        {
            var operationResult = operation.Occur(actor.entity, mapData, targetData);

            SetTargetsFromOperationResult(operationResult);

            result.Append(operationResult.ActionResult);
            result.AppendMessages(_item.FlavorMessages);
            result.AppendMessages(operationResult.GetMessages());
        }

        var consumed = !_item.Use();

        if (consumed)
        {
            // Is there an inventory on the actor?
            var actorInventory = actor.entity.gameObject.GetComponent<Inventory>();
            if (actorInventory != null)
            {
                actorInventory.RemoveItem(_item);
            }

            mapData.EntityFloorMap.RemoveEntity(_item.owner);
            GameObject.Destroy(_item.gameObject);
        }

        result.status = ActionResultType.Success;
        result.TransitionToStateOnSuccess = GameState.Global_LevelScene;

        return result;
    }

    private void SetTargetsFromOperationResult(OperationResult result)
    {
        if (result.newTargetData != null)
        {
            targetData = result.newTargetData;
        }
    }

    public override bool UpdateHandler(MapDTO mapData)
    {
        return true;
    }
}