using Managers;
using UnityEngine;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;

        private GameManager gameManager;
        private const float OpenCloseDelay = 0.001f;

        private void Start()
        {
            gameManager = GameManager.Instance;
        }

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
        
        private void ClosePauseMenu()
        {
            Time.timeScale = 1;
            GameManager.Instance.IsPaused = false;
            pauseMenu.SetActive(false);
        }
    }
}
