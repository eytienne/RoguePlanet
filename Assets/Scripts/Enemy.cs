using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float rotationalDamp = 1.25f;

    private float movementSpeed = 2f;
    public GameObject objCollider;
    public GameObject player;
    private Rigidbody rb;

    public GameObject getCollider()
    {
        return this.objCollider;
    }

    public bool Die()
    {
        this.getCollider().SetActive(false);
        Debug.Log("Ennemi détruit");
        return true;
    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            col.gameObject.SetActive(false);
            Die();
            ActiveEnemies.activeEnemies--;
        }

        if (col.gameObject.tag == "Planet")
        {
            getCollider().SetActive(false);
            Die();
            Debug.Log("touché planette enemy");
            ActiveEnemies.activeEnemies--;
        }
    }

    public void Turn()
    {
        Vector3 pos = target.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotationalDamp);
    }

    public void Move()
    {
        transform.LookAt(target);
    }

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Move();
    }
}
