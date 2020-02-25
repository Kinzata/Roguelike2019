using System.Linq;

public class UseItemAction : Action
{
    private CellPosition _targetPosition;
    private Entity _targetEntity;
    private Item _item;

    public UseItemAction(Actor actor, Item item, Entity targetEntity = null, CellPosition targetPosition = null) : base(actor)
    {
        _targetPosition = targetPosition;
        _targetEntity = targetEntity;
        _item = item;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        foreach( var operation in _item.Operations ){
            var operationResult = operation.Occur(actor.entity, mapData, _targetEntity, _targetPosition);
            result.Append(operationResult);
        }

        result.Success = true;
        result.TransitionToStateOnSuccess = GameState.Global_LevelScene;
        
        return result;
    }
}