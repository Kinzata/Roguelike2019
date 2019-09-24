using System;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent Event;
    public UnityEventWithCoords Response;

    public void Register(){
        Event.RegisterListener(this);
    }

    public void Unregister(){
        Event.UnregisterListener(this);
    }

    public void OnEventRaised(Vector2Int coords){
        Response.Invoke(coords);
    }
}

public class UnityEventWithCoords : UnityEvent<Vector2Int>{}
