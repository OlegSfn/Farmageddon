using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// Controls the camera to follow a target object in 2D space
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        /// <summary>
        /// The transform that the camera will follow
        /// </summary>
        [SerializeField] private Transform target;

        /// <summary>
        /// Updates camera position after all other updates to ensure smooth following
        /// </summary>
        private void LateUpdate()
        {
            var targetPosition = target.position;
            transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
        }
    }
}