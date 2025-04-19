using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagiable
{
    [SerializeField] private float _currentLife;
    [SerializeField] private float _maxLife;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _rotateSpeed = 1f;
    [SerializeField] private LayerMask _layerArms;
    [SerializeField] private bool _isInvisible = false;
    public bool IsInvisible { get { return _isInvisible; } set { _isInvisible = value; } }

    private Vector3 _direction;

    

    //Field of view
    [SerializeField] private float _viewRadius;
    [SerializeField] private float _viewAngle;
    [SerializeField] private List<GameObject> _colectables;
    [SerializeField] private LayerMask _wallLayer;

    private float _zAxis;
    private float _xAxis;

    [SerializeField] private Transform _leftArm;
    [SerializeField] private Transform _proyector;
    [SerializeField] private Transform _rightArm;

    [SerializeField] private Transform _levitationPoint;

    [SerializeField] private GameObject _elementDetected; //La que detecta el Raycast
    [SerializeField] private GameObject _weaponSelected; //El arma que esta activa
    [SerializeField] private GameObject _elementSelected; //El el elemento levitado

    //Partes del cuerpo
    public List<MeshRenderer> bodyRender;

    //Respaldo de las partes del cuerpo
    [SerializeField] private List<Material> _bodyRenderOriginal; public List<Material> BodyRenderOriginal { get { return _bodyRenderOriginal; } }

    private Dictionary<string, GameObject> _modulos = new Dictionary<string, GameObject>();
    

    private delegate void PowerAccion();
    private PowerAccion _myPower;

    // Componentes
    private Rigidbody rb;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _currentLife = _maxLife;

        foreach (var item in bodyRender)
        {
            _bodyRenderOriginal.Add(item.material);
        }
    }

    private void Start()
    {
        SelectModule(); //Asigna el modulo inicial (Proyector)
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    /// <summary>
    /// Selecciona el modulo vacio para asignar el arma colectada
    /// </summary>
    private void SelectModule()
    {
        var myDriver = _elementDetected.GetComponent<IDrivers>();
        if (myDriver == null) return;

        _weaponSelected = _elementDetected;
        _weaponSelected.transform.parent = transform;
        _weaponSelected.transform.rotation = this.transform.rotation;
        _weaponSelected.GetComponent<Rigidbody>().isKinematic = true;
        myDriver.Initialized(this);

        //Selecciona que modulo esta disponible para asignar el arme colectada

        MyOriginalBody();

        if (!_modulos.ContainsKey("Proyector"))
        {
            _modulos.Add("Proyector", _elementDetected);
            _elementDetected.transform.position = _proyector.transform.position;
        }
        else if (!_modulos.ContainsKey("Brazo_I"))
        {
            _modulos.Add("Brazo_I", _elementDetected);
            _elementDetected.transform.position = _leftArm.transform.position;
        }
        else if (!_modulos.ContainsKey("Brazo_D"))
        {
            _modulos.Add("Brazo_D", _elementDetected);
            _elementDetected.transform.position = _rightArm.transform.position;
        }
    }

    public void MyOriginalBody()
    {
        for (int i = 0; i < bodyRender.Count; i++)
        {
            bodyRender[i].material = _bodyRenderOriginal[i];
        }
    }

    void Update()
    {
        _zAxis = Input.GetAxisRaw("Horizontal");
        _xAxis = Input.GetAxisRaw("Vertical");
        HandleJump();

        //Colectar partes
        if (Input.GetKeyDown(KeyCode.C) && CollectWeapon())
        {
            SelectModule();
        }

        //Levitar partes
        if (Input.GetKeyDown(KeyCode.R) && CollectWeapon())
        {
            _elementSelected = _elementDetected;
            _elementSelected.transform.parent = transform;
            _elementSelected.GetComponent<Rigidbody>().isKinematic = true;
            _elementSelected.transform.position = _levitationPoint.transform.position;
            _elementSelected.GetComponent<IPuzzlesElements>().Activate();
        }
        //Cuando deja de levitar cosas
        else if (Input.GetKeyDown(KeyCode.R) && _elementSelected != null)
        {
            _elementSelected.GetComponent<Rigidbody>().isKinematic = false;
            _elementSelected.GetComponent<IPuzzlesElements>().Desactivate();
            _elementSelected.transform.parent = null;
        }


        //Cambiar de arma
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
           if (_modulos.ContainsKey("Brazo_I"))
            {
                _modulos["Brazo_I"].GetComponent<IDrivers>().Initialized(this);
                _weaponSelected = _modulos["Brazo_I"];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_modulos.ContainsKey("Proyector"))
            {
                _modulos["Proyector"].GetComponent<IDrivers>().Initialized(this);
                _weaponSelected = _modulos["Proyector"];
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (_modulos.ContainsKey("Brazo_D"))
            {
                _modulos["Brazo_D"].GetComponent<IDrivers>().Initialized(this);
                _weaponSelected = _modulos["Brazo_D"];
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _weaponSelected.GetComponent<IDrivers>().PowerElement();
        }
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = new Vector3(_zAxis, 0f, _xAxis).normalized;

        rb.MovePosition(transform.position + moveDirection * _moveSpeed * Time.fixedDeltaTime);

        if (moveDirection.magnitude != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _rotateSpeed);
        }
    }

    private bool CollectWeapon()
    {
        foreach (var item in _colectables)
        {
            if (FieldOfView(item))
            {
                return true;
            }
        }
        return false;
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
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
                    _elementDetected = obj.gameObject;
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _viewRadius);

        Vector3 lineA = GetVectorFromAngle(_viewAngle * 0.5f + transform.eulerAngles.y);
        Vector3 lineB = GetVectorFromAngle(-_viewAngle * 0.5f + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position , transform.position + lineA * _viewRadius);
        Gizmos.DrawLine(transform.position , transform.position + lineB * _viewRadius);
    }

    public Vector3 GetVectorFromAngle(float angle)
    {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    

    public void Health(int health)
    {
        _currentLife += health;
    }

    public void Damage(int damage)
    {
        _currentLife -= damage;
    }
}

