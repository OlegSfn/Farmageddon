using UnityEngine;

namespace Items
{
    public class Sword : MonoBehaviour, ILogic
    {
        public void SetActive(bool active)
        {
            enabled = active;
        }
    }
}
