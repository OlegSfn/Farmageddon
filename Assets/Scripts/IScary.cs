using UnityEngine;

/// <summary>
/// Interface for objects that can apply scare effects to other entities
/// </summary>
public interface IScary
{
    /// <summary>
    /// Gets the transform of the scary object for position checks
    /// </summary>
    /// <returns>Transform component of the scary object</returns>
    public Transform GetTransform();
}