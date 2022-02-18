using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class GameEvents : MonoBehaviour
{
    public InputData input;

    public static event System.Action PreRestart;
    public static GameEvents current;

    public GameObject endScreen;
    public TextMeshPro txtTitle;

    public MenuButton btnContinue;
    public MenuButton btnLoad;

    public static bool IsPaused => Time.timeScale == 0f;

    private void Awake()
    {
        current = this;
    }

    public void Finish(bool victory)
    {
        if (endScreen.activeSelf) return;

        if (victory)
        {
            MyAudio.current.Play(Sounds.Victory);
            txtTitle.text = "Victory!";
            txtTitle.color = LTRO1Colors.green;
            btnContinue.gameObject.SetActive(true);
        }
        else
        {
            MyAudio.current.Play(Sounds.Lose);
            txtTitle.text = "Defeat!";
            txtTitle.color = LTRO1Colors.red;
            btnLoad.gameObject.SetActive(true);
        }

        endScreen.SetActive(true);
    }

    private void Update()
    {
        if (input.selectButton.State == KeyState.Down)
        {
            if (endScreen.activeSelf) return;
            if (Time.timeScale == 0) return;
            input.BlockAll(0.5f);

            Time.timeScale = 0f;
            PreRestart?.Invoke();

            SceneManager.LoadSceneAsync("QuickMenu", LoadSceneMode.Additive);
        }
    }
}
