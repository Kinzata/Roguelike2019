using System.Linq;
using UnityEngine;

public class TriggerOperationsAction : Action
{
    private Entity _triggeringEntity;
    public TriggerOperationsAction(Actor actor, TargetData targetData, Entity triggeringEntity) : base(actor, targetData)
    {
        _triggeringEntity = triggeringEntity;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        // Trigger operations off a given Entity, like an item?
        if(_triggeringEntity != null ) {
            var itemComponent = _triggeringEntity.gameObject.GetComponent<Item>();
            if (itemComponent != null)
            {
                var itemAction = new UseItemAction(actor, itemComponent);
                if ( itemComponent.range > 0 )
                {
                    result.NextAction = new RangedTargetAction(actor, itemAction);
                    result.status = ActionResultType.TurnDeferred;
                    result.TransitionToStateOnSuccess = GameState.Global_ActionHandlerDeferred;
                    result.NextAction.targetData = new TargetData
                    {
                        range = itemComponent.range,
                        radius = itemComponent.radius
                    };

                    result.AppendMessage(new Message($"Pick a <color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>target</color>...", null));
                }
                else
                {
                    result.NextAction = itemAction;
                    result.status = ActionResultType.Continue;
                    result.NextAction.targetData = new TargetData
                    {
                        range = 0,
                        radius = 0,
                        targetEntity = actor.entity
                    };
                }
            }
            
        }

        return result;
    }

    public override bool UpdateHandler(MapDTO mapData)
    {
        return true;
    }
}