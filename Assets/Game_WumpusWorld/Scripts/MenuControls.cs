using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControls : MonoBehaviour
{
    public void ButtonClick_WumpusWorld(){
        SceneManager.LoadScene("Scene_WumpusWorld_1");
    }

    public void ButtonClick_QuitGame() {
        Application.Quit();
    }

    public void ButtonClick_BackToMenu() {
        SceneManager.LoadScene("Scene_MainMenu");
    }
}
