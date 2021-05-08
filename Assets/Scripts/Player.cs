using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed;

    public Camera m_camera;
    public Camera Camera {
        get {
            return m_camera;
        }
    }
    Quaternion cameraOrientation;

    public Light flash;
    public float flashTotalSeconds;
    public float flashMaxIntensity;

    Rigidbody m_rigidbody;
    PlayerControls inputActions;
    public Vector2 move { get; private set; }

    void Awake() {
        m_rigidbody = GetComponent<Rigidbody>();
        cameraOrientation = Quaternion.Euler(90, 0, 0);
    }

    void OnEnable() {
        inputActions = new PlayerControls();
        inputActions.Enable();
        inputActions.Player.Move.performed += Move;
    }

    void OnDisable() {
        inputActions.Disable();
    }

    void Move(InputAction.CallbackContext context) {
        move = context.ReadValue<Vector2>();
    }

    Vector2? prevMousePosition = null;

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
        /*if (Input.GetButton("Fire1"))
        {
            StartCoroutine("flashNow");
            Debug.Log("feu");
            //Bullets.Instance.SpawnFromPool("Cube", transform.position, Quaternion.identity);
        }*/
    }

    void FixedUpdate() {
        m_rigidbody.MovePosition(m_rigidbody.position
            + transform.TransformDirection(move.x, 0, move.y) * moveSpeed * Time.deltaTime);
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
