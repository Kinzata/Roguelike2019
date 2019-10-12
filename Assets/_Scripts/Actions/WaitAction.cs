public class WaitAction : Action
{
    public WaitAction(Actor actor, EntityMap eMap, GroundMap gMap) : base(actor, eMap, gMap)
    {
    }

    public override ActionResult PerformAction()
    {
        // Do Nothing
        result.success = true;

        return result;
    }
}