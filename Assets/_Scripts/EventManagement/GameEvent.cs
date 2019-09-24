using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> listeners =
        new List<GameEventListener>();

    public void Raise(Vector2Int coords){
        foreach( var listener in listeners ){
            listener.OnEventRaised(coords);
        }
    }

    public void RegisterListener(GameEventListener listener){
        listeners.Add(listener);
    }

      public void UnregisterListener(GameEventListener listener){
        listeners.Remove(listener);
    }
}
