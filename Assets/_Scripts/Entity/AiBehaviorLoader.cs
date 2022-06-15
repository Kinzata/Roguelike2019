using Newtonsoft.Json;

public static class AiBehaviorLoader
{
    public static AiBehavior LoadAiBehavior(string name, Entity entity, object behaviorData)
    {
        var data = behaviorData?.ToString();

        switch (name)
        {
            case "BasicMonsterAi":
                return new BasicMonsterAi();
            case "ConfusedAi":
                return ConfusedAi.LoadGameState(entity, JsonConvert.DeserializeObject<ConfusedAi.SaveData>(data));
            default: return null;
        }
    }
}


