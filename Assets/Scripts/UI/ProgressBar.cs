using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public List<Sprite> sprites;
    public SpriteRenderer render;
    public OreColor color = OreColor.Red;

    public int TotalFrames => sprites.Count;

    public OreColor ColorOfFill
    {
        get => color;
        set
        {
            color = value;
            Database.SetColor(render, value);
        }
    }

    //public bool animateDamage = false;

    int lastIdx = 0;

    public void Start()
    {
        lastIdx = 0;
        render.sprite = sprites[0];
        ColorOfFill = color;
    }


    public void Set(float progress)
    {
        progress = Mathf.Clamp(progress, 0f, 1f);
        int idx = Mathf.Clamp(Mathf.RoundToInt(progress * sprites.Count), 0, sprites.Count - 1);

        SetIdx(idx);
    }

    public void Set(int value, int max)
    {
        Set(value / (float)max);
    }

    void SetIdx(int idx)
    {
        if (lastIdx == idx) return;

        render.sprite = sprites[idx];

        lastIdx = idx;
    }
}
