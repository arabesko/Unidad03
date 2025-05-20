using UnityEngine;

public class Weapon : MonoBehaviour, IModules
{
    private MeshRenderer _render;
    protected PlayerMovement _player;

    [SerializeField] private GameObject _myBodyFBX; public GameObject MyBodyFBX { get { return _myBodyFBX; } }
    [SerializeField] private Animator _myAnimator; public Animator MyAnimator { get { return _myAnimator; } }

    private void Awake()
    {
        _render = GetComponent<MeshRenderer>();
    }
    public virtual void Initialized(PlayerMovement player)
    {
       _player = player;
    }
    public virtual void PowerElement()
    {
    }
    
}
