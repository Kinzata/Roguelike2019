public class Actor
{
    public Entity entity;
    private Action nextAction;

    public Actor(Entity entity)
    {
        this.entity = entity;
        this.entity.SetActor(this);
    }

    public void SetEntity(Entity entity)
    {
        this.entity = entity;
        this.entity.SetActor(this);
    }

    public void SetNextAction(Action action){
        nextAction = action;
    }

    public Action GetAction(MapDTO mapDto)
    {
        if (entity.GetComponent<AiComponent>() is AiComponent ai)
        {
            nextAction = ai.GetAction(mapDto);
        }

        var action = nextAction;
        nextAction = null;

        return action;
    }
}