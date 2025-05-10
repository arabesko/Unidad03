using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool _canBlin = true;
    private void Awake()
    {
        Instance = this;
    }

    
}
