using Managers;
using UnityEngine;

/// <summary>
/// Base class for world-space cursors that interact with game objects
/// Handles cursor visualization, positioning, and interaction logic
/// </summary>
public abstract class WorldCursor : MonoBehaviour, ILogic
{
    /// <summary>
    /// Prefab used to visualize the cursor in the game world
    /// </summary>
    [SerializeField] protected GameObject cursorPrefab;

    /// <summary>
    /// Instantiated cursor game object instance
    /// </summary>
    protected GameObject CursorGameObject;
    
    /// <summary>
    /// Collider component of the cursor for physics interactions
    /// </summary>
    protected Collider2D CursorCollider;
    
    /// <summary>
    /// Filter configuration for physics interactions
    /// </summary>
    protected ContactFilter2D ContactFilter;
    
    /// <summary>
    /// Flag indicating if the cursor can currently use items
    /// </summary>
    private bool _canUseItem = true;
    
    /// <summary>
    /// Reference to the main camera for position calculations
    /// </summary>
    private Camera _mainCamera;
    
    /// <summary>
    /// Initializes cursor object and components
    /// </summary>
    protected virtual void Start()
    {
        CursorGameObject = Instantiate(cursorPrefab, GameManager.Instance.objectsPool.position, Quaternion.identity);
        CursorCollider = CursorGameObject.GetComponent<Collider2D>();
        ContactFilter = new ContactFilter2D();
        ContactFilter.NoFilter();
        ContactFilter.useTriggers = false;
        ContactFilter.useLayerMask = true;
        _mainCamera = Camera.main;
    }

    /// <summary>
    /// Handles cursor positioning and input processing
    /// </summary>
    private void Update()
    {
        if (GameManager.Instance.IsPaused)
        {
            return;
        }
        
        Vector3Int cursorPosition = GetObjectPosition();
        CursorGameObject.transform.position = cursorPosition;
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKeyDown(KeyCode.LeftAlt) && _canUseItem)
        {
            UseItem(cursorPosition);
        }
    }

    /// <summary>
    /// Checking if item can be used and updating cursor UI
    /// </summary>
    private void FixedUpdate()
    {
        _canUseItem = CheckIfCanUseItem();
        UpdateUI();
    }
    
    /// <summary>
    /// Abstract method for item interaction logic
    /// </summary>
    /// <param name="cursorPosition">Grid-aligned position of the cursor</param>
    protected abstract void UseItem(Vector3Int cursorPosition);

    /// <summary>
    /// Abstract method to check if item can be used at current position
    /// </summary>
    /// <returns>True if item use is allowed, false otherwise</returns>
    protected abstract bool CheckIfCanUseItem();

    /// <summary>
    /// Updates cursor visual feedback based on usability
    /// </summary>
    private void UpdateUI()
    {
        if (_canUseItem)
        {
            CursorGameObject.GetComponent<SpriteRenderer>().color = GameManager.Instance.goodTint;
        }
        else
        {
            CursorGameObject.GetComponent<SpriteRenderer>().color = GameManager.Instance.badTint;
        }
    }

    /// <summary>
    /// Enables or disables the cursor functionality
    /// </summary>
    /// <param name="active">Whether the cursor should be active</param>
    public virtual void SetActive(bool active)
    {
        if (active == false && CursorGameObject is not null)
        {
            CursorGameObject.transform.position = GameManager.Instance.objectsPool.position;
        }
        enabled = active;
    }
    
    /// <summary>
    /// Gets grid-aligned position from mouse coordinates
    /// </summary>
    /// <returns>Rounded integer position in world space</returns>
    protected Vector3Int GetObjectPosition()
    {
        Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y), 0);
    }

    /// <summary>
    /// Cleanup when cursor is destroyed
    /// </summary>
    private void OnDestroy()
    {
        Destroy(CursorGameObject);
    }
}
