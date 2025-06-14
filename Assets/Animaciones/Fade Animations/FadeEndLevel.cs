using UnityEngine;

public class FadeEndLevel : MonoBehaviour
{
    [SerializeField] private FadingScript fadingScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fadingScript.FadeOut();
        }
    }
}
