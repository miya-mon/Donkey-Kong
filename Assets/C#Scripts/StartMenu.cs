// Assets/Scripts/StartMenu.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void OnStartButton()
    {
        if (GameController.Instance != null)
            GameController.Instance.StartGame();
        else
            SceneManager.LoadScene("Stage1");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
