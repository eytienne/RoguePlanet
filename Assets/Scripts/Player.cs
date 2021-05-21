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

public class Player : MonoBehaviour
{
    public Camera m_camera;
    public Camera Camera {
        get {
            return m_camera;
        }
    }

    public Light flash;
    public float flashTotalSeconds;
    public float flashMaxIntensity;

    Rigidbody m_rigidbody;
    readonly Quaternion cameraOrientation = Quaternion.Euler(90, 0, 0);

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

    void Awake() {
        m_rigidbody = GetComponent<Rigidbody>();
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

Vector2 ? prevMousePosition = null;

    void Update() {
        Resolution resolution = Screen.currentResolution;
        // - 0.5f * new Vector2(resolution.height, resolution.width)
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        mousePosition -= prevMousePosition ?? mousePosition;
        prevMousePosition = mousePosition;
        mousePosition.Normalize();
        // Debug.Log("mousePos " + mousePosition);
        Vector3 position = new Vector3(mousePosition.x, transform.position.y, mousePosition.y);
        m_camera.transform.SetPositionAndRotation(transform.position + 25 * transform.up, transform.rotation * cameraOrientation);
        // transform.LookAt(transform.TransformDirection(position), transform.up);
    }

    void FixedUpdate() {
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
        Vector2 moveDirection = move0.magnitude == 0
            ? move1 // deceleration
            : move0 // acceleration
        ;

        m_rigidbody.MovePosition(m_rigidbody.position
            + transform.TransformDirection(moveDirection.x, 0, moveDirection.y) * kvelocity * moveSpeed * Time.deltaTime);
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
