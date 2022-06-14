public class ModifyHealthOperation : Operation
{
    public IntRange ModifierRange;

    public ModifyHealthOperation(IntRange range){
        name = "ModifyHealthOperation";
        ModifierRange = range;
    }

    public override OperationResult Occur(Entity entity, MapDTO mapData, TargetData targetData)
    {
        var scriptTargets = targetData.GetTargets(mapData, (e) => e.gameObject.GetComponent<Fighter>() != null);

        // Self target requires targetData to target self

        var result = new OperationResult();
        result.Success = false;

        // Validity checks
        if ( scriptTargets.Count == 0 )
        {
            result.AppendMessage(new Message("There is nothing there.", null));
            return result;
        }

        foreach( var target in scriptTargets)
        {
            var requiredComponent = target.gameObject.GetComponent<Fighter>();
            if( requiredComponent == null ){
                result.AppendMessage(new Message("Target is invalid.", null));
                return result;
            }

            var modifierAmount = ModifierRange.RandomValue(new System.Random());

            if (modifierAmount >= 0)
            {
                var subResult = requiredComponent.Heal(modifierAmount);
                result.ActionResult.Append(subResult);
                result.AppendMessage(new Message($"{target.GetColoredName()} was healed for {modifierAmount}!", null));
            }
            else
            {
                var subResult = requiredComponent.TakeDamage(modifierAmount);
                result.ActionResult.Append(subResult);
                result.AppendMessage(new Message($"{target.GetColoredName()} was hurt for {modifierAmount}!", null));
            }
        }

        return result;
    }

    public override object SaveGameState()
    {
        return new SaveData
        {
            min = ModifierRange.min,
            max = ModifierRange.max
        };
    }

    public class SaveData
    {
        public int min;
        public int max;
    }
}