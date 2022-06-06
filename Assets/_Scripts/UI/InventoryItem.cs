using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    private Item _item;
    public Image image;
    public Button button;
    public TextMeshProUGUI labelText;
    public KeyCode key;
    public TextMeshProUGUI keyText;

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponentInChildren<Image>();
        button = GetComponentInChildren<Button>();
        labelText = transform.Find("ItemLabel").GetComponent<TextMeshProUGUI>();
        keyText = transform.Find("KeyLabel").GetComponent<TextMeshProUGUI>();

        SetToDefault();
    }

    public void Set(Item item, KeyCode key){
        _item = item;

        image.sprite = item.owner.sprite;
        image.color = item.owner.color;
        labelText.SetText(item.owner.GetColoredName());
        this.key = key;
        keyText.SetText(key.ToString().ToLower());
    }

    public void SetToDefault(){
        image.color = new Color(1,1,1,0);
        labelText.SetText("");
        keyText.SetText("");
        key = KeyCode.None;
    }

    public Item GetItem(){
        return _item;
    }
}
