using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    private const float WALK_ANIMATION_SPEED = 0.5f;
    private const float RUN_ANIMATION_SPEED = 1f;
    private const float IN_AIR_THRESHOLD = 0.5f;

    [SerializeField] private Animator animator;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private float acceleration;

    private PlayerInputActions playerInputActions;

    private Vector2 movementValue;
    private float horizontal;
    private float vertical;
    private float inAirThresholdTimer;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Movement.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (!groundCheck.isGrounded)
            inAirThresholdTimer = inAirThresholdTimer + Time.deltaTime;

        if(inAirThresholdTimer >= IN_AIR_THRESHOLD)
        {
            inAirThresholdTimer = 0;
            PlayerInAir();
        }
    }

    private void Movement()
    {
        movementValue = playerInputActions.Player.Movement.ReadValue<Vector2>();
        float horizontalLimit = (Mathf.Abs(movementValue.x) <= 0.5f) ? WALK_ANIMATION_SPEED * Mathf.Abs(movementValue.x) * 2 : RUN_ANIMATION_SPEED * Mathf.Abs(movementValue.x);
        float verticalLimit = (Mathf.Abs(movementValue.y) <= 0.5f) ? WALK_ANIMATION_SPEED * Mathf.Abs(movementValue.y) * 2 : RUN_ANIMATION_SPEED * Mathf.Abs(movementValue.y);

        if (!groundCheck.isGrounded)
            movementValue.x = movementValue.y = 0;

        if (movementValue.x != 0)
            horizontal = Mathf.Clamp(horizontal + movementValue.x * acceleration * Time.deltaTime, -horizontalLimit, horizontalLimit);
        else
        {
            if (horizontal != 0)
            {
                horizontal = (horizontal > 0) ? horizontal - acceleration * Time.deltaTime : horizontal + acceleration * Time.deltaTime;
                horizontal = (Mathf.Abs(horizontal) < 0.1f) ? 0 : horizontal;
            }
        }

        if (movementValue.y != 0)
            vertical = Mathf.Clamp(vertical + movementValue.y * acceleration * Time.deltaTime, -verticalLimit, verticalLimit);
        else
        {
            if (vertical != 0)
            {
                vertical = (vertical > 0) ? vertical - acceleration * Time.deltaTime : vertical + acceleration * Time.deltaTime;
                vertical = (Mathf.Abs(vertical) < 0.1f) ? 0 : vertical;
            }
        }

        animator.SetFloat(Properties.ANIMATOR_VARIABLE_HORIZONTAL, horizontal);
        animator.SetFloat(Properties.ANIMATOR_VARIABLE_VERTICAL, vertical);
    }

    public void Jump(InputAction.CallbackContext callbackContext)
    {
        if (groundCheck.isGrounded && callbackContext.performed)
            PlayerInAir();
    }

    public void PlayerInAir()
    {
        animator.SetBool(Properties.ANIMATOR_BOOL_IS_GROUNDED, false);
    }

    public void PlayerLanded()
    {
        animator.SetBool(Properties.ANIMATOR_BOOL_IS_GROUNDED, true);
    }
}