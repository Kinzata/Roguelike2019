using UnityEngine;

public class RangedTargetAction : Action
{
    private Action action;
    public RangedTargetAction(Actor actor, Action action) : base(actor)
    {
        this.action = action;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        if( targetData.targetPosition == null )
        {
            targetData.targetPosition = actor.entity.position;
        }

        action.targetData = targetData;
        result.NextAction = action;
        result.status = ActionResultType.Continue;

        return result;
    }

    public override bool UpdateHandler(MapDTO mapData)
    {
        if (Input.GetMouseButtonDown(0))
        {
            targetData.targetPosition = MouseUtilities.GetCellPositionAtMousePosition(mapData.GroundMap);
            return true;
        }
        return false;
    }
}