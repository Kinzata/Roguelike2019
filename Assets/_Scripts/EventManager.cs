using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    private IDictionary<EventType, GameEvent> events =
        new Dictionary<EventType, GameEvent>();
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public GameEvent GetGameEvent(EventType type)
    {
        GameEvent gameEvent;
        events.TryGetValue(type, out gameEvent);

        if (gameEvent == null)
        {
            gameEvent = ScriptableObject.CreateInstance<GameEvent>();
            events.Add(type, gameEvent);
        }

        return gameEvent;
    }

    public enum EventType
    {
        PlayerMove
    }

}
