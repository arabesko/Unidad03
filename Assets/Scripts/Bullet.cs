using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    void Start()
    {
        //Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }
}
