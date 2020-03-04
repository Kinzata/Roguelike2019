

using System.Linq;

public class ReTargetClosestActorOperation : Operation
{
    public override OperationResult Occur(Entity entity, MapDTO mapData, Entity target = null, CellPosition targetPosition = null)
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

        result.NewTargetEntity = closestEntity;

        return result;
    }
}