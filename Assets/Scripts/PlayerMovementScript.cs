using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementScript : MonoBehaviour
{
    public float moveSpeed;

    private Quaternion pos;
    public GameObject plane;
    public Light flash;
    public float flashTotalSeconds;
    public float flashMaxIntensity;

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

        Debug.Log(waitTime + flash.intensity);
    }

    PlayerControls inputActions;
    public Vector2 move { get; private set; }

    void Start() {
        inputActions = new PlayerControls();
        inputActions.Enable();
        inputActions.Player.Move.performed += Move;
    }

    void Move(InputAction.CallbackContext context) {
        move = context.ReadValue<Vector2>();
    }


    void Update() {
        Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane p = new Plane(Vector3.up, transform.position);
        if (p.Raycast(mouseRay, out float hitDist)) {
            Vector3 hitPoint = mouseRay.GetPoint(hitDist);
            hitPoint = new Vector3(hitPoint.x, this.transform.position.y, hitPoint.z);
            plane.transform.LookAt(hitPoint);
            flash.transform.LookAt(hitPoint);
            //plane.transform.LookAt(new Vector3(-hitPoint.x, hitPoint.y, -hitPoint.z));
        }
        /*if (Input.GetButton("Fire1"))
        {
            StartCoroutine("flashNow");
            Debug.Log("feu");
            //Bullets.Instance.SpawnFromPool("Cube", transform.position, Quaternion.identity);
        }*/
    }

    void FixedUpdate() {
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position
            + transform.TransformDirection(move.x, 0, move.y) * moveSpeed * Time.deltaTime);
    }
    protected void LateUpdate() {
        plane.transform.localEulerAngles = new Vector3(0, plane.transform.localEulerAngles.y, 0);
    }
}
