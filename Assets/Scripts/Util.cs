using System.Linq;
using UnityEngine;

public static class Util
{
    public static Vector3[] Vector2sToVector3s(Vector2[] vector2s) {
        return vector2s.Select(v => (Vector3)v).ToArray();
    }
}