public abstract class Action
{
    public Actor actor;
    protected ActionResult result;

    public Action(Actor actor) {
        this.actor = actor;
        result = new ActionResult();
    }

    public void SetActionResult(ActionResult result){
        this.result = result;
    }

    public abstract ActionResult PerformAction(MapDTO mapData);
}