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
    public float roamSpeed = 3f;
    public Pool bulletPool;
    public float fireRate = 2f;

    bool shooterEnemy = false;
    bool isShooting = false;
    float roamTimeInADirection = 4;
    double lastRoam;


    Rigidbody m_rigidbody;
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
        m_rigidbody = GetComponent<Rigidbody>();
        gravity = (planet.transform.position - transform.position).normalized;
        groundDirection = gravity;
        terrainLayer = LayerMask.NameToLayer("Terrain");
        target = GameObject.Find("Player");
        audioData = GetComponent<AudioSource>();
        lastRoam = -1 - roamTimeInADirection;
    }

    void OnEnable() {
        hitBox.hit += Die;
    }

    void OnDisable() {
        Debug.Log("OnDisable enemy");
        hitBox.hit -= Die;
    }

    Coroutine CoroutineFire;
    Vector3 randomeMove;
    void FixedUpdate() {
        Vector3 toPlayer = player.transform.position - transform.position;
        float playerDistance = toPlayer.magnitude;
        if (playerDistance <= 50) {
            Move(toPlayer, moveSpeed);
            if (playerDistance <= 30 && isShooting == false && shooterEnemy == true) {
                CoroutineFire = StartCoroutine(ShotNow());
                // Debug.Log("Dans la zone, dist : " + dist);
            } else if (playerDistance > 30 && isShooting == true) {
                isShooting = false;
                // Debug.Log("hors de la zone, dist : " + dist);
                StopCoroutine(CoroutineFire);
            }
        } else {
            // roaming si le joueur n'est pas en vue
            if (Time.fixedTimeAsDouble - lastRoam > roamTimeInADirection) {
                lastRoam = Time.fixedTimeAsDouble;
                randomeMove = transform.TransformDirection(new Vector3(UnityEngine.Random.value * 2 - 1, 0, UnityEngine.Random.value * 2 - 1)).normalized;
            }
            Move(randomeMove, roamSpeed);
        }
    }

    public void SetShootingAbility(bool b) {
        this.shooterEnemy = b;
    }

    void Die() {
        AudioSource.PlayClipAtPoint(audioData.clip, transform.position);
        gameObject.SetActive(false);
    }

    void Move(Vector3 moveDirection, float moveSpeed) {
        // transform.LookAt(target.transform.position);
        Vector3 groundNormal = transform.GetGroundNormal(planet, groundDirection, terrainLayer);
        m_rigidbody.MoveRotation(
            Quaternion.FromToRotation(transform.forward, moveDirection).normalized
            * Quaternion.FromToRotation(transform.up, groundNormal).normalized
            * m_rigidbody.rotation
        );

        moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;
        Debug.DrawRay(transform.position, moveDirection.normalized, Color.magenta);
        m_rigidbody.velocity = moveDirection * moveSpeed;
    }

    IEnumerator ShotNow() {
        while (true) {
            GameObject bullet = bulletPool.GetObject();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;
            bullet.GetComponent<Bullet>().Initialize(transform.forward * 50);
            bullet.SetActive(true);
            isShooting = true;
            yield return new WaitForSeconds(fireRate);
        }
    }
}

