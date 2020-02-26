

public class ModifyHealthOperation : Operation
{
    public IntRange ModifierRange;

    public ModifyHealthOperation(IntRange range){
        ModifierRange = range;
    }

    public override ActionResult Occur(Entity entity, MapDTO mapData, Entity target = null, CellPosition targetPosition = null)
    {
        // Target and target position could be used for things like throwing something that modifies health.

        // Base entity, just affect that entity
        var scriptTarget = entity;

        var requiredComponent = scriptTarget.gameObject.GetComponent<Fighter>();
        var result = new ActionResult();
        result.Success = false;

        // Validity check
        if( requiredComponent == null ){
            result.AppendMessage(new Message("Target is invalid.", null));
            return result;
        }

        var modifierAmount = ModifierRange.RandomValue();

        if( modifierAmount >= 0 ) {
            requiredComponent.Heal(modifierAmount);
            result.AppendMessage(new Message($"{entity.GetColoredName()} was healed for {modifierAmount}!", null));
        }
        else {
            requiredComponent.TakeDamage(modifierAmount);
            result.AppendMessage(new Message($"{entity.GetColoredName()} was hurt for {modifierAmount}!", null));
        }

        return result;
    }
}