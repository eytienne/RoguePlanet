using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    public GameObject player;
    private double time;

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
        Debug.Log(Time.time - time);
        if (Time.time - time > 2) {
            gameObject.SetActive(false);
            Debug.Log("Trop long");
        }
    }

    public void setTime(double t)
    {
        this.time = t;
    }

}
