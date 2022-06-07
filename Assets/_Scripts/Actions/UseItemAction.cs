using System.Linq;
using UnityEngine;

public class UseItemAction : Action
{
    private Entity _targetEntity;
    private Item _item;

    public UseItemAction(Actor actor, Item item, Entity targetEntity = null) : base(actor)
    {
        _targetEntity = targetEntity;
        _item = item;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        foreach (var operation in _item.Operations)
        {
            var operationResult = operation.Occur(actor.entity, mapData, _targetEntity, targetPosition);

            SetTargetsFromOperationResult(operationResult);

            result.Append(operationResult.ActionResult);
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
        if (result.NewTargetEntity != null)
        {
            _targetEntity = result.NewTargetEntity;
        }

        if (result.NewTargetPosition != null)
        {
            targetPosition = result.NewTargetPosition;
        }
    }
}