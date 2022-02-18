using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(ColorableComponent))]
public class Projectile : MonoBehaviour
{
    [HideInInspector] public IAttackable target;
    [HideInInspector] public GameObject source;

    public float speed = 5f;

    public int damage;
    System.Action<IAttackable, int> action;

    public ProjectileInfo info;

    public void Init(IAttackable target, ProjectileInfo info)
    {
        this.target = target;
        this.damage = info.value;
        this.action = info.action;
        speed = info.speed;
        this.info = info;
    }

    private void FixedUpdate()
    {
        if (!MyInterfaces.IsAlive(target) || !source)
        {
            Destroy(gameObject);
            return;
        }

        var targetPos = target.gameObject.transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.fixedDeltaTime);

        if(transform.position == targetPos)
        {
            action?.Invoke(target, info.value);

            Destroy(gameObject);
        }
    }

    public static Projectile Spawn(IAttackable target, ProjectileInfo info)
    {
        var instance = Instantiate(MyPrefabs.current.projectile);
        instance.Init(target, info);

        return instance;
    }
}

public class ProjectileInfo
{
    public int value;
    public System.Action<IAttackable, int> action;
    public float speed = 5f;

    public ProjectileInfo()
    {
    }

    public ProjectileInfo(Action<IAttackable, int> action, int value = 0, float speed = 5f)
    {
        this.action = action;
        this.value = value;
        this.speed = speed;
    }
}