using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class Message
{
    public string text;
    public Color color;

    public Message(string text, Color? color)
    {
        this.color = color ?? Color.white;
        this.text = text;
    }

    public SaveData SaveGameState()
    {
        return new SaveData
        {
            text = text,
            color = color.ToString(),
        };
    }

    public static Message LoadGameState(SaveData data)
    {
        ColorUtility.TryParseHtmlString(data.color, out var loadedColor);
        return new Message(data.text, loadedColor);
    }

    public class SaveData
    {
        public string text;
        public string color;
    }
}

public class MessageLog : MonoBehaviour
{
    TextMeshProUGUI textObject;
    private List<Message> messages;
    public int maxMessages = 6;

    private string preText = "<font=\"monofonto SDF\">";

    // Start is called before the first frame update
    void Start()
    {
        textObject = GetComponent<TextMeshProUGUI>();
        messages = new List<Message>();
    }

    public void AddMessage(Message message)
    {
        messages.Add(message);
        if (messages.Count > maxMessages) { messages.RemoveAt(0); }

        UpdateText();
    }

    private void UpdateText(){
        var sb = new StringBuilder();
        foreach(var msg in messages){
            var text = msg.text;
            if( msg.color != Color.white ){
                var colorText = ColorUtility.ToHtmlStringRGB(msg.color);
                text = $"<color=#{colorText}>{text}</color>";
            }
            sb.AppendLine(text);
        }

        textObject.SetText(preText + sb.ToString());
    }
}
