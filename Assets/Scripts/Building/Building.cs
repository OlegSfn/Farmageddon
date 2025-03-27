using Managers;
using ScriptableObjects.Buildings;
using UnityEngine;

namespace Building
{
    /// <summary>
    /// Base class for all building objects in the game
    /// Handles registration with the tilemap manager and basic positioning
    /// </summary>
    public class Building : MonoBehaviour
    {
        /// <summary>
        /// Configuration data for this building from ScriptableObject
        /// </summary>
        [field: SerializeField] public BuildingData buildingData { get; private set; }
        
        /// <summary>
        /// Reference to the tilemap manager for grid-based operations
        /// </summary>
        protected TilemapManager TilemapManager;
        
        /// <summary>
        /// Grid position of this building
        /// </summary>
        protected Vector2Int Position;
        
        /// <summary>
        /// Initializes the tilemap manager reference and registers the building
        /// </summary>
        protected virtual void Awake()
        {
            TilemapManager = GameManager.Instance.tilemapManager;
            TilemapManager.AddObject(this, GetPosition());
            Position = GetPosition();
        }
        
        /// <summary>
        /// Unregisters the building from the tilemap manager
        /// </summary>
        protected virtual void OnDestroy()
        {
            TilemapManager.RemoveObject(this, GetPosition());
        }

        /// <summary>
        /// Destroys this building, triggering OnDestroy
        /// </summary>
        public virtual void Die()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Converts the world position to grid coordinates
        /// </summary>
        /// <returns>Grid position as Vector2Int</returns>
        protected virtual Vector2Int GetPosition()
        {
            return new Vector2Int((int) transform.position.x, (int) transform.position.y);
        }
    }
}