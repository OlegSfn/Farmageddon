using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls a UI-based cursor that follows the mouse position
/// Used for custom cursor display in the game's UI
/// </summary>
public class Cursor : MonoBehaviour
{
    /// <summary>
    /// The image component used to display the cursor
    /// Can be accessed by other components but only set through inspector
    /// </summary>
    [field: SerializeField] public Image cursorImage { get; private set; }

    /// <summary>
    /// Updates the cursor position to match the mouse position
    /// Uses LateUpdate to ensure it happens after all other updates
    /// </summary>
    private void LateUpdate()
    {
        transform.position = Input.mousePosition;
    }
}
