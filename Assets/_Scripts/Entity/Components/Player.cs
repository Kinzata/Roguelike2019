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

    public class SaveData
    {

    }
}