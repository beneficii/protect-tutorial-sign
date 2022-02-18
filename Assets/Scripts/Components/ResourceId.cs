using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceId : MonoBehaviour
{
    public static event System.Action<ResourceId> OnInit;

    public ResourceData data;
    public SpriteRenderer render;
    public int level = 0;

    public void Init(ResourceType type, OreColor color, int level = 0)
    {
        data = new ResourceData(type, color);
        OnInit?.Invoke(this);
        Refresh();
    }

    public void Init(ResourceData data)
    {
        Init(data.type, data.color, data.level);
    }

    public void Refresh()
    {
        render.sprite = Database.current.spritesForResource.Get(data.type);
        var c = Database.current.colorsForOre.Get(data.color);
        render.material.SetColor("Color_1", c);
    }
}



