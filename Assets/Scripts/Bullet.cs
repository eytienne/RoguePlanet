using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    public GameObject player;

    Rigidbody m_rigidbody;

    void Awake() {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    public void OnCollisionEnter(Collision col) {
        Attractor attractor;
        if (col.gameObject.TryGetComponent<Attractor>(out attractor)) {
            gameObject.SetActive(false);
            Debug.Log("touche planete");
        }
    }

    void FixedUpdate() {
        if (m_rigidbody.velocity.magnitude < 1) {
            gameObject.SetActive(false);
            Debug.Log("Velocity trop basse");
        }
    }

}
