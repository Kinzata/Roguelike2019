using UnityEngine;

public class WalkAction : Action {

    private CellPosition targetPos;

    public WalkAction(Actor actor, EntityMap eMap, GroundMap gMap, CellPosition targetPosition) : base(actor, eMap, gMap){
        this.targetPos = targetPosition;
    }

    public override ActionResult PerformAction(){
        var result = new ActionResult();

        var isMoveSuccess = MoveTorwards(targetPos, eMap, gMap);
        if( isMoveSuccess ) {
            result.success = true;
        }
        else {
            result.success = false;
            // figure out why or return new Action
        }

        return result;
    }

    public CellPosition Move(int dx, int dy)
    {
        actor.entity.position.x += dx;
        actor.entity.position.y += dy;

        return actor.entity.position;
    }

    public bool MoveTorwards(CellPosition target, EntityMap map, GroundMap groundMap)
    {
        var dx = target.x - actor.entity.position.x;
        var dy = target.y - actor.entity.position.y;
        var distance = (int)Mathf.Sqrt(dx * dx + dy * dy);

        dx = dx / distance;
        dy = dy / distance;

        var newX = actor.entity.position.x + dx;
        var newY = actor.entity.position.y + dy;

        if (!groundMap.IsBlocked(newX, newY) && map.GetBlockingEntityAtPosition(newX, newY) == null)
        {
            Move(dx, dy);
            return true;
        }

        return false;
    }
}