
public abstract class Operation {

    public abstract ActionResult Occur(Entity entity, MapDTO mapData, Entity target = null, CellPosition targetPosition = null);

}