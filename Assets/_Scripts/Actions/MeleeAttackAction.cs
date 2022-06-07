using System.Linq;

public class MeleeAttackAction : Action
{
    public MeleeAttackAction(Actor actor, TargetData targetData) : base(actor, targetData)
    {
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        // TODO: Refactor to allow hitting multiple targets

        // validate components
        var fighterComponent = actor.entity.gameObject.GetComponent<Fighter>();
        var inMeleeDistance = actor.entity.DistanceTo(targetData.GetTargets(mapData, (e) => true).FirstOrDefault()) <= 1; // If this was an attribute could allow for 2+ tile melee weapons

        if (fighterComponent != null && inMeleeDistance)
        {
            var targets = targetData.GetTargets(mapData, (e) => e.gameObject.GetComponent<Fighter>() != null);

            result.Append(fighterComponent.Attack(targets.FirstOrDefault()));
            result.status = ActionResultType.Success;
        }
        else
        {
            result.status = ActionResultType.Failure;
        }

        return result;
    }

    public override bool UpdateHandler(MapDTO mapData)
    {
        return true;
    }
}