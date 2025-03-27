using Managers;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Controls the game's pause menu functionality
    /// Handles pausing/unpausing the game and displaying the pause UI
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        /// <summary>
        /// Reference to the pause menu UI GameObject
        /// </summary>
        [SerializeField] private GameObject pauseMenu;

        /// <summary>
        /// Reference to the game manager for pause state control
        /// </summary>
        private GameManager gameManager;
        
        /// <summary>
        /// Minimum delay required between closing and reopening panels
        /// Prevents accidental double-activation of the menu
        /// </summary>
        private const float OpenCloseDelay = 0.001f;

        /// <summary>
        /// Initialize references
        /// </summary>
        private void Start()
        {
            gameManager = GameManager.Instance;
        }

        /// <summary>
        /// Check for pause input and handle pause/unpause logic
        /// </summary>
        private void Update()
        {
            PanelsManager panelsManager = GameManager.Instance.panelsManager;
            
            bool canOpen = panelsManager.ActivePanelsCount == 0 &&
                           Time.time - panelsManager.lastTimeClosed >= OpenCloseDelay;
                           
            if (Input.GetKeyDown(KeyCode.Escape) && canOpen)
            {
                Time.timeScale = 0;
                gameManager.IsPaused = true;
                
                gameManager.panelsManager.OpenPanel(pauseMenu, ClosePauseMenu);
            }
        }
        
        /// <summary>
        /// Unpause the game and close the pause menu
        /// Called when the pause menu is closed
        /// </summary>
        private void ClosePauseMenu()
        {
            Time.timeScale = 1;
            GameManager.Instance.IsPaused = false;
            
            pauseMenu.SetActive(false);
        }
    }
}
