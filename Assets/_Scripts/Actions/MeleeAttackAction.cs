using System.Linq;

public class MeleeAttackAction : Action
{
    private CellPosition targetPos;
    public MeleeAttackAction(Actor actor, EntityMap eMap, GroundMap gMap, CellPosition target) : base(actor, eMap, gMap)
    {
        this.targetPos = target;
    }

    public override ActionResult PerformAction()
    {
        // validate components
        var fighterComponent = actor.entity.gameObject.GetComponent<Fighter>();
        var inMeleeDistance = actor.entity.DistanceTo(targetPos) <= 1; // If this was an attribute could allow for 2+ tile melee weapons

        if( fighterComponent != null && inMeleeDistance ){
            var targets = eMap.GetEntities(targetPos).Where(e => e.gameObject.GetComponent<Fighter>() != null);

            result.Append(fighterComponent.Attack(targets.FirstOrDefault()));
            result.success = true;
        }
        else {
            result.success = false;
        }

        return result;
    }
}