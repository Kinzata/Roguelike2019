using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            var gameEvent = EventManager.instance.GetGameEvent(EventManager.EventType.PlayerMove);
            Vector2Int direction = Vector2Int.zero;
            // Cardinals
            if (Input.GetKeyDown(KeyCode.Keypad4))
                direction.x = -1;
            if (Input.GetKeyDown(KeyCode.Keypad6))
                direction.x = 1;
            if (Input.GetKeyDown(KeyCode.Keypad2))
                direction.y = -1;
            if (Input.GetKeyDown(KeyCode.Keypad8))
                direction.y = 1;

            // Diagonals
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                direction.x = -1; direction.y = 1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                direction.x = -1; direction.y = -1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                direction.x = 1; direction.y = -1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                direction.x = 1; direction.y = 1;
            }

            if( direction.x == 0 && direction.y == 0 ){
                return;
            }

            gameEvent.Raise(direction);
        }
    }
}
