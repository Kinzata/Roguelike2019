
public abstract class Operation {

    public abstract OperationResult Occur(Entity entity, MapDTO mapData, Entity target = null, CellPosition targetPosition = null);

}