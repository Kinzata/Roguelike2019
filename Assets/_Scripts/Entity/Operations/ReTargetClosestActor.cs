

using System.Linq;

public class ReTargetClosestActorOperation : Operation
{
    public ReTargetClosestActorOperation()
    {
        name = "ReTargetClosestActorOperation";
    }

    public override OperationResult Occur(Entity entity, MapDTO mapData, TargetData targetData)
    {
        var scriptTarget = entity;

        var requiredComponent = scriptTarget.gameObject.GetComponent<Fighter>();
        var result = new OperationResult();
        result.Success = false;

        var closestEntity = mapData.EntityMap.GetEntities()
            .Where(e => e.isVisible())
            .Where(e => e != entity)
            .Where(e => e.actor != null)
            .OrderBy( e => e.DistanceTo(entity) )
            .First();

        targetData.targetEntity = closestEntity;
        result.newTargetData = targetData;

        return result;
    }

    public override object SaveGameState()
    {
        return null;
    }
}