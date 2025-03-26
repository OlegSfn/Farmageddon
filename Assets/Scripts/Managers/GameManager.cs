using Building;
using Enemies.Waves;
using Envinronment.DayNightCycle;
using Quests;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
    
        public Inventory.Inventory inventory;
        public DayNightManager dayNightManager;
        public Cursor cursor;
        public EnemyWavesManager enemyWavesManager;
        public PlayerContoller playerController;
        public HealthController playerHealthController;
        public CashManager cashManager;
        public TilemapManager tilemapManager;
        public QuestManager questManager;
        public PanelsManager panelsManager;
    
        public Transform playerTransform;
        public Transform objectsPool;
    
        public Color badTint = new(255, 78, 90);
        public Color goodTint = new(117, 241, 124);
        public float sqrDistanceToUseItems = 5f;

        public bool IsPaused { get; set; }
        
        [SerializeField] private GameObject gameOverMenu;

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
    
    
        public void LoadScene(string sceneName)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
        }

        public void GameOver()
        {
            Time.timeScale = 0;
            gameOverMenu.SetActive(true);
        }
    }
}
