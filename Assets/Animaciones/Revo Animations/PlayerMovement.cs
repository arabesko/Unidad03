using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, IDamagiable
{
    [Header("Player")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float rotationSpeed = 10f;

    [Header("Levitation Settings")]
    public float levitationAmplitude = 0.2f;
    public float levitationFrequency = 1f;
    public Vector2 levitationHeightRange = new Vector2(0.5f, 2f);
    public float maxDistanceFromPlayer = 3f;
    public float levitationRotationSpeed = 30f;
    private float levitationOffset = 0f;

    [SerializeField] private float _viewRadius;
    [SerializeField] private float _viewAngle;
    public List<GameObject> colectables;
    [SerializeField] private LayerMask _wallLayer;

    [SerializeField] private GameObject _elementDetected;
    [SerializeField] private GameObject _weaponSelected;

    [SerializeField] private GameObject _elementLevitated;
    public GameObject ElementLevitated { get { return _elementLevitated; } }

    [SerializeField] private Transform _levitationPoint;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.1f;

    [Header("Animation Settings")]
    public float pct;
    public float walkAnimValueTransition = 0.1f;

    [SerializeField] public AnimatorBasic _animatorBasic;

    [Header("Inventory")]
    [SerializeField] private GameObject _element0;
    [SerializeField] private Inventory _inventory;

    [HideInInspector] public CharacterController Controller;
    [HideInInspector] public bool EnableMovement = true;
    [HideInInspector] public bool IsDashing = false;

    [SerializeField] private bool _isInvisible = false;
    public bool IsInvisible
    {
        get { return _isInvisible; }
        set { _isInvisible = value; }
    }

    public List<MeshRenderer> bodyRender;
    [SerializeField] private Transform _projectorPosition;
    [SerializeField] private Transform _module1;

    public bool IsGrounded => Controller.isGrounded;

    private Vector3 velocity;
    private float coyoteCounter, jumpBufferCounter;
    private float currentSpeed;
    private bool isSprinting;

    [SerializeField] private bool _canWeaponChange = true;
    public bool CanWeaponChange
    {
        get { return _canWeaponChange; }
        set { _canWeaponChange = value; }
    }

    void Awake()
    {
        Controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        _inventory = new Inventory(8, _element0);
        AddModules(_projectorPosition);
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
            velocity.y += gravity * Time.deltaTime;
            Controller.Move(velocity * Time.deltaTime);
        }

        UpdateAnimation();

        if (Input.GetKeyDown(KeyCode.E) && CollectWeapon() && CanWeaponChange)
        {
            Weapon myWeapon = _elementDetected.GetComponent<Weapon>();
            if (myWeapon != null) AddModules(_module1);
        }

        // Levitar objetos
        if (Input.GetKeyDown(KeyCode.R) && CollectWeapon() && _elementLevitated == null)
        {
            _elementLevitated = _elementDetected;
            IPuzzlesElements myPuzzle = _elementLevitated.GetComponent<IPuzzlesElements>();
            if (myPuzzle == null)
            {
                _elementLevitated = null;
                return;
            }

            Rigidbody rb = _elementLevitated.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false; // DESACTIVA GRAVEDAD
                rb.isKinematic = false;
                rb.freezeRotation = true;
            }

            levitationOffset = 0f;
            myPuzzle.Activate();
        }
        // Soltar objeto
        else if (Input.GetKeyDown(KeyCode.R) && _elementLevitated != null)
        {
            NoLevitate();
        }

        // Manejo del objeto levitado
        if (_elementLevitated != null)
        {
            HandleLevitatingObject();
        }

        if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Q)) && CanWeaponChange)
        {
            CanWeaponChange = false;
            _weaponSelected.GetComponent<IModules>().PowerElement();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && CanWeaponChange)
        {
            SelectModule(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && CanWeaponChange)
        {
            SelectModule(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && CanWeaponChange)
        {
            SelectModule(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) && CanWeaponChange)
        {
            SelectModule(3);
        }
    }

    private void HandleLevitatingObject()
    {
        levitationOffset += Time.deltaTime * levitationFrequency;
        float yOffset = Mathf.Sin(levitationOffset) * levitationAmplitude;

        Vector3 targetPosition = _levitationPoint.position + new Vector3(0, yOffset, 0);

        float minHeight = transform.position.y + levitationHeightRange.x;
        float maxHeight = transform.position.y + levitationHeightRange.y;
        targetPosition.y = Mathf.Clamp(targetPosition.y, minHeight, maxHeight);

        Vector3 horizontalDirection = targetPosition - transform.position;
        horizontalDirection.y = 0;

        if (horizontalDirection.magnitude > maxDistanceFromPlayer)
        {
            horizontalDirection = horizontalDirection.normalized * maxDistanceFromPlayer;
            targetPosition = transform.position + horizontalDirection;
            targetPosition.y = Mathf.Clamp(targetPosition.y, minHeight, maxHeight);
        }

        _elementLevitated.transform.position = Vector3.Lerp(
            _elementLevitated.transform.position,
            targetPosition,
            Time.deltaTime * 5f
        );

        if (levitationRotationSpeed > 0)
        {
            _elementLevitated.transform.Rotate(
                Vector3.up,
                levitationRotationSpeed * Time.deltaTime,
                Space.World
            );
        }
    }

    public void NoLevitate()
    {
        if (_elementLevitated == null) return;

        Rigidbody rb = _elementLevitated.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true; // ACTIVA GRAVEDAD AL SOLTAR
            rb.isKinematic = false;
            rb.freezeRotation = false;
        }

        _elementLevitated.GetComponent<IPuzzlesElements>()?.Desactivate();
        _elementLevitated = null;
    }

    private void AddModules(Transform _position)
    {
        var myDriver = _elementDetected.GetComponent<IModules>();
        if (myDriver == null) return;

        _weaponSelected = _elementDetected;
        _inventory.AddWeapon(_weaponSelected);
        _weaponSelected.transform.parent = transform;
        _weaponSelected.transform.position = _position.position;
        _weaponSelected.transform.rotation = this.transform.rotation;

        Rigidbody rb = _weaponSelected.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        SelectModule(_inventory.WeaponSelected);
        myDriver.Initialized(this);
    }

    private void SelectModule(int index)
    {
        if (index > _inventory.MyItemsCount() - 1) return;
        _weaponSelected = _inventory.SelectWeapon(index);
        _animatorBasic.animator = _inventory.MyCurrentAnimator();
    }

    #region Detecciones
    public Vector3 GetVectorFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    private bool CollectWeapon()
    {
        foreach (var item in colectables)
        {
            if (FieldOfView(item))
            {
                return true;
            }
        }
        return false;
    }

    public bool FieldOfView(GameObject obj)
    {
        Vector3 dir = obj.transform.position - transform.position;
        if (dir.magnitude < _viewRadius)
        {
            if (Vector3.Angle(transform.forward, dir) < _viewAngle * 0.5f)
            {
                if (!Physics.Raycast(transform.position, dir, out RaycastHit hit, _viewRadius, _wallLayer))
                {
                    Debug.DrawLine(transform.position, obj.transform.position, Color.magenta);
                    _elementDetected = obj;
                    return true;
                }
                else
                {
                    Debug.DrawLine(transform.position, hit.point, Color.magenta);
                    _elementDetected = null;
                }
            }
        }
        return false;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Vector3 lineA = GetVectorFromAngle(_viewAngle * 0.5f + transform.eulerAngles.y);
        Vector3 lineB = GetVectorFromAngle(-_viewAngle * 0.5f + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + lineA * _viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + lineB * _viewRadius);
    }

    #region Movement & Physics
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
        Vector3 f = cam.forward;
        f.y = 0;
        f.Normalize();

        Vector3 r = cam.right;
        r.y = 0;
        r.Normalize();

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
    #endregion

    #region Health System
    public void Health(float health)
    {
        _currentHealth += health;
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
    }

    public void Damage(float damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0f)
        {
            // Morir
        }
    }
    #endregion
}