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

    public Action GetAction(EntityMap eMap, GroundMap gMap)
    {
        if (entity.GetComponent<BasicMonsterAi>() is BasicMonsterAi ai)
        {
            nextAction = ai.GetAction(eMap, gMap);
        }

        var action = nextAction;
        nextAction = null;

        return action;
    }
}