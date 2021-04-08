using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float movementSpeed = 10.0f;
    public GameObject objCollider;
    public GameObject player;
    private Rigidbody rb;
    private Vector2 movement;

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

    public void seekPlayer(Vector3 direction)
    {
        rb.MovePosition(transform.position + (direction * movementSpeed * Time.deltaTime));
    }
    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            //col.gameObject.SetActive(false);
            this.death();
            ActiveEnemies.activeEnemies--;
        }
    }

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize();
        movement = direction;
    }

    private void FixedUpdate()
    {
        seekPlayer(movement);
    }
}
