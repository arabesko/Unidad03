using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorBasic : MonoBehaviour
{
    [SerializeField] PlayerMovement _playerMovement;

    [Header("Animation Settings")]
    [Range(0f, 1f)]
    public float walkAnimValueTransition = 0.1f;

    [HideInInspector] public bool EnableMovement = true;
    [HideInInspector] public bool IsDashing = false;

    [SerializeField] private Animator _animator;

    void Update()
    {
        print(_playerMovement.pct);
        UpdateAnimation();
    }

    void ChangeAnimator(Animator myAnimator)
    {
        _animator = myAnimator;
    }

    private void UpdateAnimation()
    {
        if (_animator == null) return;
        _animator.SetFloat("Velocity", _playerMovement.pct, 0.1f, Time.deltaTime);
    }
}
