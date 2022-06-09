public class DeferAction : Action
{
    public Action deferredAction;

    public DeferAction(Actor actor, Action actionToDefer) : base(actor)
    {
        deferredAction = actionToDefer;
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        // Do Nothing
        result.status = ActionResultType.TurnDeferred;
        result.NextAction = deferredAction;
        result.TransitionToStateOnSuccess = GameState.Global_ActionHandlerDeferred;
        return result;
    }

    public override bool UpdateHandler(MapDTO mapData)
    {
        return true;
    }
}