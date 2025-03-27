using Building;
using Enemies.Waves;
using Envinronment.DayNightCycle;
using Quests;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    /// <summary>
    /// Central manager class that coordinates game systems and provides access to shared resources
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Static reference to the singleton instance
        /// </summary>
        public static GameManager Instance;
    
        /// <summary>
        /// Reference to the player's inventory system
        /// </summary>
        public Inventory.Inventory inventory;
        
        /// <summary>
        /// Reference to the day and night cycle manager
        /// </summary>
        public DayNightManager dayNightManager;
        
        /// <summary>
        /// Reference to the cursor controller
        /// </summary>
        public Cursor cursor;
        
        /// <summary>
        /// Reference to the enemy waves manager for spawning and controlling enemies
        /// </summary>
        public EnemyWavesManager enemyWavesManager;
        
        /// <summary>
        /// Reference to the main player controller
        /// </summary>
        public PlayerContoller playerController;
        
        /// <summary>
        /// Reference to the player's health controller
        /// </summary>
        public HealthController playerHealthController;
        
        /// <summary>
        /// Reference to the cash/economy manager
        /// </summary>
        public CashManager cashManager;
        
        /// <summary>
        /// Reference to the tilemap manager for building placement
        /// </summary>
        public TilemapManager tilemapManager;
        
        /// <summary>
        /// Reference to the quest manager for tracking player progress
        /// </summary>
        public QuestManager questManager;
        
        /// <summary>
        /// Reference to the UI panels manager
        /// </summary>
        public PanelsManager panelsManager;
    
        /// <summary>
        /// Transform reference to the player for easy access
        /// </summary>
        public Transform playerTransform;
        
        /// <summary>
        /// Transform used as a parent for pooled objects
        /// </summary>
        public Transform objectsPool;
    
        /// <summary>
        /// Color used to indicate negative or invalid actions
        /// </summary>
        public Color badTint = new(255, 78, 90);
        
        /// <summary>
        /// Color used to indicate positive or valid actions
        /// </summary>
        public Color goodTint = new(117, 241, 124);
        
        /// <summary>
        /// Maximum squared distance for using items (using squared magnitude for performance)
        /// </summary>
        public float sqrDistanceToUseItems = 5f;

        /// <summary>
        /// Flag indicating whether the game is currently paused
        /// </summary>
        public bool IsPaused { get; set; }
        
        /// <summary>
        /// Reference to the game over menu UI
        /// </summary>
        [SerializeField] private GameObject gameOverMenu;

        /// <summary>
        /// Initializes the singleton instance
        /// Ensures only one GameManager exists in the scene
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    
        /// <summary>
        /// Loads a new scene by name
        /// Ensures timescale is reset to normal before scene transition
        /// </summary>
        /// <param name="sceneName">Name of the scene to load</param>
        public void LoadScene(string sceneName)
        {
            // Reset timescale in case it was modified (e.g., during pause)
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Handles game over state
        /// Pauses the game and shows the game over menu
        /// </summary>
        public void GameOver()
        {
            Time.timeScale = 0;
            gameOverMenu.SetActive(true);
        }
    }
}
