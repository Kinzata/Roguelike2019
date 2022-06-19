using UnityEngine;

public class Stairs : ObjectComponent
{
    public bool isStairsDown = true;
    public DungeonLevelNode toNode;

    void Start()
    {
        componentName = "Stairs";
    }

    public override ActionResult InteractWith(Entity interactingEntity)
    {
        Debug.Log("You found the stairs down!");
        var result = new ActionResult();
        result.AppendMessage(new Message("You found the stairs down!", null));
        result.status = ActionResultType.Success;

        result.AppendEntityEvent("transition", owner);

        return result;
    }

    public override object SaveGameState()
    {
        return new SaveData
        {
            isStairsDown = isStairsDown
        };
    }

    public static bool LoadGameState(Entity entity, SaveData data)
    {
        var component = entity.gameObject.AddComponent<Stairs>();
        component.owner = entity;
        component.isStairsDown = data.isStairsDown;

        return true;
    }

    public class SaveData
    {
        public bool isStairsDown;
    }
}
