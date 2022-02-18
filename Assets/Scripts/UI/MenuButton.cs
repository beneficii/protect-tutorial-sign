using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MenuButton : MonoBehaviour
{
    public string actionLine;
    MenuAction mAction;

    TextMeshPro txt;

    BtnState state = BtnState.Disabled;
    public BtnState State
    {
        get => state;
        set
        {
            state = value;
            txt.color = GetStateColor(value);
        }
    }

    static Color GetStateColor(BtnState state)
    {
        switch (state)
        {
            case BtnState.Disabled: return LTRO1Colors.darkGrey;
            case BtnState.Normal: return LTRO1Colors.grey;
            case BtnState.Selected: return LTRO1Colors.yellow;
            default: return LTRO1Colors.grey;
        }
    }

    private void Awake()
    {
        txt = GetComponent<TextMeshPro>();
        mAction = MenuAction.Create(actionLine);

        var state = BtnState.Disabled;
        if(mAction != null && mAction.IsActive)
        {
            state = BtnState.Normal;
        }

        State = state;
    }

    public void Execute()
    {
        mAction?.Execute();
    }

    public enum BtnState
    {
        Disabled,
        Normal,
        Selected
    }
}
