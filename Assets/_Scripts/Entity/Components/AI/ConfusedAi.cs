using UnityEngine;

public class ConfusedAi : AiBehavior
{
    public int turnLimit;
    public override string name => "ConfusedAi";

    public ConfusedAi(int turnLimit)
    {
        this.turnLimit = turnLimit;
    }

    public override Action GetAction(MapDTO mapDto)
    {
        Action action;
        var actionResult = new ActionResult();

        var shouldWait = Random.value > 0.8f;
        if( shouldWait )
        {
            var message = new Message($"The {owner.GetColoredName()} rocks back and forth, staring at nothing.",null);
            action = BuildWaitAction(actionResult, message);
        }
        else
        {
            // Get a random direction
            var traversableTiles = mapDto.GroundMap.GetTraversableNeighbors(owner.position);
            var moveTile = traversableTiles.GetRandomItem();
            if(moveTile)
            {
                actionResult.AppendMessage(new Message($"The {owner.GetColoredName()} stumbles around.", null));
                action = new WalkAction(owner.actor, new TargetData { targetPosition = new CellPosition(moveTile.x, moveTile.y) });
                action.SetActionResult(actionResult);
            }
            else
            {
                var message = new Message($"The {owner.GetColoredName()} rocks back and forth, staring at nothing.",null);
                action = BuildWaitAction(actionResult, message);
            }
        }

        if( turnLimit == 0 )
        {
            var assignResult = aiComponent.AssignBehavior(aiComponent.previousBehavior);
            actionResult.AppendMessages(assignResult.GetMessages());
        }
        turnLimit--;

        return action;
    }

    private WaitAction BuildWaitAction(ActionResult actionResult, Message logMessage)
    {
        if (logMessage != null)
        {
            actionResult.AppendMessage(logMessage);
        }
        var action = new WaitAction(owner.actor);
        actionResult.status = ActionResultType.Success;
        action.SetActionResult(actionResult);
        return action;
    }

    public override object SaveGameState()
    {
        return new SaveData
        {
            turnLimit = turnLimit
        };
    }

    public static ConfusedAi LoadGameState(Entity entity, SaveData data)
    {
        var ai = new ConfusedAi(data.turnLimit);

        // TODO: This needs a refactor -- How to save this?  Are these defaults?
        var color = new Color32(160, 34, 201, 255);
        ai.switchTo = delegate (Entity delEntity, ActionResult result)
        {
            result.AppendMessage(new Message(
                $"The {delEntity.GetColoredName()} is now {"confused".ColorMe(color)}", null
                ));
        };

        ai.switchFrom = delegate (Entity delEntity, ActionResult result)
        {
            result.AppendMessage(new Message(
                $"The {delEntity.GetColoredName()} is no longer {"confused".ColorMe(color)}", null
                ));
        };

        return ai;
    }

    public class SaveData
    {
        public int turnLimit;
    }
}