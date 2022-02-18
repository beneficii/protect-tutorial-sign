using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ColorableComponent))]
[RequireComponent(typeof(BuildingAnimator))]
public class StationContainer : MonoBehaviour, IInteractableHammer, ISaveable, IInfoForDescription
{
    public ResourceType outputResource;

    public int HammerSwingsNeeded { get; private set; }

    ColorableComponent colorable;
    OreColor color => colorable.MainColor;

    public string Description => $"Hammer to get {color} {outputResource}";

    void Awake()
    {
        colorable = GetComponent<ColorableComponent>();
    }

    void Start()
    {
        Init();
        StartCoroutine(Appear());
    }


    IEnumerator Appear()
    {
        var animator = GetComponent<BuildingAnimator>();
        animator.SetAnimation(BuildingAnimator.AnimState.Working, false);
        yield return new WaitUntil(animator.Finished);
    }

    void Init()
    {
        switch (color)
        {
            case OreColor.Grey:
                HammerSwingsNeeded = 2;
                break;
            case OreColor.Red:
                HammerSwingsNeeded = 3;
                break;
            case OreColor.Blue:
                HammerSwingsNeeded = 5;
                break;
            default:
                HammerSwingsNeeded = 5;
                break;
        }
    }

    public void Hammer(PlayerCtrl player)
    {
        var item = MyPrefabs.current.CreateResource(new ResourceData(outputResource, color));

        ResourceCarrier.Assign(player, item.gameObject);
        player.StopSwinging();
        
    }

    public bool CanHammer(PlayerCtrl player) => ResourceCarrier.CanAssign(player);

    public string Save()
    {
        return "";
    }

    public void Load(string line)
    {
        //nothing
        Init();
    }
}
