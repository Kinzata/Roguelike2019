

using System.Collections.Generic;
using System.Linq;

public class ApplyAiBehavorOperation : Operation
{
    public AiBehavior behaviorToApply;
    public List<Message> flavorMessages = new List<Message>();

    public ApplyAiBehavorOperation(AiBehavior behavior)
    {
        behaviorToApply = behavior;
    }

    public override OperationResult Occur(Entity entity, MapDTO mapData, Entity target = null, CellPosition targetPosition = null)
    {
        // Target and target position could be used for things like throwing something that modifies health.

        // Base entity, just affect that entity, but use a target or cell if provided
        var scriptTarget = entity;
        if( target != null ){ scriptTarget = target; }
        if(targetPosition != null ) {
            scriptTarget = mapData.EntityMap.GetEntities(targetPosition).FirstOrDefault();
        }

        var result = new OperationResult();
        result.Success = false;

        // Validity checks
        if ( scriptTarget == null)
        {
            result.AppendMessage(new Message("There is nothing there.", null));
            return result;
        }

        var requiredComponent = scriptTarget.gameObject.GetComponent<AiComponent>();
        if( requiredComponent == null ){
            result.AppendMessage(new Message("Target is invalid.", null));
            return result;
        }

        result.ActionResult = requiredComponent.AssignBehavior(behaviorToApply);

        return result;
    }
}