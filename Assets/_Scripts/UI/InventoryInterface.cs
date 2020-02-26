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

    public bool isUse = true;
    public bool isDrop = false;

    // Start is called before the first frame update
    void Start()
    {
        canvas = gameObject.GetComponent<Canvas>();
        gameObject.SetActive(false);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.D)) { isDrop = true; isUse = false;}
        if (Input.GetKeyDown(KeyCode.U)) { isDrop = false; isUse = true;}
    }

    public void Show() {
        gameObject.SetActive(true);

        var inventoryCount = _inventory.heldItems.Count;

        for(int i = 0; i < itemSlots.Count; i++){
            if( i+1 <= inventoryCount ){
                itemSlots[i].Set(_inventory.heldItems[i]);
            }
            else {
                itemSlots[i].SetToDefault();
            }
        }

        isUse = true;
        isDrop = false;
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
        if( isUse ){
            _inventory.owner.actor.SetNextAction( new TriggerOperationsAction(_inventory.owner.actor, item.GetItem().owner ));
        }
        else if( isDrop ){
            _inventory.owner.actor.SetNextAction( new DropItemAction(_inventory.owner.actor, item.GetItem()));
        }
    }
}
