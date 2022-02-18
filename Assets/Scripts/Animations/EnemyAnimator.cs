using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAnimator : MonoBehaviour
{
    public List<Sprite> spritesMovement;
    public List<Sprite> spritesAttack;
    public List<Sprite> spritesDeath;
    public SpriteRenderer render;

    public int framesMovement = 2;
    public int framesAttack = 2;

    State state = State.Idle;
    public State AnimState
    {
        get => state;
        set
        {
            if(state != value)
            {
                switch (value)
                {
                    case State.Idle:
                        info.SetAnimation(0, 1, 0.15f);
                        break;
                    case State.Moving:
                        info.SetAnimation(0, framesMovement, 0.15f);
                        break;
                    default:
                        break;
                }

                state = value;
            }
        }
    }

    public Queue<AnimationAction> actionQueue = new Queue<AnimationAction>();
    Queue<Sprite> spriteQueue = new Queue<Sprite>();

    AnimationFrameInfo info = new AnimationFrameInfo(0.15f);

    private void Awake()
    {
    }

    private void Update()
    {
        if(actionQueue.Count > 0)
        {
            bool anyAdded = false;
            var action = actionQueue.Dequeue();
            switch (action)
            {
                case AnimationAction.Attack:
                    foreach (var item in spritesAttack)
                    {
                        spriteQueue.Enqueue(item);
                        anyAdded = true;
                    }
                    break;
                case AnimationAction.Death:
                    foreach (var item in spritesDeath)
                    {
                        spriteQueue.Enqueue(item);
                        anyAdded = true;
                    }
                    break;
            }
            if(anyAdded)
            {
                info.FramesQueued();
            }
        }


        if(info.ReadyForNext)
        {
            if(spriteQueue.Count > 0 )
            {
                render.sprite = spriteQueue.Dequeue();
            }
            else
            {
                if (state == State.Dead) return;
                render.sprite = spritesMovement[info.CurrentFrame];
                info.SetNextFrameIdx();
            }
            info.SetNextFrameTime();
        }
    }

    public enum State
    {
        Idle,
        Moving,
        Dead
    }
}

public enum AnimationAction
{
    None,
    Attack,
    Death
}

public struct AnimationFrameInfo
{
    public float rate;
    public float next;
    public int idx;
    public int offset;
    public int count;
    public bool actionInProgress;

    public bool ReadyForNext => Time.time >= next;
    public int CurrentFrame => offset + idx;

    public AnimationFrameInfo(float frameRate, int frameCount = 1)
    {
        this.rate = frameRate;
        this.next = 0;
        this.idx = 0;
        this.offset = 0;
        this.count = frameCount;
        this.actionInProgress = false;
    }

    public void SetAnimation(int offset, int count, float frameRate = 99f)
    {
        this.offset = offset;
        this.count = count;

        rate = frameRate;

        AnimationChanged();
    }

    public void FramesQueued()
    {
        AnimationChanged();
    }

    public void AnimationChanged()
    {
        next = 0;
        idx = 0;
    }

    public void SetNextFrameIdx()
    {
        idx = (idx + 1) % count;
    }

    public void SetNextFrameTime()
    {
        next = Time.time + rate;
    }
}
