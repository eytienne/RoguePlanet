using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    PlayerControls inputActions;

    public struct Parameters
    {
        public const string WALK = "walk";
    }

    void Start() {
        animator = GetComponent<Animator>();
    }

    public void Move(InputAction.CallbackContext context) {
        Vector2 move = context.ReadValue<Vector2>();
        switch (move.y)
        {
            case 1:
                animator.Play("Walk", 0);
                break;
            case 0:
                animator.Play("Idle.Idle01", 0);
                break;
            default:
                break;
        }
    }
}
