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

    Vector2 move;

    void Move(InputAction.CallbackContext context) {
        move = context.ReadValue<Vector2>();
    }

    void Jump(InputAction.CallbackContext context) {
        animator.CrossFadeInFixedTime("Jump", 0.1f);
    }

    void FixedUpdate() {
        float moveX = animator.GetFloat("moveX");
        float moveY = animator.GetFloat("moveY");
        animator.SetFloat("moveX", Mathf.Lerp(moveX, move.x, 25 * Time.fixedDeltaTime));
        animator.SetFloat("moveY", Mathf.Lerp(moveY, move.y, 25 * Time.fixedDeltaTime));

        Vector3 gravityUp = (body.position - planet.transform.position).normalized;
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 50 * Time.fixedDeltaTime);
    }


}
