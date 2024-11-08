using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Camera controller reference.
    [Space(10)]
    public Animator animator;
    public CameraController cameraController;
    public CharacterController characterController;

    // Ground check settings.
    [Space(10)]
    public float groundCheckRadius = 0.2f;
    public Vector3 groundCheckOffset;
    public LayerMask groundLayer;

    // Movement and rotation settings.
    [Space(10)]
    public float moveSpeed = 5f;
    public float rotationSpeed = 500f;

    // Player control state.
    private bool hasControl = true;

    // Internal state for ground detection and vertical speed.
    private bool isGrounded;
    private float ySpeed;

    // Target rotation for smoothing.
    private Quaternion targetRotation;

    // Update is called once per frame.
    private void FixedUpdate()
    {
        // Get horizontal and vertical input.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement magnitude.
        float moveAmount = Mathf.Clamp01(Math.Abs(horizontal) + Math.Abs(vertical));

        // Create normalized movement vector.
        var moveInput = (new Vector3(horizontal, 0, vertical)).normalized;

        // Apply camera's planar rotation to movement.
        var moveDirection = cameraController.PlanarRotation * moveInput;

        // Check if player control is disabled.
        if (!hasControl)
        {
            return;
        }

        // Check if the player is on the ground.
        GroundCheck();
        if (isGrounded)
        {
            // Reset ySpeed to keep the player grounded.
            ySpeed = -0.5f;
        }
        else
        {
            // Apply gravity.
            ySpeed += Physics.gravity.y * Time.fixedDeltaTime;
        }

        // Calculate final velocity.
        var velocity = moveDirection * moveSpeed;
        velocity.y = ySpeed;

        // Move the player.
        characterController.Move(velocity * Time.fixedDeltaTime);

        // If there is movement input, move the player.
        if (moveAmount > 0)
        {
            // Rotate the player to face movement direction.
            targetRotation = Quaternion.LookRotation(moveDirection);
        }

        // Smooth rotation to target.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Update animator's moveAmount for blending.
        animator.SetFloat("moveAmount", moveAmount, 0.2f, Time.deltaTime);
    }

    // Set player control state.
    public void SetControl(bool hasControl)
    {
        // Update control state and character controller activation.
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        // Reset animator and rotation when control is disabled.
        if (!hasControl)
        {
            animator.SetFloat("moveAmount", 0f);
            targetRotation = transform.rotation;
        }
    }

    // Check if the player is grounded.
    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    // Draws a gizmo in the editor to visualize the ground check area.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    public float RotationSpeed => rotationSpeed;
}
