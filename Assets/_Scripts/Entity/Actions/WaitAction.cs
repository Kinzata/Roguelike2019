public class WaitAction : Action
{
    public WaitAction(Actor actor) : base(actor)
    {
    }

    public override ActionResult PerformAction(MapDTO mapData)
    {
        // Do Nothing
        result.status = ActionResultType.Success;

        return result;
    }

    public override bool UpdateHandler(MapDTO mapData)
    {
        return true;
    }
}