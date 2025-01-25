using UnityEngine;

public struct HitInfo
{
    public readonly int Damage;
    public readonly Vector2 HitPoint;

    public HitInfo(int damage, Vector2 hitPoint)
    {
        Damage = damage;
        HitPoint = hitPoint;
    }
}