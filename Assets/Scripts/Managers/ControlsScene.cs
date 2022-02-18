using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ControlsScene : MonoBehaviour
{
    public static ControlsScene current;

    private void Awake()
    {
        if(current != null)
        {
            Debug.LogError("Too many Control scenes!");
            Destroy(this);
            return;
        }
        current = this;
        DontDestroyOnLoad(this);
    }
}
