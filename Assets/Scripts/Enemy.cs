using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float rotationalDamp = 1.25f;

    public GameObject player;
    public HitBox hitBox;

    float movementSpeed = 2f;
    Rigidbody rb;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        Debug.Log("hitBox " + hitBox);
        hitBox.hit += Die;
    }

    void FixedUpdate() {
        Move();
    }

    void Die() {
        gameObject.SetActive(false);
        Debug.Log("Hit!");
    }

    void Turn() {
        Vector3 pos = player.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotationalDamp);
    }

    void Move() {
        transform.LookAt(player.transform);
    }
}

