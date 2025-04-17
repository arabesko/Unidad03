using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseScavanger : MonoBehaviour
{
    public Transform _player;
    [SerializeField] private float _speed;
    [SerializeField] private float _visionRange;
    [SerializeField] private float _life;
    [SerializeField] private Transform[] _positions;
    [SerializeField] private int _index;
    [SerializeField] private float _rotationSpeed;

    [SerializeField] Transform _point1;
    [SerializeField] Transform _point2;
    [SerializeField] Transform _point3;
    [SerializeField] Transform _point4;

    private bool shaseNow;

    void Start()
    {
        
    }

    
    void Update()
    {
        if (Vector3.Distance(transform.position, _player.position) < _visionRange)
        {
            shaseNow = true;
        }

        if (shaseNow == true)
        {

            Vector3 playerDirection = (_player.position - transform.position).normalized;
            playerDirection.y = 0;

            if (Vector3.Distance(transform.position, _player.position) > 0.5f)
            {
                transform.position += playerDirection * _speed * Time.deltaTime;
            }

            Vector3 targetDirection = _player.position - transform.position;
            transform.rotation = Quaternion.LookRotation(targetDirection);
        }
        else
        {
            Vector3 positionDirection = (_positions[_index].position - transform.position).normalized;
            positionDirection.y = 0;

            transform.position = Vector3.MoveTowards(transform.position, _positions[_index].position, _speed * Time.deltaTime);


            Vector3 targetDirection = _positions[_index].position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _positions[_index].position) < 0.1f)
            {
                _index++;
                if (_index >= _positions.Length)
                {
                    _index = 0;
                }
            }
        }
    }
}
