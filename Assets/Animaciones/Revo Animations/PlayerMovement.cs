using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] private float _viewRadius;
    [SerializeField] private float _viewAngle;
    public List<GameObject> colectables;
    [SerializeField] private LayerMask _wallLayer;

    [SerializeField] private GameObject _elementDetected; //La que detecta el Raycast
    [SerializeField] private GameObject _weaponSelected; //El arma que esta activa

    [SerializeField] private GameObject _elementLevitated; //El el elemento levitado
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

    [SerializeField] private bool _isInvisible = false; public bool IsInvisible { get { return _isInvisible; } set { _isInvisible = value; } }
    //Partes del cuerpo
    public List<MeshRenderer> bodyRender;
    [SerializeField] private Transform _projectorPosition;
    [SerializeField] private Transform _module1;


    public bool IsGrounded => Controller.isGrounded;

    private Vector3 velocity;
    private float coyoteCounter, jumpBufferCounter;
    private float currentSpeed;
    private bool isSprinting;

    //Es para que solo pueda cambiar de arma cuando el poder de cada arma este concluido
    [SerializeField] private bool _canWeaponChange = true; public bool CanWeaponChange { get { return _canWeaponChange; } set { _canWeaponChange = value; } }

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
            // Solo aplicar gravedad si está en el aire
            velocity.y += gravity * Time.deltaTime;
            Controller.Move(velocity * Time.deltaTime);
        }

        UpdateAnimation();

        //Colectar Modulos
        if (Input.GetKeyDown(KeyCode.E) && CollectWeapon() && CanWeaponChange)
        {
            Weapon myWeapon = _elementDetected.GetComponent<Weapon>();
            if (myWeapon != null) AddModules(_module1);
        }

        //Levitar partes
        print(CollectWeapon());
        if (Input.GetKeyDown(KeyCode.R) && CollectWeapon() && _elementLevitated == null)
        {
            _elementLevitated = _elementDetected;
            IPuzzlesElements myPuzzle = _elementLevitated.GetComponent<IPuzzlesElements>();
            if (myPuzzle == null)
            {
                _elementLevitated = null;
                return;
            }
            _elementLevitated.transform.parent = transform;
            _elementLevitated.GetComponent<Rigidbody>().isKinematic = true;
            _elementLevitated.transform.position = _levitationPoint.transform.position;
            myPuzzle.Activate();
        }
        //Cuando deja de levitar cosas
        else if (Input.GetKeyDown(KeyCode.R) && _elementLevitated != null)
        {
            NoLevitate();
        }
        //Ejecutar poder del arma

        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Q) && CanWeaponChange)
        {
            CanWeaponChange = false;
            _weaponSelected.GetComponent<IModules>().PowerElement();
        }

        if (Input.GetKeyDown (KeyCode.Alpha1) && CanWeaponChange)
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

    public void NoLevitate()
    {
        if (_elementLevitated == null) return;
        _elementLevitated.GetComponent<Rigidbody>().isKinematic = false;
        _elementLevitated.GetComponent<IPuzzlesElements>().Desactivate();
        _elementLevitated.transform.parent = null;
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
        _weaponSelected.GetComponent<Rigidbody>().isKinematic = true;
        SelectModule(_inventory.WeaponSelected);
        myDriver.Initialized(this);
    }

    private void SelectModule(int index)
    {
        if (index > _inventory.MyItemsCount() - 1) return;
        _weaponSelected = _inventory.SelectWeapon(index);
        _animatorBasic.animator = _inventory.MyCurrentAnimator(); //Asigna el animator
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
            if (Vector3.Angle(transform.forward, dir) < _viewAngle * 0.5f) //Field of View
            {
                if (!Physics.Raycast(transform.position, dir, out RaycastHit hit, _viewRadius, _wallLayer)) //Line of Sign
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

    #region LoDelAgus
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
            //Morir
        }
    }

    #endregion
}
