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
using UnityEngine.UI;

[RequireComponent(typeof(Pool))]
public class Player : MonoBehaviour
{
    public Camera m_camera;
    AudioSource audioData;
    public Light flash;
    public float fireRate;

    Rigidbody m_rigidbody;

    public float moveSpeed = 10;
    public float accelerationTime = 0.5f;
    public float decelerationTime = 0.4f;
    public float delayBeforeStop = 0.35f;
    PlayerControls inputActions;

    const int lastMovesLimit = 3;
    public int life = 100;
    public Slider slider;
    public int bulletnumber = 1;
    // struct to persist the data of the Input System context getters which only return global current values
    public PauseMenu pauseMenu;
    private float tempstir;
    public GameObject GameOverCanvas;

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

    static readonly Vector3[] speedCurve = Util.Vector2sToVector3s(new Vector2[] {
       new Vector2(0 , 0),
       new Vector2(0.5f , 0),
       new Vector2(0.5f , 1),
       new Vector2(1 , 1),
    });

    static readonly Func<float, Vector3> hoverSinusoidal = Tooling.SinusoidalFromBezierCubic01(Util.Vector2sToVector3s(new Vector2[] {
       new Vector2(0 , 0),
       new Vector2(0.7f , 0),
       new Vector2(0.3f , 1),
       new Vector2(1 , 1),
    }));

    public GameObject planet;

    // It moves/levitates by measuring the distance to ground with a
    // raycast then applying a force that decreases as the object
    // reaches the desired levitation height.

    // Vary the parameters below to get different control effects.
    // For example, reducing the hover damping will tend to make the object bounce
    // if it passes over an object underneath.
    public float altitude = 2.0f;
    public float hoverForce = 9.0f;
    // The amount that the lifting force is reduced per unit of upward speed.
    // This damping tends to stop the object from bouncing after passing over
    // something.
    public float hoverDamp = 2f;
    public float hoverPeriod = 0.5f;
    public float hoverAmplitude = 2f;

    int terrainLayer;
    float shipRadius;

    public Pool bulletPool;

    void Awake() {
        m_rigidbody = GetComponent<Rigidbody>();
        terrainLayer = LayerMask.NameToLayer("Terrain");
        shipRadius = transform.lossyScale.z;
    }

    void Start() {
        Time.timeScale = 1f;
        slider.value = 100;
        audioData = GetComponent<AudioSource>();
        m_rigidbody.drag = 0.5f;
        m_rigidbody.angularDrag = 0.5f;
        tempstir = Time.time;
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
        inputActions.Player.Fire.started += Fire;
        inputActions.Player.Fire.canceled += StopFire;
        inputActions.Player.Pause.performed += _ => pauseMenu.Pause();
    }

    void OnDisable() {
        inputActions.Disable();
    }

    void OnDrawGizmos() {
        _OnDrawGizmos?.Invoke();
    }
    event Action _OnDrawGizmos;

    void LateUpdate() {
        Transform ctransform = m_camera.transform;

        // Quaternion orientation = Quaternion.LookRotation(gravity, -gravity);
        // ctransform.rotation = orientation;
        ctransform.LookAt(transform, ctransform.up);
    }

    Quaternion _rotateToGround;
    void FixedUpdate() {
        _OnDrawGizmos = null;
        Vector3 gravity = (planet.transform.position - transform.position).normalized;
        Vector3 groundDirection = gravity; // TODO shift it with velocity

        Ray groundRay = new Ray(transform.position, groundDirection);
        RaycastHit groundHit;
        if (Physics.Raycast(groundRay, out groundHit, Mathf.Infinity, ~terrainLayer)) {
            float hoverDelta = altitude - groundHit.distance;
            Vector3 velocity = m_rigidbody.velocity;
            float upwardSpeed = (transform.worldToLocalMatrix * velocity).y;
            float lift = hoverDelta * Mathf.Pow(hoverForce, 2);
            lift += (1 * Mathf.Sign(hoverDelta)) * hoverAmplitude * hoverSinusoidal((Time.fixedTime % hoverPeriod) / hoverPeriod).y / lift;
            // Debug.Log($"lift {lift} velocity.y {velocity.y} upwardSpeed {upwardSpeed}");
            m_rigidbody.AddForce(lift * -groundDirection, ForceMode.Acceleration);
        }

        Vector3 transformUp = transform.up;
        Vector3 forwardShift = Vector3.Cross(groundDirection, transform.right).normalized;
        // Debug.Log("dot gravity transformDown" + Vector3.Dot(gravity, transformUp));
        // Debug.DrawRay(transform.position, transform.forward, Color.red);
        // Debug.DrawRay(transform.position, forwardShift, Color.green);
        forwardShift *= Mathf.Sign(Vector3.Dot(transform.forward, forwardShift)) * shipRadius;

        Vector3 groundNormal = transform.GetGroundNormal(planet, groundDirection, terrainLayer);


        Plane gravityPlane = new Plane(-gravity, transform.position);
        Debug.DrawRay(transform.position, groundNormal, Color.cyan);
        Quaternion rotateToGround = Quaternion.FromToRotation(transform.up, groundNormal);
        rotateToGround = _rotateToGround = Quaternion.SlerpUnclamped(_rotateToGround, rotateToGround, 30 * Time.fixedDeltaTime);
        // Debug.Log("groundNormal " + groundNormal + " magnitude " + groundNormal.magnitude + " rotateToGround " + rotateToGround);

        Vector3 _mouseDirection = Vector3.zero;
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray mouseRay = m_camera.ScreenPointToRay(mousePosition);

        // Debug.Log("dot gravity groundNormal: " + Vector3.Dot(gravity, groundNormal));
        float enter;
        if (gravityPlane.Raycast(mouseRay, out enter)) {
            Vector3 hitPoint = mouseRay.GetPoint(enter);
            _mouseDirection = (hitPoint - transform.position).normalized;

            _OnDrawGizmos += () => {
                // Gizmos.DrawSphere(hitPoint, 0.1f);
                // Gizmos.color = Color.cyan;
                // CGizmos.DrawPlane(gravityPlane, transform.position, 0.25f * Vector3.one);
            };
        }

        Plane normalPlane = new Plane(groundNormal, transform.position);
        Vector3 mouseDirection = Vector3.ProjectOnPlane(_mouseDirection, groundNormal).normalized;
        _OnDrawGizmos += () => {
            // Gizmos.color = Color.magenta;
            // CGizmos.DrawPlane(normalPlane, transform.position, 0.25f * Vector3.one);
            // Gizmos.color = Color.black;
            // Gizmos.DrawRay(transform.position, _mouseDirection);
            // Gizmos.color = Color.yellow;
            // Gizmos.DrawRay(transform.position, mouseDirection);
        };
        Quaternion rotateToMouseDirection = Quaternion.FromToRotation(transform.forward, mouseDirection);
        // Debug.Log($"unit length: rotateToMouseDirection {rotateToMouseDirection} rotateToGround {rotateToGround}");
        m_rigidbody.MoveRotation(
            rotateToMouseDirection.normalized *
            rotateToGround.normalized *
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
        float kvelocity = Tooling.GetBezierCubicPoint(Mathf.Clamp01(t), speedCurve).y;
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
        Resolution resolution = Screen.currentResolution;
        Ray forwardRay = m_camera.ScreenPointToRay(new Vector2(resolution.width / 2, resolution.height));

        if (gravityPlane.Raycast(forwardRay, out enter)) {
            Vector3 hitPoint = forwardRay.GetPoint(enter);
            Vector3 mouseForward = (hitPoint - transform.position).normalized;

            Vector3 moveDirection = Quaternion.FromToRotation(transform.forward, mouseForward) * transform.TransformDirection(_moveDirection.x, 0, _moveDirection.y);

            const float C = 100;
            m_rigidbody.velocity = C * moveDirection * kvelocity * moveSpeed * Time.deltaTime;
        }
        m_camera.transform.position = transform.position + 25 * -gravity;
    }

    void OnCollisionEnter(Collision col) {

        if (col.gameObject.tag == "Enemy") {
            //Debug.Log("VIE : " + life);
            //GameObject imageObject = GameObject.FindGameObjectWithTag("life" + lives);
            //imageObject.SetActive(false);
            life -= 10;
            //bruit de dégats
            if (life == 0) {
                GameOverCanvas.SetActive(true);
                Time.timeScale = 0f;
                gameObject.SetActive(false);
                //bruit de destruction
            }
            slider.value = life;
        }
    }

    void Move(InputAction.CallbackContext context) {
        lastMoves.Enqueue(new _CallbackContext(context));
    }

    Coroutine CoroutineFire;
    Coroutine CoroutineFlash;

    void Fire(InputAction.CallbackContext context) {
        if (Time.timeScale == 0) return;
        CoroutineFlash = StartCoroutine(FlashNow());
        CoroutineFire = StartCoroutine(ShotNow());
    }

    void StopFire(InputAction.CallbackContext context) {
        if (Time.timeScale == 0) return;
        StopCoroutine(CoroutineFire);
        StopCoroutine(CoroutineFlash);
        flash.intensity = 0;
    }

    IEnumerator FlashNow() {
        float waitTime = fireRate / 2;
        yield return new WaitForSeconds(fireRate - (Time.time - tempstir));
        // Get half of the seconds (One half to get brighter and one to get darker
        while (true) {
            flash.intensity = 1;
            yield return new WaitForSeconds(waitTime);
            flash.intensity = 0;
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator ShotNow() {
        float decalage = 0.5f;
        float difftime = fireRate - (Time.time - tempstir);
        float largeur = (bulletnumber - 1) * decalage;
        yield return new WaitForSeconds(difftime);

        while (true) {
            for (int i = 0; i < bulletnumber; i++) {
                GameObject bullet = bulletPool.GetObject();
                bullet.transform.position = transform.TransformPoint(new Vector3((float)i / bulletnumber * largeur - largeur / 2, 0, 0));
                bullet.transform.rotation = Quaternion.identity;
                bullet.GetComponent<Bullet>().Initialize(transform.forward * 50);
                bullet.SetActive(true);
                tempstir = Time.time;
                audioData.Play();
            }
            yield return new WaitForSeconds(fireRate);
        }
    }
}
