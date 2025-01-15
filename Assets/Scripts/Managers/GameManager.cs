using Enemies.Waves;
using Envinronment;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
    
        public Inventory.Inventory inventory;
        public DayNightManager dayNightManager;
        public Cursor cursor;
        public EnemyWavesManager enemyWavesManager;
        public PlayerContoller playerContoller;
    
        public Transform playerTransform;
        public Transform objectsPool;
    
        public Color badTint = new(255, 78, 90);
        public Color goodTint = new(117, 241, 124);
        public float sqrDistanceToUseItems = 5f;

        public int money;

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
    
    
        public void SetPause(bool isPaused)
        {
            Time.timeScale = isPaused ? 0 : 1;
        }
    }
}
