using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField, Range(1, 100)]
    int nbSegments = 10;
    [SerializeField]
    Noise noise;

    MeshFilter meshFilter;
    MeshCollider meshCollider;

    void Start() {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        noise = new Noise(new NoiseOctave(0.15f, 3, 0), 4, 0.85f, 5, 1);
        noise.maskAgainstFirstOctave = true;
        SetupMesh();
    }

    void OnValidate() {
        if (meshFilter == null)
            return;
        SetupMesh();
    }

    void SetupMesh() {
        meshFilter.sharedMesh = GenerateSphere(nbSegments);
        meshCollider.sharedMesh = GenerateSphere(nbSegments / 3);
    }

    Mesh GenerateSphere(int nbSegments = 1) {
        Mesh mesh = new Mesh();
        mesh.name = "sphere";

        CombineInstance[] combines = new CombineInstance[6];
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

        // transform cube face to "sphere face"
        Mesh upperMesh = GeneratePlaneXZ(2 * Vector2.one, nbSegments, nbSegments);

        // reorient the faces
        Quaternion upToBack = Quaternion.FromToRotation(Vector3.up, Vector3.back);
        for (int i = 0; i < 6; i++) {
            Vector3 dir = directions[i];
            Mesh copy = Instantiate(upperMesh);
            Vector3[] vertices = copy.vertices;
            Vector3[] normals = new Vector3[vertices.Length];
            for (int j = 0; j < vertices.Length; j++) {
                Vector3 vertex = vertices[j];
                vertex.y += 1;

                vertex = EvenSpherePoint(vertex);
                if (dir != Vector3.up) {
                    Quaternion orientation = Quaternion.FromToRotation(Vector3.back, dir);
                    vertex = Matrix4x4.Rotate(orientation * upToBack) * vertex;
                }
                normals[j] = vertex;

                float effectiveRadius = 1 + noise.GetElevation(vertex);
                vertex *= effectiveRadius;

                vertex.y -= 1;
                vertices[j] = vertex;
            }
            copy.vertices = vertices;
            copy.normals = normals;
            combines[i].mesh = copy;
        }

        mesh.CombineMeshes(combines, true, false);
        mesh.Optimize();

        return mesh;
    }

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
}