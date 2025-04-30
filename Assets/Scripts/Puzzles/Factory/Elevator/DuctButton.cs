using System.Collections;
using UnityEngine;

public class DuctButton : MonoBehaviour
{
    [SerializeField] private Transform[] doorsToOpen;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveSpeed = 1f;
    private bool opened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !opened)
        {
            opened = true;
            foreach (Transform door in doorsToOpen)
            {
                StartCoroutine(MoveDoorUp(door));
            }
        }
    }

    IEnumerator MoveDoorUp(Transform door)
    {
        Vector3 startPos = door.position;
        Vector3 targetPos = startPos + Vector3.up * moveDistance;
        while (Vector3.Distance(door.position, targetPos) > 0.01f)
        {
            door.position = Vector3.MoveTowards(door.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
