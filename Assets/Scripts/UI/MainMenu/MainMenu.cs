using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public void EnterScene(string sceneName)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
        }
    
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}
