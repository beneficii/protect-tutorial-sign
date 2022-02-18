using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ObjectWithHealth))]
[RequireComponent(typeof(ColorableComponent))]
[RequireComponent(typeof(EnemyAnimator))]
[RequireComponent(typeof(ObjectWithTutorial))]
public class Enemy : MonoBehaviour, IAttackable, IKillable, IWithUnitInfo
{
    public static event System.Action<Enemy> OnKilled;
    public static event System.Action<Enemy> OnSpawned;

    public string id;

    ObjectWithHealth healthComponent;
    EnemyAnimator animator;

    public bool IsEnemyTeam => true;

    public UnitInfo unitInfo = new UnitInfo();
    public UnitInfo UInfo => unitInfo;

    public CombatData data;
    public IAttackable target;

    public bool IsAlive => healthComponent.Hp > 0;

    public bool HasHp => true;

    public Queue<Vector3> checkPoints = new Queue<Vector3>();
    public Vector3? currentCheckpoint = null;

    private void Awake()
    {
        healthComponent = GetComponent<ObjectWithHealth>();
        animator = GetComponent<EnemyAnimator>();
    }

    private void Start()
    {
        Init();
        FindTarget();
        OnSpawned?.Invoke(this);
    }
    
    public void Init()
    {
        data = Database.current.GetEnemyData(id);
        healthComponent.SetHp(data.hp, data.hp);
    }

    public void FindTarget()
    {
        target = WarZone.GetClosestAttackable(transform.position, false);

        if(target == null)
        {
            unitInfo.Pause(2f);
            animator.AnimState = EnemyAnimator.State.Idle;
        }
    }

    bool prevIsSlowed = false;

    private void FixedUpdate()
    {
        bool slowed = unitInfo.IsSlowed;
        if (prevIsSlowed != slowed)
        {
            prevIsSlowed = slowed;
            if (!slowed) GetComponent<ColorableComponent>().OutlineColor = OreColor.Black;
        }

        if (currentCheckpoint != null)
        {
            if(MoveTowards(currentCheckpoint.Value))
            {
                currentCheckpoint = null;
            }
            return;
        }

        if(checkPoints.Count > 0)
        {
            currentCheckpoint = checkPoints.Dequeue();
            return;
        }

        if (unitInfo.IsPaused) return;

        if(!MyInterfaces.IsAlive(target))
        {
            FindTarget();
            return;
        }

        if (Vector2.Distance(target.gameObject.transform.position, transform.position) <= data.range)
        {
            animator.AnimState = EnemyAnimator.State.Idle;

            if (unitInfo.CanAttack)
            {
                target.Damage(data.damage);
                animator.actionQueue.Enqueue(AnimationAction.Attack);

                unitInfo.SetNextAttack(data.attackRate);
            }
        }
        else
        {
            animator.AnimState = EnemyAnimator.State.Moving;
            MoveTowards(target.gameObject.transform.position);
        }
    }

    public bool MoveTowards(Vector3 point)
    {
        animator.AnimState = EnemyAnimator.State.Moving;

        float speed = data.moveSpeed;
        if (prevIsSlowed) speed *= 0.3f;
        transform.position = Vector3.MoveTowards(transform.position, point, Time.fixedDeltaTime * speed);
        return transform.position == point;
    }

    public void OnDie()
    {
        animator.actionQueue.Enqueue(AnimationAction.Death);
        enabled = false;
        healthComponent.healthBar.gameObject.SetActive(false);
        animator.AnimState = EnemyAnimator.State.Dead;

        OnKilled?.Invoke(this);

        Destroy(gameObject, 3f);
    }

    public void Damage(int damage)
    {
        if (!IsAlive) return;
        healthComponent.Damage(damage);
    }

}


public interface IWithUnitInfo
{
    UnitInfo UInfo { get; }
}

public class UnitInfo
{
    public float nextAttack;
    public float pauseUntil;
    public float slowedUntil;

    public bool IsPaused => Time.time < pauseUntil;
    public bool IsSlowed => Time.time < slowedUntil;

    public void Pause(float time)
    {
        pauseUntil = Time.time + time;
    }

    public void Slow(float time)
    {
        slowedUntil = Time.time + time;
    }

    public void SetNextAttack(float rate)
    {
        nextAttack = Time.time + rate;
    }

    public bool CanAttack => Time.time >= nextAttack;
}

public interface IAttackable
{
    bool IsEnemyTeam { get; }
    bool IsAlive { get; }
    bool HasHp { get; }


    void Damage(int damage);

    GameObject gameObject { get; }
}
