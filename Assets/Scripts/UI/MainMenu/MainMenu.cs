using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
    /// <summary>
    /// Handles main menu navigation and scene transitions
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        /// <summary>
        /// Loads a specified scene and resumes game time
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        public void EnterScene(string sceneName)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
        }
    
        /// <summary>
        /// Closes the game application
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}