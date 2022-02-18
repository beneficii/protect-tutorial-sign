using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyDebug : MonoBehaviour
{
    public float speed = 1f;

    [EasyButtons.Button]
    public void SetSpeed()
    {
        Time.timeScale = speed;
    }
}
