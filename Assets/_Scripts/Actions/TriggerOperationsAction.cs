using System.Linq;

public class TriggerOperationsAction : Action
{
    private CellPosition _targetPosition;
    private Entity _targetEntity;
    public TriggerOperationsAction(Actor actor, Entity targetEntity = null, CellPosition targetPosition = null) : base(actor)
    {
        _targetPosition = targetPosition;
        _targetEntity = targetEntity;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        // Trigger operations off a given Entity, like an item?
        if( _targetEntity != null ) {
            var itemComponent = _targetEntity.gameObject.GetComponent<Item>();
            if( itemComponent != null ){
                result.NextAction = new UseItemAction(actor, itemComponent);
                result.Success = false;
            }
        }

        return result;
    }
}