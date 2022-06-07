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
        if( targetPosition == null )
        {
            targetPosition = actor.entity.position;
        }
        //var targetEntity = mapData.EntityMap.GetEntities(targetPosition).FirstOrDefault();
        action.targetPosition = targetPosition;
        result.NextAction = action;
        result.status = ActionResultType.Continue;

        return result;
    }
}