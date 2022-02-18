using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorableComponent : MonoBehaviour
{
    public SpriteRenderer render;
    public OreColor defaultColor = OreColor.Red;

    public OreColor MainColor
    {
        get => defaultColor;
        set
        {
            defaultColor = value;
            Database.SetColor(render, value);
        }
    }

    public OreColor outline = OreColor.Black;
    public OreColor OutlineColor
    {
        get => outline;
        set
        {
            outline = value;
            Database.SetOutlineColor(render, value);
        }
    }

    public void Set(params OreColor[] colors)
    {
        if(colors.Length == 1)
        {
            MainColor = colors[0];
        }
        else
        {
            Database.SetColor(render, colors);
        }
    }

    public void Start()
    {
        MainColor = defaultColor;
        OutlineColor = outline;
    }

    public static void SetColors(MonoBehaviour target, params OreColor[] colors)
    {
        target.GetComponent<ColorableComponent>()?.Set(colors);
    }
}
