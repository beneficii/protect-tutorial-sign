using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InfoArrow : MonoBehaviour
{
    public Animator anim;
    public SpriteRenderer render;

    float showTme = 0f;
    bool showed = false;

    public void Init(Sprite sprite, bool animated = false, float showDelay = 0f)
    {
        render.sprite = sprite;
        anim.enabled = animated;

        if(showDelay == 0f)
        {
            Show();
        }
        else
        {
            showTme = Time.time + showDelay;
        }
    }

    private void Update()
    {
        if(!showed && Time.time >= showTme)
        {
            Show();
        }
    }

    void Show()
    {
        render.gameObject.SetActive(true);
        showed = true;
    }
}
