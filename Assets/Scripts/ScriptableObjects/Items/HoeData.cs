using UnityEngine;

namespace ScriptableObjects.Items
{
    [CreateAssetMenu(fileName = "HoeData", menuName = "ScriptableObjects/HoeData")]
    public class HoeData : ScriptableObject
    {
        public float distanceMultiplier;
    }
}