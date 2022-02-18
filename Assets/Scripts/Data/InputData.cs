using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "LTRO-1/other/inputs")]
public class InputData : ScriptableObject
{
    public SingleButton hammerButton { get; private set; }
    public SingleButton useButton { get; private set; }
    public SingleButton selectButton { get; private set; }
    public Vector2Int Direction { get; private set; }
    public bool directionChanged = false;

    string useName;
    string hammerName;
    string startName;

    public string debugLine;

    public void ResetAll(string useName, string hammerName, string startName)
    {
        hammerButton = new SingleButton();
        useButton = new SingleButton();
        selectButton = new SingleButton();

        this.useName = useName;
        this.hammerName = hammerName;
        this.startName = startName;
    }

    public string AddControlsToString(string msg)
    {
        return msg
            .Replace("(_bUse)", $"[{useName}]")
            .Replace("(_bStart)", $"[{startName}]")
            .Replace("(_bHammer)", $"[{hammerName}]");
    }

    public void BlockAll(float time = 0f)
    {
        hammerButton.Block(time);
        useButton.Block(time);
        selectButton.Block(time);

    }

    public void SetInputs(Vector2Int direction, bool a, bool b, bool select)
    {
        hammerButton.Set(b);
        useButton.Set(a);
        selectButton.Set(select);

        if (Direction != direction) directionChanged = true;


        directionChanged = Direction != direction;
        Direction = direction;
    }


    public bool AnyButtonDown =>
        hammerButton.State == KeyState.Down
        || useButton.State == KeyState.Down
        || selectButton.State == KeyState.Down;
}

public class SingleButton
{
    bool prevDown;
    bool down;
    bool block;

    float blockedUntil = 0f;

    public KeyState State
    {
        get
        {
            if (down)
            {
                if (prevDown)
                {
                    return KeyState.Holding;
                }
                else
                {
                    return KeyState.Down;
                }
            }
            else
            {
                if (prevDown)
                {
                    return KeyState.Up;
                }
                else
                {
                    return KeyState.None;
                }
            }
        }
    }

    public void Reset()
    {
        prevDown = false;
        down = false;
    }

    public void Set(bool value)
    {
        prevDown = down;

        if (block)
        {
            if (Time.realtimeSinceStartup < blockedUntil) return;

            if (value)
            {
                return;
            }
            else
            {
                block = false;
            }
        }
        down = value;
    }

    public void Block(float time = 0f)
    {
        Set(false);
        block = true;
        if (time > 0) blockedUntil = Time.realtimeSinceStartup + time;
    }
}

public enum KeyState
{
    None,
    Down,
    Holding,
    Up,
}
