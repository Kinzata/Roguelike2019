public abstract class AiBehavior
{
    public AiComponent aiComponent;

    public Entity owner => aiComponent.owner;

    public SwitchToBehaviorDelegate switchTo;
    public SwitchFromBehaviorDelegate switchFrom;

    public abstract Action GetAction(MapDTO mapDto);
}

public delegate void SwitchToBehaviorDelegate(Entity entity, ActionResult result);
public delegate void SwitchFromBehaviorDelegate(Entity entity, ActionResult result);
