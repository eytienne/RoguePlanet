using CMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Enemy : MonoBehaviour
{
    [SerializeField] float rotationalDamp = 1.25f;

    public GameObject player;
    public GameObject planet;
    public HitBox hitBox;
    public float moveSpeed = 10f;

    Rigidbody rb;
    Vector3 gravity, groundDirection;
    int terrainLayer;

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
        hitBox.hit += Die;
    }

    Quaternion _rotateToGround;
    void FixedUpdate() {
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

    void Die() {
        gameObject.SetActive(false);
        Debug.Log("Hit!");
    }

    void Turn() {
        Vector3 pos = player.transform.position - transform.position;
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
}

