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

    public static class Extensions
    {
        public const int nbTangents = 8;
        public static Vector3 GetGroundNormal(this Transform transform, GameObject planet, Vector3 groundDirection, int terrainLayer)
        {
            Vector3[] tangents = new Vector3[nbTangents];
            Vector3 forwardShift = Vector3.Cross(groundDirection, transform.right).normalized;

            int k = 0;
            for (int i = 0; i < nbTangents; i++)
            {
                float angle = (float)i / nbTangents * 180;
                float oppositeAngle = angle + 180;

                Vector3 originA = transform.position + Quaternion.AngleAxis(angle, -groundDirection) * forwardShift;
                Vector3 originB = transform.position + Quaternion.AngleAxis(oppositeAngle, -groundDirection) * forwardShift;
                // Debug.DrawRay(originA, groundDirection, Color.cyan);
                // Debug.DrawRay(originB, groundDirection, Color.blue);

                Ray rayA = new Ray(originA, groundDirection);
                RaycastHit hitA;
                if (Physics.Raycast(rayA, out hitA, Mathf.Infinity, ~terrainLayer))
                {
                    Ray rayB = new Ray(originB, groundDirection);
                    RaycastHit hitB;
                    if (Physics.Raycast(rayB, out hitB, Mathf.Infinity, ~terrainLayer))
                    {
                        Vector3 tangent = (hitB.point - hitA.point).normalized;
                        tangents[k] = tangent;
                        k++;
                    }
                }
            }

            Vector3 groundNormal = Vector3.zero;
            for (int i = 0; i < k / 2; i++)
            {
                Vector3 tangent1 = tangents[i];
                Vector3 tangent2 = tangents[i + k / 2];
                Vector3 normal = Vector3.Cross(tangent1, tangent2).normalized;
                normal *= Mathf.Sign(Vector3.Dot(transform.up, normal));

                // Debug.DrawRay(transform.position, normal.normalized, Color.red);
                groundNormal = Vector3.LerpUnclamped(groundNormal, normal, (float)(k == 0 ? 1 : k) / (k + 1));
            }

            return groundNormal;
        }
    }
}