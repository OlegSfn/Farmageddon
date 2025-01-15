using Managers;
using UnityEngine;

public abstract class WorldCursor : MonoBehaviour, ILogic
{
    
    [SerializeField] protected GameObject cursorPrefab;

    protected GameObject CursorGameObject;
    protected Collider2D CursorCollider;
    protected ContactFilter2D ContactFilter;
    
    private bool _canUseItem = true;
    private Camera _mainCamera;
    
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

    private void Update()
    {
        Vector3Int cursorPosition = GetObjectPosition();
        CursorGameObject.transform.position = cursorPosition;
        
        if (Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKeyDown(KeyCode.LeftAlt) && _canUseItem)
        {
            UseItem(cursorPosition);
        }
    }

    private void FixedUpdate()
    {
        _canUseItem = CheckIfCanUseItem();
        UpdateUI();
    }
    
    protected abstract void UseItem(Vector3Int cursorPosition);

    protected abstract bool CheckIfCanUseItem();

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

    public virtual void SetActive(bool active)
    {
        if (active == false && CursorGameObject is not null)
        {
            CursorGameObject.transform.position = GameManager.Instance.objectsPool.position;
        }
        enabled = active;
    }
    
    private Vector3Int GetObjectPosition()
    {
        Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y), 0);
    }

    private void OnDestroy()
    {
        Destroy(CursorGameObject);
    }
}
