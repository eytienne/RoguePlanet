using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float rotationalDamp = 1.25f;

    private float movementSpeed = 2f;
    public GameObject objCollider;
    public GameObject player;
    private Rigidbody rb;
    AudioSource audioData;
    public GameObject getCollider()
    {
        return this.objCollider;
    }

    public bool Die()
    {
        AudioSource.PlayClipAtPoint(audioData.clip, transform.position);
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
        Vector3 pos = target.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotationalDamp);
    }

    public void Move()
    {
        transform.LookAt(target.transform.position);
        transform.position += transform.forward * 25 * Time.deltaTime;
        //Debug.Log(target.transform.position);
    }

    void Start()
    {
        target = GameObject.Find("Player");
        audioData = GetComponent<AudioSource>();
        rb = this.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Move();
        Debug.DrawRay(transform.position, this.transform.forward, Color.green);
    }
}
