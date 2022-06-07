using System.Linq;

public class MeleeAttackAction : Action
{
    private CellPosition targetPos;
    public MeleeAttackAction(Actor actor, CellPosition target) : base(actor)
    {
        this.targetPos = target;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        // validate components
        var fighterComponent = actor.entity.gameObject.GetComponent<Fighter>();
        var inMeleeDistance = actor.entity.DistanceTo(targetPos) <= 1; // If this was an attribute could allow for 2+ tile melee weapons

        if (fighterComponent != null && inMeleeDistance)
        {
            var targets = mapData.EntityMap.GetEntities(targetPos).Where(e => e.gameObject.GetComponent<Fighter>() != null);

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