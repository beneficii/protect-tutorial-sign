using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ObjectWithHealth))]
public class StationWall : MonoBehaviour, IKillable, IInteractableHammer, IAttackable, IInfoForDescription
{
    public ResourceData resourceToHeal;
    public int healAmount = 2;

    ObjectWithHealth healthComponent;

    public int HammerSwingsNeeded => 1;

    public bool IsEnemyTeam => false;

    public bool IsAlive => healthComponent.Hp > 0;
    public bool HasHp => true;


    public string Description => $"Wall: Hammer to repair";

    float lowHpCooldown = 0f;

    private void Awake()
    {
        healthComponent = GetComponent<ObjectWithHealth>();
        healthComponent.OnChanged += HandleHpChange;
    }
    
    void HandleHpChange(int current, int max)
    {
        if (Time.time >= lowHpCooldown && current < 10)
        {
            MyEffects.current.AddEffect(AnimEffect.Warning, transform.position, 3);
            lowHpCooldown = Time.time + 10f;
        }
    }


    public void OnDie()
    {
        Destroy(gameObject);
    }

    public bool CanHammer(PlayerCtrl player) => true;

    public void Hammer(PlayerCtrl player)
    {
        healthComponent.Heal(healAmount);
        if(healthComponent.IsFullHp)
        {
            player.StopSwinging();
        }
    }

    public void Damage(int damage)
    {
        healthComponent.Damage(damage);
    }
}
