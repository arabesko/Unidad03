using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour, IDamagiable
{
    [SerializeField] private float _currentLife;
    [SerializeField] private float _maxLife;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _rotateSpeed = 1f;
    [SerializeField] private float _lineToCollect;
    [SerializeField] private float _hightToCollect;
    [SerializeField] private LayerMask _layerArms;

    [SerializeField] private Transform _leftArm;
    [SerializeField] private Transform _proyector;
    [SerializeField] private Transform _rightArm;

    [SerializeField] private Transform _levitationPoint;

    [SerializeField] private GameObject _elementDetected; //La que detecta el Raycast
    [SerializeField] private GameObject _weaponSelected; //El arma que esta activa
    [SerializeField] private GameObject _elementSelected; //El el elemento levitado

    [SerializeField] private List<MeshRenderer> _bodyRender;

    private Dictionary<string, GameObject> _modulos = new Dictionary<string, GameObject>();
    public List<MeshRenderer> BodyRender {  get { return _bodyRender; } }
    private delegate void PowerAccion();
    private PowerAccion _myPower;

    // Componentes
    private Rigidbody rb;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _currentLife = _maxLife;
    }

    private void Start()
    {
        SelectModule(); //Asigna el modulo inicial (Proyector)
    }

    public void Proyector()
    {
        print("Proyectando");
    }

    public void Invisible()
    {
        print("Me hago invisible");
    }

    public void Pulse()
    {
        print("Disparo una bala");
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

    void Update()
    {
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
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        rb.MovePosition(transform.position += moveDirection * _moveSpeed * Time.fixedDeltaTime);

        if (moveDirection.magnitude != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotateSpeed);
        }
        
    }

    private bool CollectWeapon()
    {
        //ESTO SE CAMBIARA POR UN RANGO DE VISION
        RaycastHit hit;
        bool hasHit = Physics.Raycast(transform.position, transform.forward, out hit, _lineToCollect, _layerArms);

        if (hasHit)
        {
            _elementDetected = hit.transform.gameObject;
            return true;
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + (transform.up / 5) * _hightToCollect, transform.position + (transform.up / 5) * _hightToCollect + transform.forward * _lineToCollect);
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

