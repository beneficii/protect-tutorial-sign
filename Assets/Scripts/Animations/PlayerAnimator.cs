using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public const float defaultHammerFramerate = 0.15f;
    public const int attackFrames = 4;

    public List<Sprite> sprites;
    public List<Transform> spotsForCarry;
    public SpriteRenderer render;
    int spritesPerAnimation = 3;
    int framesPerAnimation = 4;
    public InputData inp;


    ResourceCarrier carrier;
    PlayerCtrl ctrl;

    AnimType prevAnim = AnimType.Normal;
    bool lastCarrying = false;
    Vector2Int lastDirection;
    float frameRate = 0.15f;
    float nextFrame = 0f;
    int frameIdx = 0;

    private void Awake()
    {
        carrier = GetComponent<ResourceCarrier>();
        ctrl = GetComponent<PlayerCtrl>();
    }

    int GetWalkFrame(Vector2Int direction, int idx, AnimType state)
    {
        int offset = 0;
        if(direction.x != 0)
        {
            offset = spritesPerAnimation * 2;
        }
        else if(direction.y > 0)
        {
            offset = spritesPerAnimation * 1;
        }
        else if(direction.y < 0)
        {
            offset = 0;
        }

        offset += (int)state * spritesPerAnimation * 3;

        if(state != AnimType.Attack)
        {
            switch (idx)
            {
                case 1: return offset + 1;
                case 3: return offset + 2;
                default: return offset;
            }
        }
        else
        {
            if (idx > 0) idx --; //adjust if necessary
            return offset + idx;
        }

        
    }

    private void LateUpdate()
    {
        var direction = inp.Direction;
        bool carrying =  carrier.currentItem != null;
        bool moving = direction != Vector2Int.zero;
        bool isAttacking = inp.hammerButton.State != KeyState.None;

        var state = carrying ? AnimType.Carry : (isAttacking ? AnimType.Attack : AnimType.Normal);

        if(state == AnimType.Attack)
        {
            direction = ctrl.InteractableDirection;
        }

        if (lastDirection != direction)
        {
            if(!moving) frameIdx = 0;
            nextFrame = Time.time; //instantly switch frames
        }

        if(state != prevAnim)
        {
            frameIdx = 0;
            nextFrame = Time.time;
        }

        if(Time.time >= nextFrame)
        {
            render.sprite = sprites[GetWalkFrame(direction, frameIdx, state)];
            if (moving || state == AnimType.Attack)
            {
                frameIdx++;
                if (frameIdx >= framesPerAnimation)
                {
                    frameIdx = 0;
                    if (state == AnimType.Attack) MyAudio.current.Play(Sounds.Hammer);
                }
            }

            render.flipX = direction.x < 0;

            //change carry spot
            int spotIdx = 0;
            if (direction.x > 0)
            {
                spotIdx = 1;
            }
            else if (direction.x < 0)
            {
                spotIdx = 3;
            }
            else if (direction.y > 0)
            {
                spotIdx = 2;
            }

            carrier.carryParent.localPosition = spotsForCarry[spotIdx].localPosition;

            if(state == AnimType.Attack)
            {
                nextFrame += defaultHammerFramerate / ctrl.hammerSpeed;
            }
            else
            {
                nextFrame = Time.time + frameRate;
            }
        }


        lastDirection = direction;
        lastCarrying = carrying;
        prevAnim = state;
    }


    public enum AnimType
    {
        Normal = 0,
        Carry = 1,
        Attack = 2,
    }
}
