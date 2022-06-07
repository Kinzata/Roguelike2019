using System.Linq;
using UnityEngine;

public class WalkAction : Action
{

    private CellPosition targetPos;

    public WalkAction(Actor actor, CellPosition targetPosition) : base(actor)
    {
        this.targetPos = targetPosition;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        var result = new ActionResult();

        var isMoveSuccess = MoveTorwards(targetPos, mapData.EntityMap, mapData.GroundMap);
        if (isMoveSuccess)
        {
            result.status = ActionResultType.Success;
        }
        else
        {
            // Ok, why can't we move?
            // Is it a wall?
            var worldTile = mapData.GroundMap.GetTileAt(targetPos);
            if (worldTile != null)
            {
                // is it blocked?
                if (worldTile.blocked)
                {
                    // Ok it's a wall, let's just return normally
                    return result;
                }
            }

            // Does an entity exist?
            if (mapData.EntityMap.GetEntities(targetPos).Any())
            {
                result.NextAction = new MeleeAttackAction(actor, targetPos);
            }

            result.status = ActionResultType.Failure;
        }

        return result;
    }

    public CellPosition Move(int dx, int dy)
    {
        actor.entity.position.x += dx;
        actor.entity.position.y += dy;

        actor.entity.transform.position = actor.entity.position.ToVector3Int();

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

    public override bool UpdateHandler(MapDTO mapData)
    {
        return true;
    }
}