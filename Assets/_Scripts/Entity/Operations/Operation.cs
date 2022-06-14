
public abstract class Operation {

    public string name;

    public abstract OperationResult Occur(Entity entity, MapDTO mapData, TargetData targetData);

    public abstract object SaveGameState();

}