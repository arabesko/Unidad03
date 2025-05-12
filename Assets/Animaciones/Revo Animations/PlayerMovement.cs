using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
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
    public float pct;
    public float walkAnimValueTransition = 0.1f;

    [SerializeField] AnimatorBasic _animatorBasic;


    [HideInInspector] public CharacterController Controller;
    [HideInInspector] public bool EnableMovement = true;
    [HideInInspector] public bool IsDashing = false;
    public bool IsGrounded => Controller.isGrounded;

    private Vector3 velocity;
    private float coyoteCounter, jumpBufferCounter;
    private float currentSpeed;
    private bool isSprinting;

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleTimers();

        if (EnableMovement)
        {
            HandleMovement();
            HandleGravityAndJump();
        }
        else if (!IsGrounded)
        {
            // Solo aplicar gravedad si está en el aire
            velocity.y += gravity * Time.deltaTime;
            Controller.Move(velocity * Time.deltaTime);
        }

        UpdateAnimation();
    }

    private void HandleTimers()
    {
        if (Controller.isGrounded)
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
        Vector3 dir = GetCameraRelativeDirection(h, v);

        if (dir.magnitude >= 0.1f)
        {
            Quaternion target = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);

            isSprinting = Input.GetKey(KeyCode.LeftShift);
            currentSpeed = moveSpeed * (isSprinting ? sprintMultiplier : 1f);
            Controller.Move(dir.normalized * currentSpeed * Time.deltaTime);
        }
        else
        {
            currentSpeed = 0f;
            isSprinting = false;
        }
    }

    private Vector3 GetCameraRelativeDirection(float h, float v)
    {
        var cam = Camera.main.transform;
        Vector3 f = cam.forward; f.y = 0; f.Normalize();
        Vector3 r = cam.right; r.y = 0; r.Normalize();
        return f * v + r * h;
    }

    private void HandleGravityAndJump()
    {
        if (Controller.isGrounded && velocity.y <= 0f)
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
        Controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateAnimation()
    {
        pct = (currentSpeed > 0f) ? (isSprinting ? 1f : walkAnimValueTransition) : 0f;
    }
}
