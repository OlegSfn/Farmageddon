using Managers;
using ScriptableObjects.Buildings;
using UnityEngine;

namespace Building
{
    public class Building : MonoBehaviour
    {
        [field: SerializeField] public BuildingData buildingData { get; private set; }
        
        protected TilemapManager TilemapManager;
        protected Vector2Int Position;
        
        protected virtual void Awake()
        {
            TilemapManager = GameManager.Instance.tilemapManager;
            TilemapManager.AddObject(this, GetPosition());
            Position = GetPosition();
        }
        
        protected virtual  void OnDestroy()
        {
            TilemapManager.RemoveObject(this, GetPosition());
        }

        protected virtual Vector2Int GetPosition()
        {
            return new Vector2Int((int) transform.position.x, (int) transform.position.y); 
        }
    }
}