using System;
using UnityEngine;

[Serializable, RequireComponent(typeof(MeshRenderer)), ExecuteInEditMode]
public class Planet : MonoBehaviour
{

    [Range(1, 256)]
    public int nbSegments = 10;
    [Range(0.001f, 1)]
    public float colliderPrecision = 0.33f;

    public MinMax elevationMinMax;
    public ColourSettings coloursSettings;
    ColourGenerator colourGenerator = new ColourGenerator();
    SimplexNoise simplexNoise;

    // currently only avoid collider rendering
    public bool designMode = false;

    [Flags]
    public enum Faces
    {
        Up = 1 << 0,
        Down = 1 << 1,
        Right = 1 << 2,
        Left = 1 << 3,
        Forward = 1 << 4,
        Back = 1 << 5,
        All = Up | Down | Right | Left | Forward | Back
    }
    public Faces toRender;
    static readonly Vector3[] directions = {
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back,
    };

    [RequireComponent(typeof(MeshRenderer), typeof(MeshCollider), typeof(MeshFilter)), ExecuteInEditMode]
    public class Face : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;
        public MeshCollider meshCollider;

        void Awake() {
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
            meshFilter = GetComponent<MeshFilter>();
        }
    }
    Face[] faces;

    public ShapeSettings shape;
    [HideInInspector]
    public bool shapeFoldout;

    public MeshRenderer meshRenderer;

    int terrainLayer;

    public void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        if (!shape) shape = new ShapeSettings();
        terrainLayer = LayerMask.NameToLayer("Terrain");
        colourGenerator.UpdateSettings(coloursSettings);
        colourGenerator.UpdateColours();
    }

    public void Start() {
        faces = GetComponentsInChildren<Face>();
    }

    public void Initialize() {
        colourGenerator.UpdateSettings(coloursSettings);
        elevationMinMax = new MinMax();

        if (faces.Length != 6) faces = new Face[6];
        for (int i = 0; i < 6; i++) {
            if (faces[i] == null) {
                GameObject go = new GameObject("face");
                go.transform.parent = transform;
                go.transform.localScale = Vector3.one;
                Debug.Log("init face " + i);
                faces[i] = go.AddComponent<Face>();
            }
            ;
            Face face = faces[i];
            face.gameObject.layer = terrainLayer;
            face.GetComponent<MeshRenderer>().sharedMaterial = coloursSettings.planetMaterial;
        }
    }

    public void OnInspectorUpdate() {
        Initialize();
        SetupMesh();
        colourGenerator.UpdateColours();
    }

    void SetupMesh() {
        for (int i = 0; i < 6; i++) {
            Face face = faces[i];
            bool renderIt = toRender.HasFlag((Faces)(1 << i));
            face.meshFilter.sharedMesh = renderIt ? GenerateFaceMesh(directions[i], nbSegments) : null;
            face.meshCollider.sharedMesh = renderIt && !designMode ? GenerateFaceMesh(directions[i], Mathf.CeilToInt(colliderPrecision * nbSegments)) : null;
            face.GetComponent<MeshRenderer>().sharedMaterial = coloursSettings.planetMaterial;
            colourGenerator.UpdateElevation(elevationMinMax);
        }
    }

    public Mesh GenerateFaceMesh(Vector3 dir, int nbSegments) {
        Vector3 axisA = new Vector3(dir.y, dir.z, dir.x);
        Vector3 axisB = Vector3.Cross(dir, axisA);

        Vector3[] vertices = new Vector3[nbSegments * nbSegments];
        int[] triangles = new int[(nbSegments - 1) * (nbSegments - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < nbSegments; y++) {
            for (int x = 0; x < nbSegments; x++) {
                int i = x + y * nbSegments;
                Vector2 percent = new Vector2(x, y) / (nbSegments - 1);
                Vector3 pointOnUnitCube = dir + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 even = EvenSpherePoint(pointOnUnitCube);
                float elevation = CalculateElevation(even);
                elevationMinMax.AddValue(1 + elevation);
                even *= 1 + elevation;
                vertices[i] = even;
                
                if (x != nbSegments - 1 && y != nbSegments - 1) {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + nbSegments + 1;
                    triangles[triIndex + 2] = i + nbSegments;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + nbSegments + 1;
                    triIndex += 6;
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "face";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
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


    public float CalculateElevation(Vector3 pointOnUnitSphere) {
        float firstLayerValue = 0;
        float elevation = 0;

        if (shape.noiseLayers.Length > 0) {
            firstLayerValue = shape.noiseLayers[0].GetNoise().GetElevation(pointOnUnitSphere);
            if (shape.noiseLayers[0].enabled) {
                elevation = firstLayerValue;
            }
        }

        for (int i = 1; i < shape.noiseLayers.Length; i++) {
            if (shape.noiseLayers[i].enabled) {
                float mask = (shape.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
                elevation += shape.noiseLayers[i].GetNoise().GetElevation(pointOnUnitSphere) * mask;
            }
        }
        return elevation;
    }
}