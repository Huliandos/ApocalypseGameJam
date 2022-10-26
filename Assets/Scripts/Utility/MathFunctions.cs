using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utility functions accessable by the whole projects
/// </summary>
public static class MathFunctions
{
    //Generate random pos in circle
    public static Vector3 RandomPositionInCircle(Vector3 circleCenter, float radius)
    {
        float lengthInRadius = radius * Mathf.Sqrt(Random.Range(0f, 1));
        float theta = Random.Range(0f, 1) * 2 * Mathf.PI;
        return new Vector3(circleCenter.x + lengthInRadius * Mathf.Cos(theta), 0, circleCenter.z + lengthInRadius * Mathf.Sin(theta));
    }
}
