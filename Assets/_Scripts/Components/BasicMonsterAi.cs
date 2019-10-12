using UnityEngine;

public class BasicMonsterAi : EntityComponent
{
    public Action GetAction(EntityMap entityMap, GroundMap groundMap)
    {
        Action action;
        var aStar = new AStar(groundMap, entityMap);
        var target = entityMap.GetPlayer();
        var actionResult = new ActionResult();

        if (target == null)
        {
            // If no target, the pathfinding doesn't know what to do, so wait instead and send a message up
            action = BuildWaitAction(actionResult, entityMap, groundMap);
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
                    action = BuildWaitAction(actionResult, entityMap, groundMap);
                }
                else
                {
                    action = new WalkAction(owner.actor, entityMap, groundMap, new CellPosition(moveTile.x, moveTile.y));
                    action.SetActionResult(actionResult);
                }
            }
            else
            {
                // If we're in melee range, attack instead of move
                if (owner.fighterComponent == null)
                {
                    // Can't fight though... ha
                    action = BuildWaitAction(actionResult, entityMap, groundMap);
                }
                else
                {
                    // TODO: we'll come back here, need a fight action... getting walk done first
                    // actionResult.Append(owner.fighterComponent.Attack(target));
                    action = BuildWaitAction(actionResult, entityMap, groundMap);
                }
            }
        }
        else
        {
            // Nothing to do, not visible, just wait
            action = BuildWaitAction(actionResult, entityMap, groundMap, logMessage: false);
        }

        return action;
    }

    private WaitAction BuildWaitAction(ActionResult actionResult, EntityMap entityMap, GroundMap groundMap, bool logMessage = true)
    {
        if (logMessage)
        {
            actionResult.AppendMessage(new Message($"The {owner.name} is confused.  What should it do?", null));
        }
        var action = new WaitAction(owner.actor, entityMap, groundMap);
        action.SetActionResult(actionResult);
        return action;
    }
}