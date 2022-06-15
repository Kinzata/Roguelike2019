

using System.Collections.Generic;
using System.Linq;

public class ApplyAiBehavorOperation : Operation
{
    public AiBehavior behaviorToApply;
    public List<Message> flavorMessages = new List<Message>();

    public ApplyAiBehavorOperation(AiBehavior behavior)
    {
        name = "ApplyAiBehavorOperation";
        behaviorToApply = behavior;
    }

    public override OperationResult Occur(Entity entity, MapDTO mapData, TargetData targetData)
    {
        // Target and target position could be used for things like throwing something that modifies health.

        // Base entity, just affect that entity, but use a target or cell if provided
        var scriptTarget = entity;
        if( targetData.targetEntity != null ){ scriptTarget = targetData.targetEntity; }
        if( targetData.targetPosition != null ) {
            scriptTarget = targetData.GetTargets(mapData, (e) => true).FirstOrDefault();
        }

        var result = new OperationResult();
        result.Success = false;

        // Validity checks
        if ( scriptTarget == null )
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

    public override object SaveGameState()
    {
        return new SaveData
        {
            behaviorToApply = new Dictionary<string, object>() { { behaviorToApply.name, behaviorToApply.SaveGameState() } },
            messages = flavorMessages.Select(m => m.SaveGameState()).ToList(),
        };
    }

    public static bool LoadGameState(Item item, SaveData data)
    {
        var operation = new ApplyAiBehavorOperation(new BasicMonsterAi());

        operation.flavorMessages = data.messages.Select(m => Message.LoadGameState(m)).ToList();
        var kvp = data.behaviorToApply.First();
        operation.behaviorToApply = AiBehaviorLoader.LoadAiBehavior(kvp.Key, null, kvp.Value);
        item.Operations.Add(operation);

        return true;
    }

    public class SaveData
    {
        public Dictionary<string, object> behaviorToApply;
        public List<Message.SaveData> messages;
    }
}