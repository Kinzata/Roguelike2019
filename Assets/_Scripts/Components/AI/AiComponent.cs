public class AiComponent : EntityComponent
{
    public AiBehavior behavior;
    public AiBehavior previousBehavior;

    public void AssignBehavior(AiBehavior behavior)
    {
        behavior.aiComponent = this;
        previousBehavior = this.behavior;
        this.behavior = behavior;
    }

    public Action GetAction(MapDTO mapDto)
    {
        if( behavior == null) { return null; }
        return behavior.GetAction(mapDto);
    }
}
