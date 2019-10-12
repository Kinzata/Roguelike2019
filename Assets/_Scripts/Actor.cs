public class Actor
{
    public Entity entity;

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

    public Action GetAction(EntityMap eMap, GroundMap gMap)
    {

        Action action;
        if (entity.enemy && entity.aiComponent != null)
        {
            var ai = entity.aiComponent;
            action = ai.GetAction(eMap, gMap);
        }
        else {
            // If we're not an enemy or don't have an aiController... might as well wait?  We need a player check here, will come later
            action = new WaitAction(this, eMap, gMap);
        }
        return action;
    }
}