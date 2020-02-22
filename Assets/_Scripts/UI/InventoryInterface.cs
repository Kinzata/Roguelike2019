using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryInterface : MonoBehaviour
{
    public Canvas canvas;
    public List<InventoryItem> itemSlots;
    private Inventory _inventory;
    public void SetInventory(Inventory inventory){ _inventory = inventory; }

    // Start is called before the first frame update
    void Start()
    {
        canvas = gameObject.GetComponent<Canvas>();
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
        for(int i = 0; i < _inventory.heldItems.Count; i++){
            itemSlots[0].Set(_inventory.heldItems[0]);
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

        result.success = false;
        return result;
    }
}
