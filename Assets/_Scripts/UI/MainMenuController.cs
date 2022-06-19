using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        GameSceneManager.Instance.NewGame();
    }

    public void LoadGame()
    {
        var file = Application.persistentDataPath + "/roguelike_save_data.json";
        GameSceneManager.Instance.LoadGame(file);
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(RewriteRule.Generate());
        }
    }
}
