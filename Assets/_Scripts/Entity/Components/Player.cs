using UnityEngine;

public class Player : EntityComponent {

    void Start()
    {
        componentName = "Player";
    }

    public override object SaveGameState()
    {
        return new SaveData();
    }

    public static bool LoadGameState(Entity entity, SaveData data)
    {
        var component = entity.gameObject.AddComponent<Player>();
        component.owner = entity;
        return true;
    }

    public class SaveData
    {

    }
}