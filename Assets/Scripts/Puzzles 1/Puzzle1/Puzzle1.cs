using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Puzzle1 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _myPercent;

    private void OnTriggerEnter(Collider other)
    {
        IPuzzlesElements myPuzzle = GetComponent<IPuzzlesElements>();
        if (myPuzzle != null)
        {

        }
    }
}
