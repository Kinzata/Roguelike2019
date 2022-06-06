using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInterface : MonoBehaviour
{
    public Canvas canvas;
    public List<InventoryItem> itemSlots;
    private Inventory _inventory;
    public GameObject InventoryItemPrefab;
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
        itemSlots = new List<InventoryItem>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.D)) { isDrop = true; isUse = false;}
        if (Input.GetKeyDown(KeyCode.U)) { isDrop = false; isUse = true;}
    }

    public void Show() {
        gameObject.SetActive(true);

        var inventoryCount = _inventory.heldItems.Count;
        var itemSlotCount = itemSlots.Count;

        var diffSlots = inventoryCount - itemSlotCount;

        if( diffSlots > 0 )
        {
            AddItemSlots(diffSlots);
        }

        for( int i = 0; i < itemSlots.Count; i++)
        {
            var slot = itemSlots[i];
            var item = _inventory.GetItem(i);
            if( item != null)
            {
                slot.Set(item);
            }
            else
            {
                slot.SetToDefault();
            }

            var rect = slot.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, i * -60f);

            slot.button.onClick.AddListener( delegate { UseItem(slot); } );
        }

        isUse = true;
        isDrop = false;
    }

    private void AddItemSlots(int diffSlots)
    {
        for (int i = 0; i < diffSlots; i++)
        {
            var itemSlot = Instantiate(InventoryItemPrefab).GetComponent<InventoryItem>();
            itemSlot.transform.position = Vector3.zero;
            itemSlot.transform.SetParent(canvas.transform);
            itemSlots.Add(itemSlot);

            var rect = itemSlot.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(.25f, .75f);
            rect.anchorMax = new Vector2(.25f, .75f);
            rect.localScale = Vector3.one;
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
        if( isUse ){
            _inventory.owner.actor.SetNextAction( new TriggerOperationsAction(_inventory.owner.actor, item.GetItem().owner ));
        }
        else if( isDrop ){
            _inventory.owner.actor.SetNextAction( new DropItemAction(_inventory.owner.actor, item.GetItem()));
        }
    }
}
