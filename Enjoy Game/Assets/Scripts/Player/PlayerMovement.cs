using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private float acceleration;
    [SerializeField] private float speed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpTime;

    private PlayerInputActions playerInputActions;

    private Vector2 movementValue;
    private Vector3 move;
    private bool jump;
    private float jumpTimer;
    private float horizontal;
    private float vertical;
    private float horizontalSpeed;
    private float verticalSpeed;

    public bool isMoving { get; private set; }

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Movement.Enable();
    }

    private void Update()
    {
        movementValue = playerInputActions.Player.Movement.ReadValue<Vector2>();
        //Move();
        if(groundCheck.isGrounded)
            Movement();

        if(jump)
            PerformJump();
        else
            Gravity();
    }

    private void Movement()
    {
        isMoving = (movementValue != Vector2.zero && groundCheck.isGrounded);
        horizontalSpeed = (Mathf.Abs(movementValue.x) <= 0.5f) ? speed * Mathf.Abs(movementValue.x) : speed * Mathf.Abs(movementValue.x) * 2;
        verticalSpeed = (Mathf.Abs(movementValue.y) <= 0.5f) ? speed * Mathf.Abs(movementValue.y) : speed * Mathf.Abs(movementValue.y) * 2;

        if (movementValue.x != 0)
            horizontal = Mathf.Clamp(horizontal + movementValue.x * acceleration * Time.deltaTime, -horizontalSpeed, horizontalSpeed);
        else
        {
            if (horizontal != 0)
            {
                horizontal = (horizontal > 0) ? horizontal - 2 * acceleration * Time.deltaTime : horizontal + 2 * acceleration * Time.deltaTime;
                horizontal = (Mathf.Abs(horizontal) < 0.25f) ? 0 : horizontal;
            }
        }

        if (movementValue.y != 0)
            vertical = Mathf.Clamp(vertical + movementValue.y * acceleration * Time.deltaTime, -verticalSpeed, verticalSpeed);
        else
        {
            if (vertical != 0)
            {
                vertical = (vertical > 0) ? vertical - 2 * acceleration * Time.deltaTime : vertical + 2 * acceleration * Time.deltaTime;
                vertical = (Mathf.Abs(vertical) < 0.25f) ? 0 : vertical;
            }
        }

        move.x = horizontal;
        move.z = vertical;
        move = transform.right * move.x + transform.forward * move.z;

        characterController.Move(move * Time.deltaTime);

        if (isMoving)
            transform.eulerAngles = Vector3.up * mainCamera.transform.eulerAngles.y;
    }

    private void Gravity()
    {
        move.y = move.y + gravity * Time.deltaTime * Time.deltaTime;

        if (groundCheck.isGrounded)
            move.y = -0.2f;

        characterController.Move(Vector3.up * move.y);
    }

    public void Jump(InputAction.CallbackContext callbackContext)
    {
        if (groundCheck.isGrounded && callbackContext.performed)
        {
            jump = true;
            jumpTimer = 0;
        }
    }

    private void PerformJump()
    {
        jumpTimer = (jumpTimer >= jumpTime) ? jumpTime : jumpTimer + Time.deltaTime;
        characterController.Move((move.x * Vector3.right + move.z * Vector3.forward + Vector3.up * jumpHeight * jumpForce * (1 - jumpTimer / jumpTime)) * Time.deltaTime);

        if (jumpTimer >= jumpTime)
        {
            jump = false;
            //playerAnimation.ResetJumpTrigger();
        }
    }

    private void Move()
    {
        move = movementValue.x * transform.right + movementValue.y * transform.forward;
        characterController.Move(move * speed * Time.deltaTime);
    }
}