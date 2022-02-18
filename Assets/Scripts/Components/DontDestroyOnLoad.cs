using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
