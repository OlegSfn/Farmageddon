using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "HoeData", menuName = "ScriptableObjects/HoeData")]
    public class HoeData : ScriptableObject
    {
        public float distanceMultiplier;
    }
}