using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtension
{
    public static Vector3 GetFlatVector(this Vector3 vector) => new(vector.x, 0, vector.z);

    public static float GetFlatMagnitude(this Vector3 vector) => vector.GetFlatVector().magnitude;
}