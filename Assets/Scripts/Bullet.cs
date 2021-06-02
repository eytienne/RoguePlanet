using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public GameObject shooter;
    public GameObject planet;
    int terrainLayer;
    double shotTime;
    Vector3 shotDirection;
    Rigidbody m_rigidbody;

    void Awake() {
        m_rigidbody = GetComponent<Rigidbody>();
        terrainLayer = LayerMask.NameToLayer("Terrain");
    }

    void OnEnable() {
        shotTime = Time.time;
        // to reset the pooled object
        m_rigidbody.velocity = Vector3.zero;
        // m_rigidbody.velocity = 20 * shotDirection;
    }

    void OnTriggerEnter(Collider collider) {
        if (shooter.tag == collider.tag) return;
        HitBox hitBox = collider.GetComponent<HitBox>();
        if (hitBox) {
            hitBox.Hit();
            gameObject.SetActive(false);
        }
    }

    void FixedUpdate() {
        if (Time.fixedTime - shotTime > 2) {
            gameObject.SetActive(false);
            return;
        }
        Vector3 groundNormal = transform.GetGroundNormal(planet, planet.transform.position - transform.position, terrainLayer);
        shotDirection = Vector3.ProjectOnPlane(shotDirection, groundNormal).normalized;
        // Debug.DrawRay(transform.position, shotDirection, Color.red);
        m_rigidbody.velocity = 20 * shotDirection;
    }

    public void Initialize(Vector3 shotDirection) {
        this.shotDirection = transform.TransformVector(shotDirection).normalized;
    }
}
