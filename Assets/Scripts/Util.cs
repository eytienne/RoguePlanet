using System;
using System.Linq;
using UnityEngine;

namespace Utils
{
    public static class Util
    {
        public static Vector3[] Vector2sToVector3s(Vector2[] vector2s) {
            return vector2s.Select(v => (Vector3)v).ToArray();
        }
    }

    public class CGizmos : MonoBehaviour
    {
        static Mesh planeMesh;

        static CGizmos() {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
            MeshFilter meshFilter = go.GetComponent<MeshFilter>();
            planeMesh = meshFilter.sharedMesh;
        }

        public static void DrawPlane(Vector3? position = null, Quaternion? rotation = null, Vector3? scale = null) {
            Gizmos.DrawMesh(planeMesh, position ?? Vector3.zero, rotation ?? Quaternion.identity, scale ?? Vector3.one);
        }

        public static void DrawPlane(Plane? plane = null, Vector3? position = null, Vector3? scale = null) {
            if (plane is Plane _plane) {
                DrawPlane(position, Quaternion.FromToRotation(Vector3.up, _plane.normal), scale);
            } else {
                DrawPlane(null as Vector3?, null, scale);
            }
        }
    }
}