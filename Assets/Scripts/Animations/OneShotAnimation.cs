using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OneShotAnimation : MonoBehaviour
{
    public SpriteRenderer render;

    List<Sprite> sprites = new List<Sprite>();
    int idx = 0;
    float nextFrame = float.MaxValue;
    int loops;


    public void Init(List<Sprite> sprites, int loops = 1)
    {
        this.sprites = sprites;
        this.loops = loops;
        NextFrame();
    }


    // Update is called once per frame
    void Update()
    {
        if (Time.time < nextFrame) return;

        idx++;
        if (idx >= sprites.Count)
        {
            loops--;
            idx = 0;
            if(loops <= 0)
            {
                Destroy(gameObject);
                return;
            }
        }

        NextFrame();
    }

    void NextFrame()
    {
        render.sprite = sprites[idx];
        nextFrame = Time.time + Constants.frameDelay;
    }
}
