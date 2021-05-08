using UnityEngine;
using System;

public class Hovering : MonoBehaviour
{
    // planet along which is oriented the object
    public GameObject planet;

    // It moves/levitates by measuring the distance to ground with a
    // raycast then applying a force that decreases as the object
    // reaches the desired levitation height.

    // Vary the parameters below to get different control effects.
    // For example, reducing the hover damping will tend to make the object bounce
    // if it passes over an object underneath.
    float moveForce = 20.0f;
    float torque = 10.0f;
    float altitude = 2.0f;
    float hoverForce = 5.0f;
    // The amount that the lifting force is reduced per unit of upward speed.
    // This damping tends to stop the object from bouncing after passing over
    // something.
    float hoverDamp = 0.5f;

    Rigidbody m_rigidbody;
    int terrainLayer;
    float shipRadius;

    Mesh sphere;

    void Awake() {
        m_rigidbody = GetComponent<Rigidbody>();
        terrainLayer = LayerMask.NameToLayer("Terrain");
        shipRadius = 4f * transform.lossyScale.z;
        sphere = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
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
        Vector3 groundDirection = (planet.transform.position - transform.position).normalized;
        Debug.DrawRay(transform.position, groundDirection);

        Ray groundRay = new Ray(transform.position, groundDirection);
        RaycastHit groundHit;
        if (Physics.Raycast(groundRay, out groundHit, Mathf.Infinity, ~terrainLayer)) {
            // m_rigidbody.MoveRotation(m_rigidbody.rotation * Quaternion.FromToRotation(transform.up, groundHit.normal).normalized);
            float hoverDelta = altitude - groundHit.distance;
            float upwardSpeed = m_rigidbody.velocity.y;
            float lift = hoverDelta * hoverForce - upwardSpeed * hoverDamp;
            m_rigidbody.AddForce(lift * -groundDirection);
        }

        const int nbTangents = 8;
        Vector3[] tangents = new Vector3[nbTangents];
        int k = 0;
        for (int i = 0; i < nbTangents; i++) {
            float angle = (float)i / nbTangents * 180;
            float oppositeAngle = angle + 180;

            Vector3 shiftBase = transform.forward * shipRadius;
            Vector3 originA = transform.position + Quaternion.AngleAxis(angle, transform.up) * shiftBase;
            Vector3 originB = transform.position + Quaternion.AngleAxis(oppositeAngle, transform.up) * shiftBase;
            Debug.DrawRay(originA, groundDirection, Color.cyan);
            Debug.DrawRay(originB, groundDirection, Color.blue);
            Ray rayA = new Ray(originA, groundDirection);
            RaycastHit hitA;
            if (Physics.Raycast(rayA, out hitA, Mathf.Infinity, ~terrainLayer)) {
                Ray rayB = new Ray(originB, groundDirection);
                RaycastHit hitB;
                if (Physics.Raycast(rayB, out hitB, Mathf.Infinity, ~terrainLayer)) {
                    Vector3 tangent = (hitB.point - hitA.point).normalized;
                    tangents[k] = tangent;
                    k++;
                }
            }
        }

        Vector3 groundNormal = Vector3.one;

        for (int i = 0; i < k / 2; i++) {
            Vector3 tangent1 = tangents[i];
            Vector3 tangent2 = tangents[i + k / 2];
            Vector3 normal = Vector3.Cross(tangent1, tangent2);
            Debug.DrawRay(transform.position, normal, Color.white);

            groundNormal = Vector3.LerpUnclamped(groundNormal, normal, (float)(k == 0 ? 1 : k) / (k + 1));
        }

        transform.rotation = Quaternion.FromToRotation(transform.up, groundNormal) * transform.rotation;
        Debug.DrawRay(transform.position, -groundNormal, Color.black);
    }
}
