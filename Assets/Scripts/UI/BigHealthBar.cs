using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(ColorableComponent))]
public class BigHealthBar : MonoBehaviour
{
    public SpriteRenderer render;

    ColorableComponent colorable;

    float maxWidth = 1f;

    ObjectWithHealth target;

    private void Awake()
    {
        maxWidth = render.transform.localScale.x;
        colorable = GetComponent<ColorableComponent>();
    }

    public void Set(int current, int max)
    {
        float value = current / (float)max;

        var scale = render.transform.localScale;
        scale.x = value * maxWidth;
        render.transform.localScale = scale;
    }

    public void Bind(ObjectWithHealth obj)
    {
        Unbind();
        target = obj;
        if (obj == null) return;

        obj.OnChanged += Set;
        Set(obj.Hp, obj.maxHp);
    }

    public void Unbind()
    {
        if (!target) return;
        target.OnChanged -= Set;
    }
}