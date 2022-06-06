using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    private Item _item;
    public Image image;
    public Button button;
    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponentInChildren<Image>();
        button = GetComponentInChildren<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        SetToDefault();
    }

    public void Set(Item item){
        _item = item;

        image.sprite = item.owner.sprite;
        image.color = item.owner.color;
        text.SetText(item.owner.GetColoredName());
    }

    public void SetToDefault(){
        image.color = new Color(1,1,1,0);
        text.SetText("");
    }

    public Item GetItem(){
        return _item;
    }
}
