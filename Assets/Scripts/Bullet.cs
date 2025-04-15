using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _speedRotation;
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        transform.Rotate(0, 0, _speedRotation * Time.deltaTime);
    }
}
