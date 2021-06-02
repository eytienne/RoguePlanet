using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    public GameObject shooter;
    private double time;

    void OnTriggerEnter(Collider collider) {
        HitBox hitBox = collider.GetComponent<HitBox>();
        if (shooter.tag == "Enemy") return;
        if (hitBox) {
            hitBox.Hit();
            gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision col) {
        Attractor attractor;
        if (col.gameObject.TryGetComponent<Attractor>(out attractor)) {
            gameObject.SetActive(false);
        }
    }

    void FixedUpdate() {
        if (Time.time - time > 2) {
            gameObject.SetActive(false);
        }
    }

    public void setTime(double t) {
        this.time = t;
    }

}
