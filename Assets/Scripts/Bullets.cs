using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    public GameObject player;
    private GameObject objCollider;

    private Rigidbody rb;

    private void Start()
    {
        objCollider = GameObject.FindGameObjectWithTag("Bullet");

    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Attractor")
        {
            //col.gameObject.SetActive(false);
            this.getCollider().SetActive(false);
            Debug.Log("touché planette");
        }
    }

    public GameObject getCollider()
    {
        return this.objCollider;
    }

    private void FixedUpdate()
    {
        rb = GetComponent<Rigidbody>();
        if (rb.velocity.magnitude < 1)
        {
            this.getCollider().SetActive(false);
            Debug.Log("Velocity trop basse");
        }
    }

}
