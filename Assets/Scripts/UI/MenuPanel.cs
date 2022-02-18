using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class MenuPanel : MonoBehaviour
{
    public InputData inputData;
    public string scene;
    List<MenuButton> items;
    int menuIdx;

    float nextPress = 0f;

    int GetNextSelectableIdx(int idx, int delta)
    {
        for (int i = 0; i < items.Count; i++)
        {
            idx = (idx + items.Count + delta) % items.Count;

            if (items[idx].State != MenuButton.BtnState.Disabled) return idx;
        }

        return -1; // just don't have this scenario, so we throw error
    }


    private void Start()
    {
        items = FindObjectsOfType<MenuButton>()
            .OrderBy(x => -x.transform.position.y)
            .ToList();

        menuIdx = GetNextSelectableIdx(-1, +1);
        items[menuIdx].State = MenuButton.BtnState.Selected;
    }

    // Update is called once per frame
    void Update()
    {
        if(inputData.AnyButtonDown)
        {
            Time.timeScale = 1f;
            inputData.BlockAll(0.5f);
            items[menuIdx].Execute();
            if (!string.IsNullOrEmpty(scene))
            {
                SceneManager.UnloadSceneAsync(scene);
            }
            MyAudio.current.Play(Sounds.Select2);
            return;
        }

        if(inputData.Direction == Vector2Int.zero)
        {
            nextPress = 0f;
        }

        if (Time.time < nextPress) return;

        if(inputData.Direction == Vector2Int.down)
        {
            DirectionPressed(+1);
        }
        else if (inputData.Direction == Vector2Int.up)
        {
            DirectionPressed(-1);
        }
    }

    void DirectionPressed(int delta)
    {
        items[menuIdx].State = MenuButton.BtnState.Normal;
        menuIdx = GetNextSelectableIdx(menuIdx, delta);
        items[menuIdx].State = MenuButton.BtnState.Selected;
        MyAudio.current.Play(Sounds.Select);

        nextPress = Time.time + 0.5f;
    }

}
