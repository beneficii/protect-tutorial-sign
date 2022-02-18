using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerAnimator : MonoBehaviour
{
    public List<Sprite> sprites;
    public List<int> frameCount;
    //public List<Transform> spotsForCarry;
    public SpriteRenderer render;
    public InputData inp;

    ResourceCarrier carrier;

    bool lastCarrying = false;
    bool lastMoving = false;
    Vector2Int lastDirection;
    float frameRate = 0.15f;
    float nextFrame = 0f;
    int frameIdx = 0;
    int animationOffset = 0;

    Animations currentAnim = Animations.Iddle;


    private void Awake()
    {
        carrier = GetComponent<ResourceCarrier>();
    }

    void SetAnimationOffset(Animations anim)
    {
        int offset = 0;
        int animIdx = (int)anim;

        for (int i = 0; i < animIdx; i++)
        {
            offset += frameCount[i];
        }

        animationOffset = offset;
    }

    Sprite GetFrame(int idx, bool carry)
    {
        int frameIdx = animationOffset + idx;

        return sprites[frameIdx];
    }

    private void LateUpdate()
    {
        var direction = inp.Direction;
        bool carrying =  carrier.currentItem != null;
        bool moving = direction != Vector2Int.zero;
       

        if (moving != lastMoving) //changing animation
        {
            frameIdx = 0;
            nextFrame = 0; //instantly switch frames
            currentAnim = moving ? Animations.Walk : Animations.Iddle;

            SetAnimationOffset(currentAnim);
        }

        //if(carrying != lastCarrying) nextFrame = 0;

        if(Time.time >= nextFrame)
        {
            render.sprite = GetFrame(frameIdx, carrying);
            frameIdx = (frameIdx + 1)%frameCount[(int)currentAnim];
            if(direction.x != 0) render.flipX = direction.x < 0;
            /*
            //change carry spot
            int spotIdx = 0;
            if (direction.x > 0)
            {
                spotIdx = 1;
            }

            carrier.carryParent.localPosition = spotsForCarry[spotIdx].localPosition;
            */
            nextFrame = Time.time + frameRate;
        }


        lastMoving = moving;
        lastCarrying = carrying;
    }


    public enum Animations
    {
        Iddle = 0,
        Walk = 1,
        Attack = 2,
    }
}
