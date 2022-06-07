

using System.Linq;

public class ModifyHealthOperation : Operation
{
    public IntRange ModifierRange;

    public ModifyHealthOperation(IntRange range){
        ModifierRange = range;
    }

    public override OperationResult Occur(Entity entity, MapDTO mapData, TargetData targetData)
    {
        // Target and target position could be used for things like throwing something that modifies health.

        // Base entity, just affect that entity, but use a target or cell if provided
        var scriptTarget = entity;
        if (targetData.targetEntity != null) { scriptTarget = targetData.targetEntity; }
        if (targetData.targetPosition != null)
        {
            scriptTarget = targetData.GetTargets(mapData, (e) => true).FirstOrDefault();
        }

        var result = new OperationResult();
        result.Success = false;

        // Validity checks
        if ( scriptTarget == null)
        {
            result.AppendMessage(new Message("There is nothing there.", null));
            return result;
        }

        var requiredComponent = scriptTarget.gameObject.GetComponent<Fighter>();
        if( requiredComponent == null ){
            result.AppendMessage(new Message("Target is invalid.", null));
            return result;
        }

        var modifierAmount = ModifierRange.RandomValue();

        if( modifierAmount >= 0 ) {
            result.ActionResult = requiredComponent.Heal(modifierAmount);
            result.AppendMessage(new Message($"{scriptTarget.GetColoredName()} was healed for {modifierAmount}!", null));
        }
        else {
            result.ActionResult = requiredComponent.TakeDamage(modifierAmount);
            result.AppendMessage(new Message($"{scriptTarget.GetColoredName()} was hurt for {modifierAmount}!", null));
        }

        return result;
    }
}