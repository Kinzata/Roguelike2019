public abstract class Action
{
    public Actor actor;
    public EntityMap eMap;
    public GroundMap gMap;
    protected ActionResult result;

    public Action(Actor actor, EntityMap eMap, GroundMap gMap) {
        this.actor = actor;
        this.eMap = eMap;
        this.gMap = gMap;
        result = new ActionResult();
    }

    public void SetActionResult(ActionResult result){
        this.result = result;
    }

    public abstract ActionResult PerformAction();
}