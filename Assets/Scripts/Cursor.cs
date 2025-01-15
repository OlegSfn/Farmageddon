using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    [field: SerializeField] public Image cursorImage { get; private set; }

    private void LateUpdate()
    {
        transform.position = Input.mousePosition;
    }
}
