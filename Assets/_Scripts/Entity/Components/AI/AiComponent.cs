using System.Collections.Generic;
using System.Linq;

public class AiComponent : EntityComponent
{
    public AiBehavior behavior;
    public AiBehavior previousBehavior;

    void Start()
    {
        componentName = "AiComponent";    
    }

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

    public override object SaveGameState()
    {
        return new SaveData
        {
            behavior = new Dictionary<string, object>() { { behavior.name, behavior.SaveGameState() } },
            previousBehavior = new Dictionary<string, object>() { { previousBehavior?.name ?? "None", previousBehavior?.SaveGameState() } }
        };
    }

    public static bool LoadGameState(Entity entity, SaveData data)
    {
        var component = entity.gameObject.AddComponent<AiComponent>();
        component.owner = entity;

        var kvp = data.behavior.First();
        component.behavior = AiBehaviorLoader.LoadAiBehavior(kvp.Key, null, kvp.Value);
        component.behavior.aiComponent = component;

        kvp = data.previousBehavior.First();
        component.previousBehavior = AiBehaviorLoader.LoadAiBehavior(kvp.Key, null, kvp.Value);
        if (component.previousBehavior != null)
        {
            component.previousBehavior.aiComponent = component;
        }

        return true;
    }

    public class SaveData
    {
        public Dictionary<string,object> behavior;
        public Dictionary<string, object> previousBehavior;
    }
}
