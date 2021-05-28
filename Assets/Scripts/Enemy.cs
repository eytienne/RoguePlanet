using CMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Enemy : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float rotationalDamp = 1.25f;

    public float moveSpeed = 10f;
    public GameObject planet;
    public GameObject objCollider;
    public GameObject player;
    private Rigidbody rb;
    private Vector3 gravity, groundDirection;
    int terrainLayer;

    static readonly Vector3[] speedCurve = Util.Vector2sToVector3s(new Vector2[] {
       new Vector2(0 , 0),
       new Vector2(0.5f , 0),
       new Vector2(0.5f , 1),
       new Vector2(1 , 1),
    });

    public void Awake()
    {
        planet = GameObject.Find("Planet");
        gravity = (planet.transform.position - transform.position).normalized;
        groundDirection = gravity;
        terrainLayer = LayerMask.NameToLayer("Terrain");
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

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

    public void Roam()
    {
        //déplacement de l'ennemie si le joueur n'est pass en vue
    }

    public void Shoot()
    {
        
    }

    public void IsInRange()
    {
        //se déplace vers le joueur car il est en ligne de vue
    }

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    Quaternion _rotateToGround;
    void FixedUpdate()
    {
        Vector3 groundNormal = transform.GetGroundNormal(planet, gravity, terrainLayer);

        Quaternion rotateToGround = Quaternion.FromToRotation(transform.up, groundNormal);
        rotateToGround = _rotateToGround = Quaternion.SlerpUnclamped(_rotateToGround, rotateToGround, 30 * Time.fixedDeltaTime);

        Vector3 playerDirection = player.transform.position;

        //Plane normalPlane = new Plane(groundNormal, transform.position);
        Vector3 _playerDirection = Vector3.ProjectOnPlane(playerDirection, groundNormal).normalized;
        Quaternion rotateToPlayerDirection = Quaternion.FromToRotation(transform.forward, _playerDirection);
        transform.rotation = rotateToPlayerDirection;

        rb.MoveRotation(
            (rotateToPlayerDirection *
            rotateToGround *
            rb.rotation).normalized
            );

        Vector3 playerForward = (_playerDirection - transform.position).normalized;

        Vector3 moveDirection = Quaternion.FromToRotation(transform.forward, playerForward) * transform.TransformDirection(_playerDirection.x, 0, _playerDirection.y);

        const float C = 100;
        rb.velocity = C * moveDirection * moveSpeed * Time.deltaTime;
        Debug.Log(playerDirection);

    }
}
