public abstract class Action
{
    public Actor actor;
    public TargetData targetData;
    protected ActionResult result;

    public Action(Actor actor) {
        this.actor = actor;
        result = new ActionResult();

        if (targetData == null)
        {
            targetData = new TargetData();
        }
    }

    public Action(Actor actor, TargetData targetData): this(actor)
    {
        this.targetData = targetData;
    }

    public void SetActionResult(ActionResult result){
        this.result = result;
    }

    public abstract ActionResult PerformAction(MapDTO mapData);

    public abstract bool UpdateHandler(MapDTO mapData);
}