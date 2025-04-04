using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    // Serialized Fields (ajustables en el Inspector)
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _rotateSpeed = 1f;
    [SerializeField] private float _lineToCollect;
    [SerializeField] private float _hightToCollect;
    [SerializeField] private LayerMask _layerArms;

    [SerializeField] private Transform _weaponSpace1;
    [SerializeField] private Transform _weaponSpace2;
    [SerializeField] private Transform _weaponSpace3;

    [SerializeField] private GameObject _weapon;

    // Componentes
    private Rigidbody rb;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void Update()
    {
        HandleJump();

        if (Input.GetKeyDown(KeyCode.C) && CollectArm())
        {
            _weapon.transform.parent = transform;
            _weapon.GetComponent<Rigidbody>().isKinematic = true;
            if (_weapon.gameObject.name == "Arma1")
            {
                _weapon.transform.position = _weaponSpace1.transform.position;
            }

            if (_weapon.gameObject.name == "Arma2")
            {
                _weapon.transform.position = _weaponSpace2.transform.position;
            }

            if (_weapon.gameObject.name == "Arma3")
            {
                _weapon.transform.position = _weaponSpace3.transform.position;
            }
        }
    }

    private void MovePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        Vector3 moveForce = moveDirection * _moveSpeed;
        rb.AddForce(moveForce, ForceMode.Force);

        if (moveDirection.magnitude != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * _rotateSpeed
            );
        }
        
    }

    private bool CollectArm()
    {
        RaycastHit hit;
        bool hasHit = Physics.Raycast(transform.position, transform.forward, out hit, _lineToCollect, _layerArms);

        if (hasHit)
        {
            _weapon = hit.transform.gameObject;
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

}

