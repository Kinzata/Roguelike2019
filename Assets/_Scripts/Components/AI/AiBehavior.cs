public abstract class AiBehavior
{
    public AiComponent aiComponent;

    public Entity owner => aiComponent.owner;

    public abstract Action GetAction(MapDTO mapDto);
}
