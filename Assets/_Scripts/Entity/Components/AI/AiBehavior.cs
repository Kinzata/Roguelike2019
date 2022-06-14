public abstract class AiBehavior
{
    public AiComponent aiComponent;
    public abstract string name { get; }

    public Entity owner => aiComponent.owner;

    public SwitchToBehaviorDelegate switchTo;
    public SwitchFromBehaviorDelegate switchFrom;

    public abstract Action GetAction(MapDTO mapDto);

    public abstract object SaveGameState();
}

public delegate void SwitchToBehaviorDelegate(Entity entity, ActionResult result);
public delegate void SwitchFromBehaviorDelegate(Entity entity, ActionResult result);
