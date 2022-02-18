using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(ResourceCarrier))]
public class PlayerCtrl : MonoBehaviour
{
    public static System.Action<PlayerCtrl, GameObject> OnHammer;
    public static System.Action<PlayerCtrl, GameObject> OnTake;
    public static System.Action<PlayerCtrl, GameObject> OnPlace;
    public static System.Action<PlayerCtrl, GameObject> OnUse;

    public SpriteRenderer render;
    public Transform selector;
    public Transform gridSelector;

    public InputData inp;
    public float speed = 5;
    public float hammerSpeed { get; private set; } = 1f;

    PlayerHelperInfo helperInfo = new PlayerHelperInfo();

    CachedTarget closestInteractable = new CachedTarget();
    Vector2Int lastDirection;

    AttackJobB jobB = null;
    ProgressBar timer = null;

    public static System.Action<Transform, GameObject> OnClosestChanged;

    Rigidbody2D rb;

    BuffInfo buff;
    
    public Vector2Int InteractableDirection
    {
        get
        {
            if (!closestInteractable) return Vector2Int.zero;
            var diff = transform.position - closestInteractable.Target.transform.position;

            if (Mathf.Abs(diff.x) < Mathf.Abs(diff.y))
            {
                if(diff.y > 0)
                {
                    return Vector2Int.down;
                }
                else
                {
                    return Vector2Int.up;
                }
            }
            else
            {
                if (diff.x > 0)
                {
                    return Vector2Int.left;
                }
                else
                {
                    return Vector2Int.right;
                }
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<ResourceCarrier>().OnAssigned += HandleResourceAssigned;
        GetComponent<ResourceCarrier>().OnCleared += HandleResourceCleared;
        buff = new BuffInfo(OreColor.None);
    }

    private void Update()
    {
        if(inp.Direction != Vector2Int.zero)
        {
            lastDirection = inp.Direction;
        }

        CheckClosestInteractable();
        CheckInputActions();

        RefreshJobProgress();
        CheckBuffs();
        CheckGridPlacement();
    }

    void HandleResourceAssigned(ResourceId obj)
    {
        //check special effects and transformations
        var input = obj.data;
        var output = buff.data.TransformResource(input);
        if (!input.Equals(output)) obj.Init(output);


        //bonus actions
        if(output.type == ResourceType.Potion)
        {
            SetBuff(output.color, 10f);
            MyAudio.current.Play(Sounds.Potion);
            ResourceCarrier.Clear(this);
            return;
        }

        gridSelector.gameObject.SetActive(true);
        helperInfo.HandleResourceAssigned(this);
    }

    void HandleResourceCleared()
    {
        gridSelector.gameObject.SetActive(false);
        helperInfo.HandleResourceCleared();
    }

    bool closestNotNull = true;
    void CheckClosestInteractable()
    {
        var closest = GetClosestInteractable();

        if ((closestNotNull != closest) || (closestInteractable != closest))
        {
            OnClosestChanged?.Invoke(transform, closest);

            if(!closest)
            {
                selector.gameObject.SetActive(false);
            }
            else
            {
                selector.gameObject.SetActive(true);
                selector.position = closest.transform.position;
            }
            closestInteractable.Target = closest;
        }
        closestNotNull = closest;
    }

    public void CheckGridPlacement()
    {
        if (!gridSelector.gameObject.activeSelf) return;

        var pos = (closestInteractable && closestInteractable.canPlace)
            ? closestInteractable.Target.transform.position
            : transform.position;

        gridSelector.position = new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
    }

    void CheckBuffs()
    {
        if (buff.color == OreColor.None) return;

        if (Time.time >= buff.expireAt) SetBuff(OreColor.None);
    }

    public void SetBuff(OreColor color, float duration = 0f)
    {
        buff.data.Remove(this);

        buff = new BuffInfo(color, Time.time + duration);

        buff.data.Apply(this);

        if(color == OreColor.None)
        {
            Database.SetOutlineColor(render, OreColor.Black);
        }
        else
        {
            Database.SetOutlineColor(render, color);
        }
    }

    void CheckInteractUse()
    {
        if (inp.useButton.State != KeyState.Down) return;
        var resource = ResourceCarrier.Peek(this);

        if(closestInteractable)
        {
            // place if we have resource
            if (resource && closestInteractable.Target.TryGetComponent<IInteractablePlace>(out var placeTarget))
            {
                if ( placeTarget.CanPlace(resource))
                {
                    OnPlace?.Invoke(this, closestInteractable);
                    placeTarget.PlaceItem(this, resource);
                    MyAudio.current.Play(Sounds.PlaceItem);

                    ResourceCarrier.Clear(this);
                    return;
                }
            }

            // take if we have no resource
            if (!resource && closestInteractable.Target.TryGetComponent<IInteractableTake>(out var takeTarget))
            {
                if (takeTarget.CanTakeCount(this) > 0)
                {
                    OnTake?.Invoke(this, closestInteractable);
                    takeTarget.TakeItem(this);
                    MyAudio.current.Play(Sounds.PickupItem);
                    return;
                }
            }

            // if all else fails - try to use
            if (closestInteractable.Target.TryGetComponent<IInteractableUse>(out var useTarget))
            {
                if (useTarget.CanUse(this))
                {
                    OnUse?.Invoke(this, closestInteractable);
                    useTarget.Use(this);
                    //ToSound: Use
                    return;
                }
            }
        }

        //nothing to do, so we can place a block
        if(!resource || !TryToPutBlock(resource))
        {
            MyAudio.current.Play(Sounds.Error);
        }
    }

    void CheckInteractHammer()
    {
        if (inp.hammerButton.State != KeyState.Down) return;

        if (closestInteractable)
        {
            var resource = ResourceCarrier.Peek(this);
            if (resource)
            {
                return;
            }

            if (closestInteractable.Target.TryGetComponent<IInteractableHammer>(out var hammerTarget))
            {
                if (hammerTarget.CanHammer(this))
                {
                    StartJobB(hammerTarget);
                }

                return;
            }
        }
    }

    void CheckInputActions()
    {
        CheckInteractUse();
        CheckInteractHammer();
    }

    void StartJobB(IInteractableHammer target)
    {
        if (target.gameObject.TryGetComponent<ObjectWithTutorial>(out var tutorialObj))
        {
            hammerSpeed = buff.data.HammerSpeed(tutorialObj.id);
        }

        float totalTime = target.HammerSwingsNeeded * PlayerAnimator.defaultHammerFramerate * PlayerAnimator.attackFrames / hammerSpeed;
        jobB = new AttackJobB
        {
            target = target,
            finishTime = Time.time + totalTime,
            totalTime = totalTime,
        };
    }

    void FinishJob(bool canceled)
    {
        shouldStopSwinging = false;
        var target = jobB.target;

        if (!canceled)
        {
            if (MyInterfaces.IsAlive(target))
            {
                target.Hammer(this);
                OnHammer?.Invoke(this, target.gameObject);
            }

            if (shouldStopSwinging == true)
            {
                inp.hammerButton.Block();
            }
        }

        jobB = null;
        if (timer) Destroy(timer.gameObject);

        if(!canceled && !shouldStopSwinging)
        {
            StartJobB(target);
        }
    }

    bool shouldStopSwinging = false;
    public void StopSwinging()
    {
        shouldStopSwinging = true;
    }

    void RefreshJobProgress()
    {
        if (jobB == null) return;

        float timeLeft = jobB.finishTime - Time.time;

        if (timeLeft <= 0)
        {
            FinishJob(false);
            return;
        }

        if(inp.hammerButton.State == KeyState.Up)
        {
            FinishJob(true);
            return;
        }

        if (!timer) timer = Instantiate(MyPrefabs.current.timer, transform.position, Quaternion.identity, transform);

        timer.Set(1f - (timeLeft / jobB.totalTime));
    }

    bool TryToPutBlock(ResourceId resource)
    {
        var pos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        if (!MySpots.current.CanUse(pos)) return false;

        switch (resource.data.type)
        {
            case ResourceType.Ore:
            case ResourceType.Bar:
            case ResourceType.Gem:
                {
                    var instance = Instantiate(MyPrefabs.current.contruction, new Vector3(pos.x, pos.y), Quaternion.identity);
                    instance.PlaceItem(this, resource);
                    ResourceCarrier.Clear(this);
                }
                return true;
            case ResourceType.Upgrade_Tower:
                {
                    var instance = Instantiate(MyPrefabs.current.tower, new Vector3(pos.x, pos.y), Quaternion.identity);
                    instance.GetComponent<ColorableComponent>().MainColor = resource.data.color;
                    instance.level = ResourceCarrier.Level(this);
                    MyAudio.current.Play(Sounds.PlaceBuilding);
                    ResourceCarrier.Clear(this);

                }
                return true;
            default:
                {
                    if (Database.current.upgrades.TryGetValue(resource.data.type, out var id))
                    {
                        var instance = MyPrefabs.current.CreateObject(id);
                        instance.transform.position = new Vector3(pos.x, pos.y);
                        if(instance.TryGetComponent<ColorableComponent>(out var colorable))
                        {
                            colorable.MainColor = resource.data.color;
                        }
                        MyAudio.current.Play(Sounds.PlaceBuilding);
                        ResourceCarrier.Clear(this);
                        return true;
                    }
                }
                break;
        }

        return false;
    }

    private void FixedUpdate()
    {
        var direction = inp.Direction;

        rb.position += new Vector2(direction.x, direction.y) * speed * Time.fixedDeltaTime;
    }

    GameObject GetClosestInteractable()
    {
        float offset = 0.2f;
        float minDistance = 1f;
        GameObject closest = null;

        foreach (var item in GameObject.FindGameObjectsWithTag("Interactable"))
        {
            float distance = Vector2.Distance(transform.position + new Vector3(lastDirection.x * offset, lastDirection.y * offset), item.transform.position);

            if(distance < minDistance)
            {
                minDistance = distance;
                closest = item;
            }
        }

        return closest;
    }

    public class AttackJobB
    {
        public float finishTime;
        public float totalTime;
        public IInteractableHammer target;
    }
    
    public class CachedTarget
    {
        GameObject target;
        public GameObject Target
        {
            get => target;
            set
            {
                target = value;
                RefreshFlags();
            }
        }
        public bool canTake;
        public bool canPlace;
        public bool canHammer;
        public bool CanInteract => target && (canTake || canHammer || canPlace);

        public void SetTarget(GameObject target)
        {
            this.target = target;

            RefreshFlags();
        }

        public void RefreshFlags()
        {
            canTake = false;
            canHammer = false;
            canPlace = false;
            if (!target) return;

            if (target.TryGetComponent<IInteractableTake>(out var _)) canTake = true;
            if (target.TryGetComponent<IInteractablePlace>(out var _)) canPlace = true;
            if (target.TryGetComponent<IInteractableHammer>(out var _)) canHammer = true;
        }

        public static implicit operator GameObject(CachedTarget self)=> self.target;
        public static implicit operator bool(CachedTarget self)=> self.target;
    }
}

public interface IInteractableUse
{
    bool CanUse(PlayerCtrl player);
    void Use(PlayerCtrl player);
}

public interface IInteractableTake
{
    // amount of items that can be taken
    int CanTakeCount(PlayerCtrl player);
    void TakeItem(PlayerCtrl player);
}

public interface IInteractablePlace
{
    bool CanPlace(ResourceId resource);
    void PlaceItem(PlayerCtrl player, ResourceId item);
}

public interface IInteractableHammer
{
    GameObject gameObject { get; }
    int HammerSwingsNeeded { get; }
    bool CanHammer(PlayerCtrl player);
    void Hammer(PlayerCtrl player);
}

public class BuffInfo
{
    public OreColor color;
    public BuffBase data;
    public float expireAt;

    public BuffInfo(OreColor color, float expireAt = 0f)
    {
        this.color = color;
        this.data = BuffBase.Get(color);
        this.expireAt = expireAt;
    }
}