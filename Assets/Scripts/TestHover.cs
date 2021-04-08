using UnityEngine;
using UnityEngine.InputSystem;

public class TestHover : MonoBehaviour
{
    // planet along which is oriented the ship
    public GameObject planet;

    // It moves/levitates by measuring the distance to ground with a
    // raycast then applying a force that decreases as the object
    // reaches the desired levitation height.

    // Vary the parameters below to get different control effects.
    // For example, reducing the hover damping will tend to make the object bounce
    // if it passes over an object underneath.
    float moveForce = 20.0f;
    float rotateTorque = 10.0f;
    float hoverHeight = 2.0f;
    float hoverForce = 5.0f;
    // The amount that the lifting force is reduced per unit of upward speed.
    // This damping tends to stop the object from bouncing after passing over
    // something.
    float hoverDamp = 0.5f;

    Rigidbody rb;
    PlayerMovementScript playerMovementScript;

    public float moveSpeed;

    void Start() {
        rb = GetComponent<Rigidbody>();
        playerMovementScript = GetComponent<PlayerMovementScript>();
        rb.drag = 0.5f;
        rb.angularDrag = 0.5f;
    }

    void FixedUpdate() {
        Quaternion rotationSprite = Quaternion.Euler(90, 180, 0);

        RaycastHit hit;
        Ray downRay = new Ray(transform.position, -transform.up);
        Debug.DrawRay(transform.position, -transform.up, Color.green);
        if (Physics.Raycast(downRay, out hit)) {
            transform.rotation *= Quaternion.FromToRotation(transform.up, hit.normal);
            Debug.Log(hit.normal);
            float hoverDelta = hoverHeight - hit.distance;
            if (hoverDelta > 0) {
                // Subtract the damping from the lifting force and apply it to the rigidbody.
                float upwardSpeed = rb.velocity.y;
                float lift = hoverDelta * hoverForce - upwardSpeed * hoverDamp;
                rb.AddForce(lift * transform.up);
            } else if (hoverDelta < 0) {
                // Subtract the damping from the lifting force and apply it to
                // the rigidbody.
                float upwardSpeed = rb.velocity.y;
                float lift = hoverDelta * hoverForce - upwardSpeed * hoverDamp;
                rb.AddForce(-transform.up * 10);
            }
        }
        Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane p = new Plane(Vector3.up, transform.position);
        if (p.Raycast(mouseRay, out float hitDist)) {
            Vector3 hitPoint = mouseRay.GetPoint(hitDist);
            hitPoint = new Vector3(hitPoint.x, transform.position.y, hitPoint.z);
            transform.LookAt(hitPoint, transform.up);
        }
        Vector2 move = playerMovementScript.move;
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(move.x, 0, move.y) * moveSpeed * Time.deltaTime);
    }

    protected void LateUpdate() {
        Camera.main.transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles.x, 0, Camera.main.transform.rotation.eulerAngles.z);
    }
}


// CORBEILLE
////rotation selon le centre de l'objet donné
//Vector3 normal = (transform.position - planet.transform.position).normalized;
//Quaternion rotationorm = Quaternion.LookRotation(normal);
////Debug.Log(this.transform.rotation.eulerAngles.x);
//if(this.transform.rotation.eulerAngles.x < 90)
//{
//    this.transform.localEulerAngles = new Vector3(rotationorm.eulerAngles.x + 90, rotationorm.eulerAngles.y, rotationorm.eulerAngles.z);
//}
//else if(this.transform.rotation.eulerAngles.x >= 90)
//{
//    // TODO : PLANTAGE QUAND EQUATEUR PASSE (clignotement toutes les frames échange entre 2 valeurs)
//    // AUTRE CHOSE : Rotation sur l'axe x du sprite n'est pas le bon
//    Debug.Log("PASSE !");
//    this.transform.localEulerAngles = new Vector3(rotationorm.eulerAngles.x+90, this.transform.localEulerAngles.y, this.transform.localEulerAngles.z);
//}


// Push/turn the object based on arrow key input.
//rb.AddForce(Input.GetAxis("Vertical") * moveForce * transform.forward);
//rb.AddTorque(Input.GetAxis("Horizontal") * rotateTorque * Vector3.up);