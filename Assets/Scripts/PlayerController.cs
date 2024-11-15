using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //
    [Space(10)]
    public Animator animator;
    public CameraController cameraController;
    public CharacterController characterController;
    public EnvironmentScanner environmentScanner;

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

    // Variables for desired movement direction, actual movement, and velocity.
    private Vector3 desiredMoveDirection;
    private Vector3 moveDirection;
    private Vector3 velocity;

    // Internal state for ground detection and vertical speed.
    private bool isGrounded;
    private float ySpeed;

    // Target rotation for smoothing.
    private Quaternion targetRotation;

    // Properties to track if the player is on a ledge and store ledge data.
    public bool IsOnLedge { get; set; }
    public LedgeData LedgeData { get; set; }

    // Update is called once per frame.
    private void FixedUpdate()
    {
        // Get horizontal and vertical input.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement magnitude.
        float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        // Create normalized movement vector.
        var moveInput = (new Vector3(horizontal, 0, vertical)).normalized;

        // Apply camera's planar rotation to movement.
        desiredMoveDirection = cameraController.PlanarRotation * moveInput;
        moveDirection = desiredMoveDirection;

        // Check if player control is disabled.
        if (!hasControl)
        {
            return;
        }

        // Reset velocity to stop player movement.
        velocity = Vector3.zero;

        // Check if the player is on the ground.
        GroundCheck();
        animator.SetBool("isGrounded", isGrounded);
        if (isGrounded)
        {
            // Reset ySpeed to keep the player grounded.
            ySpeed = -0.5f;
            velocity = moveDirection * moveSpeed;

            // Check if the player is on a ledge and handle ledge movement if detected.
            IsOnLedge = environmentScanner.LedgeCheck(moveDirection, out LedgeData ledgeData);
            if (IsOnLedge)
            {
                LedgeData = ledgeData;
                LedgeMovement();
            }

            // Update animator's moveAmount for blending.
            animator.SetFloat("moveAmount", velocity.magnitude / moveSpeed, 0.2f, Time.deltaTime);
        }
        else
        {
            // Apply gravity.
            ySpeed += Physics.gravity.y * Time.fixedDeltaTime;

            velocity = transform.forward * moveSpeed / 2;
        }

        // Calculate final velocity.
        velocity.y = ySpeed;

        // Move the player.
        characterController.Move(velocity * Time.fixedDeltaTime);

        // If there is movement input, move the player.
        if (moveAmount > 0 && moveDirection.magnitude > 0.2f)
        {
            // Rotate the player to face movement direction.
            targetRotation = Quaternion.LookRotation(moveDirection);
        }

        // Smooth rotation to target.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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

    //
    public bool HasControl
    {
        get => hasControl;
        set => hasControl = value;
    }

    // Check if the player is grounded.
    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    // Handles movement when the player is on a ledge.
    private void LedgeMovement()
    {
        // Calculate the signed angle between the surface normal and the desired movement direction.
        float signedAngle = Vector3.SignedAngle(LedgeData.surfaceHit.normal, desiredMoveDirection, Vector3.up);
        float angle = Mathf.Abs(signedAngle);

        //
        if (Vector3.Angle(desiredMoveDirection, transform.forward) >= 80)
        {
            // Stop movement if the player tries to move backwards off the ledge.
            velocity = Vector3.zero;
            return;
        }

        // Prevent movement if the angle is too small (near straight-on).
        if (angle < 60)
        {
            velocity = Vector3.zero;
            moveDirection = Vector3.zero;
        }
        else if (angle < 90)
        {
            // Allow movement along the ledge.
            var left = Vector3.Cross(Vector3.up, LedgeData.surfaceHit.normal);
            var direction = left * Mathf.Sign(signedAngle);

            velocity = velocity.magnitude * direction;
            moveDirection = direction;
        }
    }

    // Draws a gizmo in the editor to visualize the ground check area.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    public float RotationSpeed => rotationSpeed;
}
