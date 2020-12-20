using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Animator animator;
    PlayerControls inputActions;
    [SerializeField]
    RuntimeAnimatorController runtimeAnimatorController;
    public Planet planet;
    Rigidbody body;

    void Start() {
        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = runtimeAnimatorController;
        inputActions = new PlayerControls();
        inputActions.Enable();
        inputActions.Player.Move.performed += Move;
        inputActions.Player.Move.canceled += Move;
        inputActions.Player.Jump.performed += Jump;
        body = GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezeRotation;
    }
    void DebugEvent(InputAction.CallbackContext context) {
        Debug.Log(context.phase + " " + context.ReadValueAsObject());
    }

    void OnDestroy() {
        inputActions.Disable();
    }

    void FixedUpdate() {
        Vector3 gravityUp = (body.position - planet.transform.position).normalized;
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 50 * Time.fixedDeltaTime);
    }

    void Move(InputAction.CallbackContext context) {
        Vector2 move = context.ReadValue<Vector2>();
        animator.SetFloat("moveX", move.x);
        animator.SetFloat("moveY", move.y);
    }

    void Jump(InputAction.CallbackContext context) {
        animator.SetTrigger("jump");
    }
}
