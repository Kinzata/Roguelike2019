using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    public delegate void OnPlayerEndTurn();
    public OnPlayerEndTurn onPlayerEndTurn;

    public delegate void OnTurnSuccess();
    public OnTurnSuccess onTurnSuccess;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void Clear()
    {
        onPlayerEndTurn = null;
        onTurnSuccess = null;
    }
}
