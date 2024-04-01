using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    /// <param name="angleZ">(angle, z)</param>
    /// <returns>(x, y, z) based on angle -> unit circle * Track.TrackSize</returns>
    public static Vector3 GetWorldPosition(Vector2 angleZ)
    {
        float multiplier = GameProperties.TrackSize / 2F;
        return new Vector3(Mathf.Cos(angleZ.x) * multiplier, Mathf.Sin(angleZ.x) * multiplier, angleZ.y);
    }

    /// <summary>
    /// Given unit circle radian, returns Euler Vector3 to be rotated to the center
    /// </summary>
    /// <param name="angle">angle in radians</param>
    /// <returns>Euler angles</returns>
    public static Vector3 GetWorldRotation(float angle)
    {
        return Vector3.forward * Mathf.Rad2Deg * (angle - Mathf.PI / 2F);
    }
}
