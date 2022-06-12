using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        GameSceneManager.Instance.NewGame();
    }

    public void LoadGame()
    {
        GameSceneManager.Instance.LoadGame("");
    }

    public void QuitGame()
    {
        GameSceneManager.Instance.Quit();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            NewGame();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            QuitGame();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
    }
}
