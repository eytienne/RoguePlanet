using CMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float rotationalDamp = 1.25f;

    public GameObject player;
    public GameObject planet;
    public HitBox hitBox;
    public float moveSpeed = 10f;

    Rigidbody rb;
    AudioSource audioData;
    int terrainLayer;
    Vector3 gravity, groundDirection;

    static readonly Vector3[] speedCurve = Util.Vector2sToVector3s(new Vector2[] {
       new Vector2(0 , 0),
       new Vector2(0.5f , 0),
       new Vector2(0.5f , 1),
       new Vector2(1 , 1),
    });

    void Awake() {
        rb = GetComponent<Rigidbody>();
        gravity = (planet.transform.position - transform.position).normalized;
        groundDirection = gravity;
        terrainLayer = LayerMask.NameToLayer("Terrain");
        target = GameObject.Find("Player");
        audioData = GetComponent<AudioSource>();
    }

    void OnEnable() {
        hitBox.hit += Die;
    }

    void FixedUpdate() {
        Move();
    }

    void Die() {
        AudioSource.PlayClipAtPoint(audioData.clip, transform.position);
        gameObject.SetActive(false);
    }

    void Turn() {
        Vector3 pos = target.transform.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(pos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotationalDamp);
    }

    void Roam() {
        //déplacement de l'ennemie si le joueur n'est pass en vue
    }

    void Shoot() {

    }

    void IsInRange() {
        //se déplace vers le joueur car il est en ligne de vue
    }

    void Move() {
        transform.LookAt(target.transform.position);
        transform.position += transform.forward * 25 * Time.deltaTime;
        // Debug.Log(target.transform.position);
    }
}

