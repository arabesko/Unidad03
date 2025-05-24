using UnityEngine;

public class BaseScavanger : Entity
{
    [Header("Patrol Points")]
    [SerializeField] private Transform[] waypoints;
    private int currentIndex = 0;

    protected override void Awake()
    {
        base.Awake();
        if (waypoints == null || waypoints.Length == 0)
            Debug.LogWarning($"{name}: no hay waypoints asignados.");
    }

    protected override void OnIdle()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentIndex];
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        RotateTowards(target.position);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
            currentIndex = (currentIndex + 1) % waypoints.Length;
    }

    protected override void OnDamageFeedback(float amount)
    {
        base.OnDamageFeedback(amount);
        // + partículas de sangre, parpadeo, etc.
    }
}