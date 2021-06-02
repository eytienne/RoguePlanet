using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class StuckAroundTheGround : MonoBehaviour
{
    // planet to stick to
    public GameObject planet;
    public float G = -Physics.gravity.y;
    public float altitude = 0;

    Rigidbody m_rigidbody;
    int terrainLayer;
    float shipRadius;
    double? stuck;

    void Awake() {
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.useGravity = false;
        terrainLayer = LayerMask.NameToLayer("Terrain");
        shipRadius = 4f * transform.lossyScale.z;
    }

    void OnEnable() {
        Unstick();
    }

    void Start() {
        m_rigidbody.drag = 0.5f;
        m_rigidbody.angularDrag = 0.5f;
    }

    event Action _OnDrawGizmos;
    void OnDrawGizmos() {
        _OnDrawGizmos?.Invoke();
    }

    void FixedUpdate() {
        _OnDrawGizmos = null;
        Vector3 transformPosition = transform.position;
        Vector3 groundDirection = planet.transform.position - transformPosition;

        // check altitude
        Ray groundRay = new Ray(transformPosition, groundDirection);
        RaycastHit groundHit;
        if (!Physics.Raycast(groundRay, out groundHit, Mathf.Infinity, ~terrainLayer)) return;
        Vector3 nextPosition = m_rigidbody.position + m_rigidbody.velocity * Time.fixedDeltaTime;
        float currentAltitude = Vector3.Distance(nextPosition, groundHit.point);
        if (currentAltitude < altitude) {
            Stick();
        } else {
            Unstick();
        }

        // apply gravity or stick it if needed
        Vector3 gravity = groundDirection.normalized * G * G * m_rigidbody.mass / groundDirection.magnitude;

        if (stuck is double) {
            Debug.DrawRay(transformPosition, m_rigidbody.velocity, Color.green);
            if (Time.fixedTimeAsDouble - stuck < 2 * Time.fixedDeltaTime) {
                m_rigidbody.velocity = Vector3.zero;
            }
        } else {
            m_rigidbody.AddForce(gravity, ForceMode.Acceleration);
        }
    }

    public void Stick() {
        stuck = Time.fixedTimeAsDouble;
    }

    public void Unstick() {
        stuck = null;
    }
}
