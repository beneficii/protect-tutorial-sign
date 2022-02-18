using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingAnimator : MonoBehaviour
{
    public List<Sprite> spritesWorking;
    public SpriteRenderer render;

    List<Sprite> activeFrames;

    Sprite spriteIddle;
    
    int frameIdx = 0;
    bool looping = false;
    bool finished = false;
    public bool Finished() => finished;

    float nextFrame = 0f;

    private void Awake()
    {
        spriteIddle = render.sprite;
        SetAnimation(spriteIddle, false);
    }

    public void SetWorkingProgress(int idx)
    {
        idx = idx % spritesWorking.Count;
        if (idx != frameIdx)
        {
            render.sprite = spritesWorking[idx];
            frameIdx = idx;
        }
    }

    public void SetAnimation(AnimState state,  bool loop = true)
    {
        switch (state)
        {
            case AnimState.Iddle:
                SetAnimation(spriteIddle, false);
                break;
            case AnimState.Working:
                SetAnimation(spritesWorking, loop);
                break;
            default:
                break;
        }
    }

    public void SetAnimation(Sprite sprite, bool loop)
    {
        SetAnimation(new List<Sprite> { sprite }, loop);
    }

    public void SetAnimation(List<Sprite> sprites, bool loop)
    {
        activeFrames = sprites;
        frameIdx = 0;
        nextFrame = 0f;
        looping = loop;
        finished = false;
    }

    public void SetIddle()
    {
        frameIdx = -1;
        render.sprite = spriteIddle;
    }

    public int GetFrameCount(AnimState state)
    {
        switch (state)
        {
            case AnimState.Iddle: return 1;
            case AnimState.Working: return spritesWorking.Count;
        }

        return 0;
    }

    private void Update()
    {
        if(!finished && Time.time >= nextFrame)
        {
            if(frameIdx >= activeFrames.Count)
            {
                if (looping)
                {
                    frameIdx = 0;
                }
            }

            render.sprite = activeFrames[frameIdx];

            frameIdx++;

            if(frameIdx >= activeFrames.Count)
            {
                if(looping)
                {
                    frameIdx = 0;
                }
                else
                {
                    finished = true;
                }
            }

            nextFrame = Time.time + Constants.frameDelay;
        }
    }

    public enum AnimState
    {
        Iddle,
        Working
    }
}