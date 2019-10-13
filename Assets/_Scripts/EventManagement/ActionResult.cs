using System.Collections.Generic;
using System.Linq;

public class ActionResult
{
    private List<Message> messages;
    private List<KeyValuePair<string, Entity>> entityEvents;
    public bool success = false;


    public ActionResult()
    {
        messages = new List<Message>();
        entityEvents = new List<KeyValuePair<string, Entity>>();
    }

    public void Append(ActionResult result)
    {
        messages.AddRange(result.GetMessages());
        foreach (var key in result.GetEntityEventKeys())
        {
            entityEvents.AddRange(result.GetEntityEvent(key).Select(e => new KeyValuePair<string, Entity>(key, e)));
        }
    }

    public void AppendMessage(Message message)
    {
        messages.Add(message);
    }

    public List<Message> GetMessages()
    {
        return messages;
    }

    public void AppendEntityEvent(string name, Entity entity)
    {
        entityEvents.Add(new KeyValuePair<string, Entity>(name, entity));
    }

    public Entity[] GetEntityEvent(string name)
    {
        return entityEvents.Where(e => e.Key == name).Select(e => e.Value).ToArray();
    }

    public string[] GetEntityEventKeys()
    {
        return entityEvents.Select(e => e.Key).ToArray();
    }

}