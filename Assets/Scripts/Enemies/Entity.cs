using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public abstract class Entity : MonoBehaviour, IDamagiable
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 30f;
    [SerializeField] protected float contactDamage = 10f;
    [SerializeField] protected float visionRange = 5f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float rotationSpeed = 5f;
    public Transform TargetPoint;

    [Header("Feedback")]
    [SerializeField] protected AudioClip damageClip;
    [SerializeField] protected AudioClip deathClip;

    [Header("Runtime Debug")]
    [SerializeField, Tooltip("Vida actual. Solo lectura en Inspector.")]
    protected float currentHealth;

    [SerializeField] protected Transform player;
    [SerializeField] protected PlayerMovement playerScript;
    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        // Inicializa la salud
        currentHealth = maxHealth;

        // Audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogError($"{name}: necesita un AudioSource para reproducir sonidos.");

       
    }

    protected virtual void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= visionRange && !playerScript.IsInvisible) //No modificar condicion de invisibilidad
            ChasePlayer();
        else
            OnIdle();
    }

    protected virtual void ChasePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        transform.position += dir * moveSpeed * Time.deltaTime;
        RotateTowards(player.position);
    }

    protected abstract void OnIdle();

    protected void RotateTowards(Vector3 target)
    {
        Vector3 dir = (target - transform.position);
        dir.y = 0;
        Quaternion toRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRot, rotationSpeed * Time.deltaTime);
    }
   

    /// <summary>
    /// Feedback visual o de animación; recibe la cantidad de daño para poder detallar.
    /// </summary>
    protected virtual void OnDamageFeedback(float amount)
    {
        Debug.Log($"{name} recibió {amount} de daño. Vida restante: {currentHealth}");
        // Aquí puedes disparar partículas, parpadeos de material, triggers de animación...
    }

    /// <summary>
    /// Reproduce sonido de muerte y destruye el GameObject tras el clip.
    /// </summary>
    private IEnumerator DieRoutine()
    {
        if (deathClip != null)
            audioSource.PlayOneShot(deathClip);

        // Esperar a que se acabe el clip (o 0.1s si no hay clip)
        float delay = deathClip != null ? deathClip.length : 0.1f;
        yield return new WaitForSeconds(delay);

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //playerScript?.TakeDamage(contactDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }

    public void Health(float health)
    {
        
    }

    /// <summary>
    /// Llama a este método para infligir daño.
    /// </summary>
    public void Damage(float damage)
    {
        // Reducir vida
        currentHealth -= damage;

        // Sonido de daño
        if (damageClip != null)
            audioSource.PlayOneShot(damageClip);

        // Feedback visual / debug
        OnDamageFeedback(damage);

        // Comprobación de muerte
        if (currentHealth <= 0f)
            StartCoroutine(DieRoutine());
    }
}