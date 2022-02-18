using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ToolSceneLoader : MonoBehaviour
{
    private void Awake()
    {
        if(ControlsScene.current == null)
        {
            SceneManager.LoadSceneAsync("Controls", LoadSceneMode.Additive);
        }
    }
}

public enum SceneId
{
    Menu,
    Controls,
    Game,
    QuickMenu
}
