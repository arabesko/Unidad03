using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float rotationSpeed = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.1f;

    [Header("Animation Settings")]
    [Range(0f, 1f)]
    public float walkAnimValueTransition = 0.3f;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float currentSpeed;
    private bool isSprinting;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleTimers();
        HandleMovement();
        HandleGravityAndJump();
        UpdateAnimation();
    }

    private void HandleTimers()
    {
        if (controller.isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    private void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDir = GetCameraRelativeDirection(h, v);

        if (moveDir.magnitude >= 0.1f)
        {
            // Rotación hacia la dirección deseada
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Determinar sprint
            isSprinting = Input.GetKey(KeyCode.LeftShift);
            currentSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);

            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);
        }
        else
        {
            currentSpeed = 0f;
            isSprinting = false;
        }
    }

    private Vector3 GetCameraRelativeDirection(float h, float v)
    {
        Transform cam = Camera.main.transform;
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        return camForward * v + camRight * h;
    }

    private void HandleGravityAndJump()
    {
        if (controller.isGrounded && velocity.y <= 0f)
        {
            velocity.y = -2f;

            if (jumpBufferCounter > 0f && coyoteCounter > 0f)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                jumpBufferCounter = 0f;
                coyoteCounter = 0f;
            }
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateAnimation()
    {
        if (animator != null)
        {
            float velPercent = 0f;
            if (currentSpeed > 0f)
                velPercent = isSprinting ? 1f : walkAnimValueTransition;
            animator.SetFloat("Velocity", velPercent, 0.1f, Time.deltaTime);
        }
    }
}
