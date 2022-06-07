public class AiComponent : EntityComponent
{
    public AiBehavior behavior;
    public AiBehavior previousBehavior;

    public ActionResult AssignBehavior(AiBehavior behavior)
    {
        var result = new ActionResult();

        if (this.behavior != null && this.behavior.switchFrom != null)
        {
            this.behavior.switchFrom(owner, result);
        }
        if (behavior != null && behavior.switchTo != null)
        {
            behavior.switchTo(owner, result);
        }

        behavior.aiComponent = this;
        previousBehavior = this.behavior;
        this.behavior = behavior;

        return result;
    }

    public Action GetAction(MapDTO mapDto)
    {
        if( behavior == null) { return null; }
        return behavior.GetAction(mapDto);
    }
}
