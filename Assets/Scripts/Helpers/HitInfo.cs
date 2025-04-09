using UnityEngine;

namespace Helpers
{
    /// <summary>
    /// Contains information about a hit or damage event
    /// Used to transfer damage data between attacking and receiving entities
    /// </summary>
    public struct HitInfo
    {
        /// <summary>
        /// Amount of damage inflicted by this hit
        /// </summary>
        public readonly int Damage;
    
        /// <summary>
        /// World position where the hit occurred
        /// Used for effects and directional damage calculations
        /// </summary>
        public readonly Vector2 HitPoint;

        /// <summary>
        /// Creates a new hit information package
        /// </summary>
        /// <param name="damage">Amount of damage</param>
        /// <param name="hitPoint">Position where the hit occurred</param>
        public HitInfo(int damage, Vector2 hitPoint)
        {
            Damage = damage;
            HitPoint = hitPoint;
        }
    }
}