using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _speedRotation;
    [SerializeField] private float _damage = 10f;
    private Transform _target;

    void Start()
    {
        Destroy(gameObject, 5f);

        // Busca el enemigo más cercano al iniciar
        FindNearestEnemy();
    }

    void Update()
    {
        if (_target != null)
        {
            // Calcula dirección hacia el enemigo
            Vector3 direction = (_target.position - transform.position).normalized;
            transform.position += direction * _moveSpeed * Time.deltaTime;

            // Opcional: rota para mirar hacia el enemigo
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, _speedRotation * Time.deltaTime);
        }
        else
        {
            // Si no hay enemigo, sigue avanzando recto
            transform.position += transform.forward * _moveSpeed * Time.deltaTime;
            transform.Rotate(0, 0, _speedRotation * Time.deltaTime);
        }
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

    private void FindNearestEnemy()
    {
        BaseScavanger[] enemies = FindObjectsOfType<BaseScavanger>();
        float minDistance = Mathf.Infinity;
        BaseScavanger nearestEnemy = null;

        foreach (BaseScavanger enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null)
        {
            _target = nearestEnemy.transform;
        }
    }
}
