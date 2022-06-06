using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryInterface : MonoBehaviour
{
    public Canvas canvas;
    public List<InventoryItem> itemSlots;
    private Inventory _inventory;
    public GameObject InventoryItemPrefab;
    public float itemUnitAdjustment = 60;
    public void SetInventory(Inventory inventory){ _inventory = inventory; }

    public EntityMap entityMap;
    public GroundMap groundMap;

    public bool isUse = true;
    public bool isDrop = false;

    public KeyCode[] useableKeys = new KeyCode[]
    {
        KeyCode.A,
        KeyCode.B,
        KeyCode.C,
        KeyCode.E,
        KeyCode.F,
        KeyCode.G,
        KeyCode.H,
        KeyCode.J,
        KeyCode.K,
        KeyCode.L,
        KeyCode.M,
        KeyCode.N,
        KeyCode.O,
        KeyCode.P
    };

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
                slot.Set(item, useableKeys[i]);
            }
            else
            {
                slot.SetToDefault();
            }

            var rect = slot.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, i * -itemUnitAdjustment);

            slot.button.onClick.AddListener( delegate { UseItem(slot); } );
        }

        isUse = true;
        isDrop = false;
    }

    private void AddItemSlots(int diffSlots)
    {
        for (int i = 0; i < diffSlots; i++)
        {
            var newObj = Instantiate(InventoryItemPrefab);
            var itemSlot = newObj.GetComponent<InventoryItem>();
            itemSlot.transform.position = Vector3.zero;
            itemSlot.transform.SetParent(canvas.transform);
            newObj.transform.position = Vector3.zero;
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

    /// <summary>
    /// Use the item assigned the given KeyCode
    /// </summary>
    /// <param name="key">KeyCode</param>
    public void UseItem(KeyCode key)
    {
        // Find item with key
        var itemFound = itemSlots.Select(i => i).Where(i => i.key == key).FirstOrDefault();

        if( itemFound )
        {
            UseItem(itemFound);
        }
    }

    /// <summary>
    /// Searches for an item slot in the inventory assigned to a pressable key using
    /// the Input from Unity.  If a key is pressed that is assigned to an item, the
    /// item will be used.
    /// </summary>
    public void HandleItemKeyPress()
    {
        foreach(KeyCode key in useableKeys)
        {
            if( Input.GetKeyDown(key))
            {
                UseItem(key);
            }
        }
    }
}
