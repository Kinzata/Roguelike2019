using System.Linq;
using UnityEngine;

public class TriggerOperationsAction : Action
{
    private CellPosition _targetPosition;
    private Entity _targetEntity;
    public TriggerOperationsAction(Actor actor, Entity targetEntity = null, CellPosition targetPosition = null) : base(actor)
    {
        _targetPosition = targetPosition;
        _targetEntity = targetEntity;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        // Trigger operations off a given Entity, like an item?
        if( _targetEntity != null ) {
            var itemComponent = _targetEntity.gameObject.GetComponent<Item>();
            if (itemComponent != null)
            {
                var itemAction = new UseItemAction(actor, itemComponent);
                if ( itemComponent is RangedItem )
                {
                    result.NextAction = new RangedTargetAction(actor, itemAction);
                    result.status = ActionResultType.TurnDeferred;
                    result.TransitionToStateOnSuccess = GameState.Global_TargetSelect;

                    result.AppendMessage(new Message($"Pick a <color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>target</color>...", null));
                }
                else
                {
                    result.NextAction = itemAction;
                    result.status = ActionResultType.Continue;
                }
            }
            
        }

        return result;
    }
}