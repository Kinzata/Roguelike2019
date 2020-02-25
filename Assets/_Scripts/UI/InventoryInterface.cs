using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryInterface : MonoBehaviour
{
    public Canvas canvas;
    public List<InventoryItem> itemSlots;
    private Inventory _inventory;
    public void SetInventory(Inventory inventory){ _inventory = inventory; }

    public EntityMap entityMap;
    public GroundMap groundMap;

    // Start is called before the first frame update
    void Start()
    {
        canvas = gameObject.GetComponent<Canvas>();
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
        for(int i = 0; i < _inventory.heldItems.Count; i++){
            itemSlots[i].Set(_inventory.heldItems[i]);
        }
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

    public ActionResult DescribeInventory() {
        var result = new ActionResult();

        result.AppendMessage(new Message("Your bag holds...", null));

        foreach(var item in _inventory.heldItems){
             result.AppendMessage(new Message(item.ToString(), null));
        }

        result.Success = false;
        return result;
    }

    public void UseItem(InventoryItem item){
        _inventory.owner.actor.SetNextAction( new TriggerOperationsAction(_inventory.owner.actor, item.GetItem().owner ));

        // TODO: Create list of actions, action.  One for triggering the operations, one for consuming an item use, one for removing item from inventory, etc...
        // Remove item from inventory UI if fully consumed
        var fullyConsumed = _inventory.ConsumeItemUse(item.GetItem());
        if( fullyConsumed ) { item.Set(null); }
    }
}
