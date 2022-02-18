using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingBlock : MonoBehaviour
{
    public ResourceData resource;
    public SpriteRenderer render;

    private void Start()
    {
        var c = Database.current.colorsForOre.Get(resource.color);
        render.material.SetColor("Color_1", c);
        if (resource.type != ResourceType.Bar) render.sprite = Database.current.spritesForResource.Get(resource.type);
    }

}
