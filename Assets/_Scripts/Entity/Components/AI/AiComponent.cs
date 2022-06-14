using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

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

    public class SaveData
    {
        public Dictionary<string,object> behavior;
        public Dictionary<string, object> previousBehavior;
    }
}
