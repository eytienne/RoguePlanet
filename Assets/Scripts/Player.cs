using CMath;
using Cysharp.Threading;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class Player : MonoBehaviour
{
    public Camera m_camera;

    public Light flash;
    public float flashTotalSeconds;
    public float flashMaxIntensity;

    Rigidbody m_rigidbody;

    public float moveSpeed = 10;
    public float accelerationTime = 0.5f;
    public float decelerationTime = 0.4f;
    public float delayBeforeStop = 0.35f;
    PlayerControls inputActions;
    const int lastMovesLimit = 3;

    // struct to persist the data of the Input System context getters which only return global current values
    struct _CallbackContext
    {
        public double time;
        public Vector2 value;

        public _CallbackContext(InputAction.CallbackContext _) {
            this.value = _.ReadValue<Vector2>();
            this.time = _.time;
        }
    }

    FixedQueue<_CallbackContext> lastMoves = new FixedQueue<_CallbackContext>(lastMovesLimit);

    static readonly Vector3[] ease = Util.Vector2sToVector3s(new Vector2[] {
       new Vector2(0 , 0),
       new Vector2(0.5f , 0),
       new Vector2(0.5f , 1),
       new Vector2(1 , 1),
    });

    public GameObject planet;

    Vector3 mouseDirection;
    Vector3 moveDirection;

    // It moves/levitates by measuring the distance to ground with a
    // raycast then applying a force that decreases as the object
    // reaches the desired levitation height.

    // Vary the parameters below to get different control effects.
    // For example, reducing the hover damping will tend to make the object bounce
    // if it passes over an object underneath.
    float moveForce = 20.0f;
    float torque = 10.0f;
    float altitude = 2.0f;
    float hoverForce = 5.0f;
    // The amount that the lifting force is reduced per unit of upward speed.
    // This damping tends to stop the object from bouncing after passing over
    // something.
    float hoverDamp = 0.5f;

    int terrainLayer;
    float shipRadius;

    void Awake() {
        m_rigidbody = GetComponent<Rigidbody>();
        terrainLayer = LayerMask.NameToLayer("Terrain");
        shipRadius = transform.lossyScale.z;
    }

    void Start() {
        m_rigidbody.drag = 0.5f;
        m_rigidbody.angularDrag = 0.5f;
    }

    // void Start() {
    //     Debug.Log("Start 0");
    //     StartCoroutine(StartLog());
    //     Debug.Log("Start 3");
    // }

    // IEnumerator StartLog() {
    //     yield return new WaitForSeconds(0);
    //     Debug.Log("Start 1");
    //     yield return new WaitForSeconds(1);
    //     Debug.Log("Start 2");
    // }

    // void Start() {
    //     Debug.Log("Start 0");
    //     UniTask.Run(async() => {
    //         Debug.Log("Start 1");
    //         await UniTask.Delay(1000);
    //         Debug.Log("Start 2");
    //     });
    //     Debug.Log("Start 3");
    // }

    void OnEnable() {
        inputActions = new PlayerControls();
        inputActions.Enable();
        inputActions.Player.Move.performed += Move;
        inputActions.Player.Fire.performed += _ => Fire();
    }

    void OnDisable() {
        inputActions.Disable();
    }

    void Move(InputAction.CallbackContext context) {
        lastMoves.Enqueue(new _CallbackContext(context));
    }

    void Fire()
    {
        StartCoroutine("flashNow");
        Debug.Log("feu");
        GameObject bullet = BulletsPool.Instance.SpawnFromPool("bullets", transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * 50;
    }
    void OnDrawGizmos() {
        _OnDrawGizmos?.Invoke();
    }
    event Action _OnDrawGizmos;

    Vector3 gravity;

    void Update() {
        _OnDrawGizmos = null;

        gravity = (planet.transform.position - transform.position).normalized;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray mouseRay = m_camera.ScreenPointToRay(mousePosition);

        Plane plane = new Plane(-gravity, transform.position);
        _OnDrawGizmos += () => {
            // CGizmos.DrawPlane(plane, Vector3.one * 0.25f);
        };
        float enter;
        if (plane.Raycast(mouseRay, out enter)) {
            Vector3 hitPoint = mouseRay.GetPoint(enter);
            mouseDirection = (hitPoint - transform.position).normalized;

            _OnDrawGizmos += () => {
                Gizmos.DrawSphere(hitPoint, 0.1f);
            };
        }

        // transform.LookAt(transform.TransformDirection(position), transform.up);
    }

    Vector3 transformPosition;

    void LateUpdate() {
        Transform ctransform = m_camera.transform;
        //ctransform.rotation = Quaternion.FromToRotation(-m_camera.transform.forward, gravity) * ctransform.rotation;

        // Quaternion orientation = Quaternion.LookRotation(gravity, -gravity);
        // ctransform.rotation = orientation;
        ctransform.LookAt(transform, ctransform.up);

        _OnDrawGizmos += () => {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(ctransform.position, gravity);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(ctransform.position, transform.right);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ctransform.position, mouseDirection);
        };
    }

    void FixedUpdate() {
        Vector3 groundDirection = (planet.transform.position - transform.position).normalized;

        Ray groundRay = new Ray(transform.position, groundDirection);
        RaycastHit groundHit;
        if (Physics.Raycast(groundRay, out groundHit, Mathf.Infinity, ~terrainLayer)) {
            float hoverDelta = altitude - groundHit.distance;
            float upwardSpeed = m_rigidbody.velocity.y;
            float lift = hoverDelta * hoverForce - upwardSpeed * hoverDamp;
            m_rigidbody.AddForce(lift * -groundDirection);
        }

        const int nbTangents = 8;
        Vector3[] tangents = new Vector3[nbTangents];
        int k = 0;
        for (int i = 0; i < nbTangents; i++) {
            float angle = (float)i / nbTangents * 180;
            float oppositeAngle = angle + 180;

            Vector3 shiftBase = transform.forward * shipRadius;
            Vector3 originA = transform.position + Quaternion.AngleAxis(angle, transform.up) * shiftBase;
            Vector3 originB = transform.position + Quaternion.AngleAxis(oppositeAngle, transform.up) * shiftBase;
            Debug.DrawRay(originA, groundDirection, Color.cyan);
            Debug.DrawRay(originB, groundDirection, Color.blue);
            Ray rayA = new Ray(originA, groundDirection);
            RaycastHit hitA;
            if (Physics.Raycast(rayA, out hitA, Mathf.Infinity, ~terrainLayer)) {
                Ray rayB = new Ray(originB, groundDirection);
                RaycastHit hitB;
                if (Physics.Raycast(rayB, out hitB, Mathf.Infinity, ~terrainLayer)) {
                    Vector3 tangent = (hitB.point - hitA.point).normalized;
                    tangents[k] = tangent;
                    k++;
                }
            }
        }

        Vector3 groundNormal = Vector3.zero;
        for (int i = 0; i < k / 2; i++) {
            Vector3 tangent1 = tangents[i];
            Vector3 tangent2 = tangents[i + k / 2];
            Vector3 normal = Vector3.Cross(tangent1, tangent2);

            groundNormal = Vector3.LerpUnclamped(groundNormal, normal, (float)(k == 0 ? 1 : k) / (k + 1));
        }
        Debug.Log("groundNormal " + groundNormal + " magnitude " + groundNormal.magnitude);

        Quaternion rotateToMouseDirection = Quaternion.FromToRotation(transform.forward, mouseDirection);
        Quaternion rotateToGround = Quaternion.FromToRotation(transform.up, groundNormal);
        m_rigidbody.MoveRotation(
            rotateToMouseDirection *
            rotateToGround *
            m_rigidbody.rotation
            );


        float t = 0;
        var ctx2 = lastMoves.Get(2);
        var ctx1 = lastMoves.Get(1);
        Vector2 move1 = ctx1?.value ?? Vector2.zero;
        var ctx0 = lastMoves.ElementAtOrDefault(0);
        Vector2 move0 = ctx0.value;

        double moveDeltaTime = Time.realtimeSinceStartupAsDouble - ctx0.time;
        double delay = ctx0.time - (ctx1?.time ?? -1);
        if ((move1.magnitude == 0 || move0.magnitude == 0) && (delay > delayBeforeStop)) { // stop or go
            t = (float)moveDeltaTime / accelerationTime;
        } else { // only direction change
            t = 1;
        }
        float kvelocity = Tooling.GetBezierCubicPoint(Mathf.Clamp01(t), ease).y;
        if (move0.magnitude == 0) {
            kvelocity = 1 - kvelocity;
        }

        bool same = ctx0.Equals(ctx2);

        // choose the move direction from the last move or not
        Vector2 _moveDirection = move0.magnitude == 0
            ? move1 // deceleration
            : move0 // acceleration
        ;
        _moveDirection.Normalize();

        Vector3 moveDirection = transform.TransformDirection(_moveDirection.x, 0, _moveDirection.y);
        _OnDrawGizmos += () => {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, moveDirection);
        };

        m_rigidbody.MovePosition(m_rigidbody.position
            + moveDirection * kvelocity * moveSpeed * Time.deltaTime);

        m_camera.transform.position = transform.position + 25 * -gravity;
    }

    IEnumerator flashNow() {
        float waitTime = flashTotalSeconds / 2;
        // Get half of the seconds (One half to get brighter and one to get darker)
        while (flash.intensity < flashMaxIntensity) {
            flash.intensity += Time.deltaTime / waitTime;
            yield return null;
        }
        while (flash.intensity > 0) {
            flash.intensity -= Time.deltaTime / waitTime;
            yield return null;
        }
        if (flash.intensity != 0) {
            flash.intensity = 0;
        };
        yield return null;
    }

}
