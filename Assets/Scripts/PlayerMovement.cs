using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //player attributes
    [SerializeField] private float moveSpeed = 40;
    [SerializeField] private float jumpHeight = 40;

    private Rigidbody2D rb;
    private PlayerInput playerInput;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform pointOfGroundContact;
    [SerializeField] private float groundCheckRadius = 0.1f;

    private event EventHandler OnJumpAction;

    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [SerializeField] private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = new PlayerInput();
        playerInput.Enable();
        playerInput.Player.Jump.performed += Jump_performed;
        OnJumpAction += PlayerMovement_OnJumpAction;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleJump();
        Move(playerInput.Player.Move.ReadValue<Vector2>());
    }

    private void Move(Vector2 inputVector)
    {
        transform.position += moveSpeed * Time.deltaTime * new Vector3 (inputVector.x, inputVector.y);
    }

    private void PlayerMovement_OnJumpAction(object sender, EventArgs e)
    {
        UpdateJumpBuffer();
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnJumpAction?.Invoke(this, EventArgs.Empty);
    }

    private void HandleJump()
    {
        UpdateCoyoteTime();

        jumpBufferCounter -= Time.deltaTime;

        bool canPerformJump = coyoteTimeCounter > 0.0f && jumpBufferCounter > 0.0f;

        if (canPerformJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            jumpBufferCounter = 0;
        }
    }
    private void UpdateCoyoteTime()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (playerInput.Player.Jump.WasReleasedThisFrame())
        {
            coyoteTimeCounter = 0;
        }
    }
    private void UpdateJumpBuffer()
    {
        jumpBufferCounter = jumpBufferTime;
    }
    private bool IsGrounded()
    {
        bool isGrounded = Physics2D.OverlapCircle(pointOfGroundContact.position, groundCheckRadius, groundLayer);
        return isGrounded;
    }
}
