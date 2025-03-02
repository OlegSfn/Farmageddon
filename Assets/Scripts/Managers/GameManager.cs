using Building;
using Enemies.Waves;
using Envinronment.DayNightCycle;
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
        public CashManager cashManager;
        public TilemapManager tilemapManager;
    
        public Transform playerTransform;
        public Transform objectsPool;
    
        public Color badTint = new(255, 78, 90);
        public Color goodTint = new(117, 241, 124);
        public float sqrDistanceToUseItems = 5f;

        [SerializeField] private GameObject pauseMenu;
        private bool _isPaused;

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
        
        public void SetPause()
        {
            _isPaused = !_isPaused;
            pauseMenu.SetActive(_isPaused);
            Time.timeScale = _isPaused ? 0 : 1;
        }
    }
}
