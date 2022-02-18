using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ColorableComponent))]
[RequireComponent(typeof(SpotGridItem))]
[RequireComponent(typeof(ObjectWithTutorial))]
public class Tower : MonoBehaviour, IWithUnitInfo, IInfoForRange, IInteractableHammer, IInteractablePlace, IInfoForDescription, ISaveable
{
    public static event System.Action<Tower> LevelUpdated;
    public const int maxLevel = 6;

    CombatData data;
    public Transform spawnForProjectile;
    public List<Sprite> sprites;

    public IAttackable target;

    public UnitInfo unitInfo = new UnitInfo();
    public UnitInfo UInfo => unitInfo;

    bool targetIsEnemy = true;
    bool searchForClosest = true;

    public int level = 0;
    
    public int Range => (data != null)?data.range:1;

    public OreColor MainColor => GetComponent<ColorableComponent>().MainColor;

    public int HammerSwingsNeeded => 2;

    public string Description => (data != null)?$"{data.name} Tower {data.level}: {data.description} | dps: {data.damage / data.attackRate : 0.00}":"";

    ProjectileInfo projectileInfo;

    private void Start()
    {
        Init();
        FindTarget();
    }

    public void Init()
    {
        InitLevel();

        switch (MainColor)
        {
            case OreColor.Grey:
                targetIsEnemy = false;
                break;
            case OreColor.Blue:
                searchForClosest = false;
                break;
        }


        if (MainColor == OreColor.Grey) targetIsEnemy = false;
    }

    public void InitLevel()
    {
        data = Database.current.GetTowerData(MainColor, level);
        GetComponent<SpriteRenderer>().sprite = sprites[level];

        projectileInfo = GetProjectileInfo();
    }

    ProjectileInfo GetProjectileInfo()
    {
        switch (GetComponent<ColorableComponent>().MainColor)
        {
            case OreColor.Grey: return new ProjectileInfo(HealTowerAction);
            case OreColor.Blue: return new ProjectileInfo(IceTowerAction);
            case OreColor.Teal: return new ProjectileInfo(DefaultAction, 0, 10f);
            case OreColor.Orange: return new ProjectileInfo(ChainTowerAction, data.targets+1);
            case OreColor.Brown: return new ProjectileInfo(SplashTowerAction);
        }

        return new ProjectileInfo(DefaultAction);
    }

    public void FindTarget()
    {
        if(searchForClosest)
        {
            target = WarZone.GetClosestAttackable(transform.position, targetIsEnemy, data.range, true);
        }
        else
        {
            target = WarZone.GetFarestAttackable(transform.position, targetIsEnemy, data.range, true);
        }

        if (target == null)
        {
            unitInfo.Pause(1f); //to save some cpu
            //animator.AnimState = EnemyAnimator.State.Idle;
        }
    }

    private void FixedUpdate()
    {
        if (unitInfo.IsPaused) return;

        if (unitInfo.CanAttack)
        {
            if (!MyInterfaces.IsAlive(target) || !target.IsAlive)
            {
                FindTarget();
                return;
            }

            if(!searchForClosest)
            {
                FindTarget(); //find target every time
                if (!MyInterfaces.IsAlive(target)) return;
            }

            if (targetIsEnemy  || !target.gameObject.GetComponent<ObjectWithHealth>().IsFullHp)
            {
                var projectile = Projectile.Spawn(target, projectileInfo);
                projectile.transform.position = spawnForProjectile.position;
                projectile.source = this.gameObject;
                ColorableComponent.SetColors(projectile, GetComponent<ColorableComponent>().MainColor);
            }

            unitInfo.SetNextAttack(data.attackRate);
        }
    }

    public bool CanHammer(PlayerCtrl player) => !ResourceCarrier.Peek(player);

    public void Hammer(PlayerCtrl player)
    {
        GetComponent<SpotGridItem>().Remove();
        ResourceCarrier.Assign(player, new ResourceData(ResourceType.Upgrade_Tower, GetComponent<ColorableComponent>().MainColor, level), level);
        player.StopSwinging();
    }

    void IceTowerAction(IAttackable attackable, int value)
    {
        if (attackable.gameObject.TryGetComponent<IWithUnitInfo>(out var enemy))
        {
            var unitInfo = enemy.UInfo;
            unitInfo.Slow(2f);
            attackable.gameObject.GetComponent<ColorableComponent>().OutlineColor = MainColor;
        }
    }

    void HealTowerAction(IAttackable attackable, int value)
    {
        attackable.gameObject.GetComponent<ObjectWithHealth>()?.Heal(data.damage);
    }

    void ChainTowerAction(IAttackable attackable, int value)
    {
        value--;
        if (value > 0)
        {
            var nextTarget = WarZone.GetClosestAttackable(attackable.gameObject.transform.position, true, data.splash);
            if (nextTarget != null)
            {
                var projectile = Projectile.Spawn(nextTarget, new ProjectileInfo
                {
                    value = value,
                    speed = projectileInfo.speed,
                    action = ChainTowerAction
                });
                projectile.source = this.gameObject;
                projectile.transform.position = attackable.gameObject.transform.position;
                ColorableComponent.SetColors(projectile, GetComponent<ColorableComponent>().MainColor);
            }
        }
        attackable.Damage(data.damage);
    }

    void SplashTowerAction(IAttackable attackable, int value)
    {
        var list = WarZone.GetAttackables(attackable.gameObject.transform.position, targetIsEnemy, data.splash);
        foreach (var item in WarZone.GetAttackables(attackable.gameObject.transform.position, targetIsEnemy, data.splash))
        {
            item.Damage(data.damage);
        }
    }

    void DefaultAction(IAttackable attackable, int value)
    {
        attackable.Damage(data.damage);
    }

    public bool CanPlace(ResourceId resource)
    {
        return resource.data.color == MainColor
            && resource.data.type == ResourceType.Upgrade_Tower
            && level < maxLevel;
    }

    public void PlaceItem(PlayerCtrl player, ResourceId item)
    {
        level = Mathf.Clamp(level + ResourceCarrier.Level(player) + 1, 0, maxLevel);

        MyEffects.current.AddEffect(AnimEffect.Upgrade, transform.position);
        MyAudio.current.Play(Sounds.BuildingUpgrade);

        InitLevel();
        LevelUpdated?.Invoke(this);
    }

    public string Save()
    {
        return level.ToString();
    }

    public void Load(string line)
    {
        int.TryParse(line, out level);
    }
}
