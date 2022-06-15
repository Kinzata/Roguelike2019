using Newtonsoft.Json;

public static class OperationLoader
{
    public static bool LoadOperation(string name, Item item, object operationData)
    {
        var data = operationData.ToString();

        switch (name)
        {
            case "ModifyHealthOperation":
                return ModifyHealthOperation.LoadGameState(item, JsonConvert.DeserializeObject<ModifyHealthOperation.SaveData>(data));
            case "ReTargetClosesActorOperation":
                return ReTargetClosestActorOperation.LoadGameState(item, operationData);
            case "ApplyAiBehavorOperation":
                return ApplyAiBehavorOperation.LoadGameState(item, JsonConvert.DeserializeObject<ApplyAiBehavorOperation.SaveData>(data));
            default: return false;
        }
    }
}


