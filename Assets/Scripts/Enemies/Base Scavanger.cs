using UnityEngine;
using System.Collections;

public class BaseScavanger : Entity
{
    enum State { Idle, Patrolling, Chasing, Attacking, Stunned }
    State currentState = State.Idle;

    [SerializeField] Transform player;
    [SerializeField] float visionRange = 10f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float patrolSpeed = 2f, chaseSpeed = 4f;
    [SerializeField] Animator animator;
    [SerializeField] Transform[] waypoints;

    int currentWP = 0;
    bool isStunned = false;
    bool isAttacking = false;

    float attackCooldown = 1.5f;
    float attackTimer = 0f;

    void Update()
    {
        if (isStunned)
        {
            SetState(State.Stunned);
            return;
        }

        attackTimer -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < attackRange && attackTimer <= 0f)
        {
            SetState(State.Attacking);
            Attack();
        }
        else if (dist < visionRange)
        {
            SetState(State.Chasing);
            Chase();
        }
        else
        {
            SetState(State.Patrolling);
            Patrol();
        }
    }

    void SetState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (newState)
        {
            case State.Patrolling:
                animator.ResetTrigger("Attack");
                animator.ResetTrigger("Chase");
                animator.SetTrigger("Walk");
                break;

            case State.Chasing:
                animator.ResetTrigger("Attack");
                animator.ResetTrigger("Walk");
                animator.SetTrigger("Chase");
                break;

            case State.Attacking:
                animator.ResetTrigger("Walk");
                animator.ResetTrigger("Chase");
                animator.SetTrigger("Attack");
                break;

            case State.Stunned:
                animator.ResetTrigger("Walk");
                animator.ResetTrigger("Chase");
                animator.ResetTrigger("Attack");
                animator.SetTrigger("Stun"); // si tienes animación de stun
                break;
        }
    }

    void Patrol()
    {
        Transform target = waypoints[currentWP];
        transform.position = Vector3.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.position) < 0.2f)
            currentWP = (currentWP + 1) % waypoints.Length;
    }

    void Chase()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * chaseSpeed * Time.deltaTime;
    }

    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            attackTimer = attackCooldown;
        }
    }

    // Este método se debe llamar desde un evento en la animación de ataque
    public void DealDamageToPlayer()
    {
        IDamagiable damagiable = player.GetComponent<IDamagiable>();
        if (damagiable != null)
        {
            damagiable.Damage(contactDamage);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    public void Stun(float duration) => StartCoroutine(StunRoutine(duration));

    IEnumerator StunRoutine(float time)
    {
        isStunned = true;
        SetState(State.Stunned);
        yield return new WaitForSeconds(time);
        isStunned = false;
    }
}