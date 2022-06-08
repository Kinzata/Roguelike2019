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
        var cell = MouseUtilities.GetCellPositionAtMousePosition(mapData.GroundMap);
        mapData.MiscMap.TargetTilesInRadius(cell.x, cell.y, targetData.radius);
        mapData.MiscMap.UpdateTiles();

        if (Input.GetMouseButtonDown(0))
        {
            targetData.targetPosition = MouseUtilities.GetCellPositionAtMousePosition(mapData.GroundMap);
            mapData.MiscMap.ClearTargets();
            mapData.MiscMap.UpdateTiles();
            return true;
        }
        return false;
    }
}