using System.Collections.Generic;
using System.Linq;

public class OperationResult
{
    private List<Message> messages;
    public bool Success = false;
    public TargetData newTargetData;

    // I need to find a good way to merge these... right now they feel like they should be two separate
    // things, but I'm not sure that is the case.  
    public ActionResult ActionResult;

    public OperationResult()
    {
        messages = new List<Message>();
        ActionResult = new ActionResult();
    }

    public void AppendMessage(Message message)
    {
        messages.Add(message);
    }

    public List<Message> GetMessages()
    {
        return messages;
    }
}