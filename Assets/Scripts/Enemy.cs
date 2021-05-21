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

    public bool death()
    {
        this.getCollider().SetActive(false);
        Debug.Log("Ennemi détruit");
        return true;
    }

    /*public void seekPlayer(Vector3 direction)
    {
        rb.MovePosition(transform.position + (direction * movementSpeed * Time.deltaTime));
    }*/
    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            col.gameObject.SetActive(false);
            this.death();
            ActiveEnemies.activeEnemies--;
        }

        if (col.gameObject.tag == "Planet")
        {
            this.getCollider().SetActive(false);
            this.death();
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

    void Update()
    {
        //movement = direction;
    }

    private void FixedUpdate()
    {
        //Turn();
        Move();
    }
}
