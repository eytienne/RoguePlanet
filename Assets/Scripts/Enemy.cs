using CMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

[RequireComponent(typeof(Pool))]

public class Enemy : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] float rotationalDamp = 1.25f;

    public GameObject player;
    public GameObject planet;
    public HitBox hitBox;
    public float moveSpeed = 10f;
    public Pool bulletPool;
    public float fireRate = 2f;

    private bool shooterEnemy = false;
    private bool isShooting = false;
    private double time;

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

    Coroutine CoroutineFire;
    void FixedUpdate() {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        Debug.Log(shooterEnemy);
        if (dist <= 50)
        {
            Move();
            if (dist <= 30 && isShooting == false && shooterEnemy == true)
            {
                CoroutineFire = Shoot();
                Debug.Log("Dans la zone, dist : " + dist);
            }
            else if (dist > 30 && isShooting == true)
            {
                isShooting = false;
                Debug.Log("hors de la zone, dist : " + dist);
                StopCoroutine(CoroutineFire);
            }
        }
        else if(Time.time - time > 2)
            Roam();
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

    /* RandomUnitVector()
    {
        float random = Random.Range(0f, 260f);
        return new Vector2(Mathf.Cos(random), Mathf.Sin(random));
    }*/

    void Roam() {
        //déplacement de l'ennemie si le joueur n'est pas en vue
        time = Time.time;
        Vector3 transformDirection = transform.TransformDirection(new Vector3(UnityEngine.Random.value * 2 - 1, 0, UnityEngine.Random.value * 2 - 1)).normalized;
        Vector3 groundNormal = transform.GetGroundNormal(planet, groundDirection, terrainLayer);
        Vector3 moveDirection = Vector3.ProjectOnPlane(transformDirection, groundNormal).normalized;

        transform.GetComponent<Rigidbody>().velocity = moveDirection * moveSpeed;

    }

    Coroutine Shoot() {
        return StartCoroutine(ShotNow());

    }

    void IsInRange() {
        //se déplace vers le joueur car il est en ligne de vue
        
    }

    void Move() {
        transform.LookAt(target.transform.position);
        transform.GetComponent<Rigidbody>().velocity = transform.forward * moveSpeed;
    }

    public void setShootingAbility(bool b)
    {
        this.shooterEnemy = b;
    }

    IEnumerator ShotNow()
    {
        while (true)
        {

            GameObject bullet = bulletPool.GetObject();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;
            bullet.GetComponent<Bullet>().setTime(Time.time);
            bullet.GetComponent<Rigidbody>().velocity = transform.forward * 50;
            bullet.SetActive(true);
            isShooting = true;
            yield return new WaitForSeconds(fireRate);
        }
    }
}

