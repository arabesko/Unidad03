using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _speedRotation;
    [SerializeField] private float _damage = 10f;

    void Start()
    {
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
        transform.Rotate(0, 0, _speedRotation * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseScavanger enemy = other.GetComponent<BaseScavanger>();
        if (enemy != null)
        {
            enemy.TakeDamage(_damage);
            Destroy(gameObject); 
        }
    }
}
