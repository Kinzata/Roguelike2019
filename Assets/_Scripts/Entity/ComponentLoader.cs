using Newtonsoft.Json;

public static class ComponentLoader
{
    public static bool LoadComponent(string name, Entity entity, object componentData)
    {
        var data = componentData.ToString();

        switch (name)
        {
            case "Fighter":
                return Fighter.LoadGameState(entity, JsonConvert.DeserializeObject<Fighter.SaveData>(data));
            case "Player":
                return Player.LoadGameState(entity, componentData as Player.SaveData);
            case "Inventory":
                return Inventory.LoadGameState(entity, JsonConvert.DeserializeObject<Inventory.SaveData>(data));
            case "Item":
                return Item.LoadGameState(entity, JsonConvert.DeserializeObject<Item.SaveData>(data));
            case "AiComponent":
                return AiComponent.LoadGameState(entity, JsonConvert.DeserializeObject<AiComponent.SaveData>(data));
            default: return false;
        }
    }
}


