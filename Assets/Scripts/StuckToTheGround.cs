using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class StuckToTheGround : MonoBehaviour
{
    // planet to stick to
    public GameObject planet;
    public float G;

    Rigidbody m_rigidbody;
    int terrainLayer;
    float shipRadius;
    double? stuck;

    void Awake() {
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.useGravity = false;
        terrainLayer = LayerMask.NameToLayer("Terrain");
        shipRadius = 4f * transform.lossyScale.z;
        G = G != 0 ? G : -Physics.gravity.y;
    }

    void OnEnable() {
        stuck = null;
    }

    void Start() {
        m_rigidbody.drag = 0.5f;
        m_rigidbody.angularDrag = 0.5f;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject == planet) {
            stuck = Time.fixedTimeAsDouble;
        }
    }

    void OnCollisionExit(Collision collisionInfo) {
        if (collisionInfo.gameObject == planet) {
            stuck = null;
        }
    }

    event Action _OnDrawGizmos;
    void OnDrawGizmos() {
        _OnDrawGizmos?.Invoke();
    }

    void FixedUpdate() {
        _OnDrawGizmos = null;
        Vector3 groundDirection = planet.transform.position - transform.position;
        Vector3 gravity = groundDirection.normalized * G * G * m_rigidbody.mass / groundDirection.magnitude;
        if (stuck is double) {
            Debug.DrawRay(transform.position, m_rigidbody.velocity, Color.green);
            if (Time.fixedTimeAsDouble - stuck < 2 * Time.fixedDeltaTime) {
                m_rigidbody.velocity = Vector3.zero;
            }
            return;
        }
        // Ray groundRay = new Ray(transform.position, groundDirection);
        // RaycastHit hitInfo;
        // if (Physics.Raycast(groundRay, out hitInfo, Mathf.Infinity, ~terrainLayer)) {
        //     const float DISTANCE_TO_SLOW = 5f;
        //     float t = Mathf.Clamp01(hitInfo.distance / DISTANCE_TO_SLOW);
        //     if (t < 1) Debug.Log($"hitInfo.distance {hitInfo.distance} t {t}");
        //     Debug.DrawRay(transform.position, gravity, Color.cyan);
        // }
        m_rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }
}
