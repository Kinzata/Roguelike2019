public class BasicMonsterAi : AiBehavior
{
    public override Action GetAction(MapDTO mapDto)
    {
        var groundMap = mapDto.GroundMap;
        var entityMap = mapDto.EntityMap;

        Action action;
        var aStar = new AStar(groundMap, entityMap);
        var target = entityMap.GetPlayer();
        var actionResult = new ActionResult();

        if (target == null)
        {
            // If no target, the pathfinding doesn't know what to do, so wait instead and send a message up
            action = BuildWaitAction(actionResult);
        }

        // Right now enemies will only act if the player can see them
        else if (groundMap.isTileVisible(owner.position))
        {
            // Attempt to move towards the AIs target (this doesn't have to be the player!)
            if (owner.DistanceTo(target) >= 2)
            {
                var moveTile = aStar.FindPathToTarget((owner.position.x, owner.position.y), (target.position.x, target.position.y));
                if (moveTile == null)
                {
                    // No path found I guess
                    action = BuildWaitAction(actionResult);
                }
                else
                {
                    action = new WalkAction(owner.actor, new TargetData { targetPosition = new CellPosition(moveTile.x, moveTile.y) });
                }
            }
            else
            {
                // If we're in melee range, attack instead of move
                if (owner.GetComponent<Fighter>() == null)
                {
                    // Can't fight though... ha
                    action = BuildWaitAction(actionResult);
                }
                else
                {
                    action = new MeleeAttackAction(owner.actor, new TargetData { targetEntity = target });
                }
            }
        }
        else
        {
            // Nothing to do, not visible, just wait
            action = BuildWaitAction(actionResult, logMessage: false);
        }

        return action;
    }

    private WaitAction BuildWaitAction(ActionResult actionResult, bool logMessage = true)
    {
        if (logMessage)
        {
            actionResult.AppendMessage(new Message($"The {owner.name} is confused.  What should it do?", null));
        }
        var action = new WaitAction(owner.actor);
        actionResult.status = ActionResultType.Success;
        action.SetActionResult(actionResult);
        return action;
    }
}