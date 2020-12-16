using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField, Range(1, 50)]
    int nbSegments = 10;

    MeshFilter meshFilter;

    void Start() {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        SetupMesh();
    }

    void OnValidate() {
        if (meshFilter == null)
            return;
        SetupMesh();
    }

    void SetupMesh() {
        meshFilter.sharedMesh = GenerateSphere(nbSegments, PerlinNoise);
    }

    delegate float Noise(float x, float y, float z);

    Mesh GenerateSphere(int nbSegments = 1, Noise noise = null) {
        Mesh mesh = new Mesh();
        mesh.name = "sphere";

        CombineInstance[] combines = new CombineInstance[6];
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

        // transform cube face to "sphere face"
        Mesh upperMesh = GeneratePlaneXZ(2 * Vector2.one, nbSegments, nbSegments);
        Vector3[] vertices = upperMesh.vertices;
        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++) {
            Vector3 vertex = vertices[i];
            vertex.y += 1;

            Vector3 absVertex = new Vector3(Mathf.Abs(vertex.x), Mathf.Abs(vertex.y), Mathf.Abs(vertex.z));
            // temp way to avoid holes on the face edges due to the noise
            float tx = 10 * Mathf.Clamp(1 - absVertex.x, 0f, 0.1f);
            float ty = 10 * Mathf.Clamp(1 - absVertex.z, 0f, 0.1f);
            normals[i] = vertex.normalized;
            vertex = EvenSpherePoint(vertex);

            Vector3 noiseInput = 50 * vertex;
            float effectiveRadius = 1
                // + tx * ty * 0.1f * noise(noiseInput.x, noiseInput.z)
                ;
            vertex *= effectiveRadius;
            vertex.y -= 1;
            vertices[i] = vertex;
        }
        upperMesh.vertices = vertices;
        upperMesh.normals = normals;

        // reorient the faces
        Quaternion upToBack = Quaternion.FromToRotation(Vector3.up, Vector3.back);
        for (int i = 0; i < 6; i++) {
            combines[i].mesh = upperMesh;
            Vector3 dir = directions[i];
            Quaternion orientation = Quaternion.FromToRotation(Vector3.back, dir);
            combines[i].transform =
                Matrix4x4.Translate(dir)
                * Matrix4x4.Rotate(orientation * upToBack)
                ;
        }

        mesh.CombineMeshes(combines);
        mesh.Optimize();

        return mesh;
    }

    // readonly static float

    Vector3 EvenSpherePoint(Vector3 squarePoint) {
        Vector3 v = squarePoint;
        float x2 = v.x * v.x;
		float y2 = v.y * v.y;
		float z2 = v.z * v.z;

		Vector3 ret;
		ret.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
		ret.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
		ret.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

        return ret;
    }

    Mesh GeneratePlaneXZ(Vector2 size, int nbSegmentsX = 1, int nbSegmentsY = 1) {
        Mesh mesh = new Mesh();
        mesh.name = "planeXZ";

        Vector2 halfSize = size / 2;

        int nbSquares = nbSegmentsX * nbSegmentsY;
        Vector3[] vertices = new Vector3[(nbSegmentsX + 1) * (nbSegmentsY + 1)];
        Vector2[] uv = new Vector2[(nbSegmentsX + 1) * (nbSegmentsY + 1)];
        int[] triangles = new int[nbSquares * 2 * 3];


        for (int vi = 0, i = 0; i < nbSegmentsX + 1; i++) {
            float kx = (float)i / nbSegmentsX;
            for (int j = 0; j < nbSegmentsY + 1; j++, vi++) {
                float ky = (float)j / nbSegmentsY;
                vertices[vi] = new Vector3(-halfSize.x + kx * size.x, 0, -halfSize.y + ky * size.y);
                uv[vi] = new Vector2(kx, ky);
            }
        }

        for (int i = 0; i < nbSquares; i++) {
            int ii = i / nbSegmentsY, jj = i % nbSegmentsY;
            int v0i = ii * (nbSegmentsY + 1) + jj;
            int columnStride = nbSegmentsY + 1;

            // lower right triangle
            triangles[i * 6 + 0] = v0i;
            triangles[i * 6 + 1] = v0i + columnStride + 1;
            triangles[i * 6 + 2] = v0i + columnStride;

            // upper left triangle
            triangles[i * 6 + 3] = v0i;
            triangles[i * 6 + 4] = v0i + 1;
            triangles[i * 6 + 5] = v0i + 1 + columnStride;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.Optimize();

        return mesh;
    }

    float PerlinNoise(Vector3 v) {
        return PerlinNoise(v.x, v.y, v.z);
    }

    float PerlinNoise(float x, float y, float z) {
        float sum = 0;

        sum += Mathf.PerlinNoise(x, y);
        sum += Mathf.PerlinNoise(y, z);
        sum += Mathf.PerlinNoise(x, z);

        sum += Mathf.PerlinNoise(y, x);
        sum += Mathf.PerlinNoise(z, y);
        sum += Mathf.PerlinNoise(z, x);

        return sum / 6;
    }
}