using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public abstract class Entity : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected float maxHealth = 30f;
    [SerializeField] protected float contactDamage = 10f;
    [SerializeField] protected float visionRange = 5f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float rotationSpeed = 5f;

    [Header("Feedback")]
    [SerializeField] protected AudioClip damageClip;
    [SerializeField] protected AudioClip deathClip;

    [Header("Runtime Debug")]
    [SerializeField, Tooltip("Vida actual. Solo lectura en Inspector.")]
    protected float currentHealth;

    protected Transform player;
    protected PlayerMovement playerScript;
    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        // Inicializa la salud
        currentHealth = maxHealth;

        // Audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            Debug.LogError($"{name}: necesita un AudioSource para reproducir sonidos.");

        // Referencia al jugador
        var go = GameObject.FindWithTag("Player");
        if (go != null)
        {
            player = go.transform;
            playerScript = go.GetComponent<PlayerMovement>();
        }
        else
        {
            Debug.LogWarning($"{name}: no se encontr� ning�n GameObject con tag \"Player\".");
        }
    }

    protected virtual void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= visionRange)
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
    /// Llama a este m�todo para infligir da�o.
    /// </summary>
    public virtual void TakeDamage(float amount)
    {
        // Reducir vida
        currentHealth -= amount;

        // Sonido de da�o
        if (damageClip != null)
            audioSource.PlayOneShot(damageClip);

        // Feedback visual / debug
        OnDamageFeedback(amount);

        // Comprobaci�n de muerte
        if (currentHealth <= 0f)
            StartCoroutine(DieRoutine());
    }

    /// <summary>
    /// Feedback visual o de animaci�n; recibe la cantidad de da�o para poder detallar.
    /// </summary>
    protected virtual void OnDamageFeedback(float amount)
    {
        Debug.Log($"{name} recibi� {amount} de da�o. Vida restante: {currentHealth}");
        // Aqu� puedes disparar part�culas, parpadeos de material, triggers de animaci�n...
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
}