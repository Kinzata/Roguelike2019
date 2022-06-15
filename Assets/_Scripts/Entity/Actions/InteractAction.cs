using System.Linq;

public class InteractAction : Action
{
    public InteractAction(Actor actor, TargetData targetData) : base(actor, targetData)
    {
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        var targetEntity = targetData.GetTargets(mapData, 
                            (t) => t.gameObject.GetComponent<ObjectComponent>() != null,
                            includeGround: true
                            ).FirstOrDefault();

        var obj = targetEntity.GetComponent<ObjectComponent>();

        return obj.InteractWith(actor.entity);
    }


}
