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

    [SerializeField] private Transform _weaponSpace1;
    [SerializeField] private Transform _weaponSpace2;
    [SerializeField] private Transform _weaponSpace3;

    [SerializeField] private Transform _levitationPoint;

    [SerializeField] private GameObject _elementDetected; //La que detecta el Raycast
    [SerializeField] private GameObject _weaponSelected; //El arma que esta activa
    [SerializeField] private GameObject _elementSelected; //El el elemento levitado

    [SerializeField] private List<MeshRenderer> _bodyRender;

    private Dictionary<KeyCode, GameObject> _invetoryWeapon = new Dictionary<KeyCode, GameObject>();

    public List<MeshRenderer> BodyRender {  get { return _bodyRender; } }

    // Componentes
    private Rigidbody rb;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _currentLife = _maxLife;
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void Update()
    {
        HandleJump();

        //Colectar partes
        if (Input.GetKeyDown(KeyCode.C) && CollectWeapon())
        {
            
            _weaponSelected = _elementDetected;
            _weaponSelected.transform.parent = transform;
            _weaponSelected.GetComponent<Rigidbody>().isKinematic = true;
            _weaponSelected.GetComponent<IDrivers>().Initialized(this);

            //Codigo provisorio
            if (_elementDetected.gameObject.name == "Arma1")
            {
               _invetoryWeapon.Add(KeyCode.Alpha1, _elementDetected);
                _elementDetected.transform.position = _weaponSpace1.transform.position;
            }

            if (_elementDetected.gameObject.name == "Arma2")
            {
                _invetoryWeapon.Add(KeyCode.Alpha2, _weaponSelected);
                _elementDetected.transform.position = _weaponSpace2.transform.position;
            }

            if (_elementDetected.gameObject.name == "Arma3")
            {
                _invetoryWeapon.Add(KeyCode.Alpha3, _weaponSelected);
                _elementDetected.transform.position = _weaponSpace3.transform.position;
            }
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
        if (Input.GetKeyDown (KeyCode.Alpha1))
        {
           if (_invetoryWeapon.ContainsKey(KeyCode.Alpha1))
            {
                _invetoryWeapon[KeyCode.Alpha1].GetComponent<IDrivers>().Initialized(this);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_invetoryWeapon.ContainsKey(KeyCode.Alpha2))
            {
                _invetoryWeapon[KeyCode.Alpha2].GetComponent<IDrivers>().Initialized(this);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (_invetoryWeapon.ContainsKey(KeyCode.Alpha3))
            {
                _invetoryWeapon[KeyCode.Alpha3].GetComponent<IDrivers>().Initialized(this);
            }
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

