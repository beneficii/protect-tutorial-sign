using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(ColorableComponent))]
[RequireComponent(typeof(BuildingAnimator))]
public class StationExtractor : MonoBehaviour, IInteractableTake, IInteractablePlace, ISaveable, IInfoForDescription
{
    public ResourceType type;
    public float miningDelay = 3f;
    public List<Transform> spots = new List<Transform>();
    public SDictResourceToBuilding upgrades = new SDictResourceToBuilding();

    OreColor MainColor => GetComponent<ColorableComponent>().MainColor;

    bool overflown = true;
    bool OverFlown
    {
        get => overflown;
        set
        {
            if(overflown != value)
            {
                overflown = value;
                animator.SetAnimation(value ? BuildingAnimator.AnimState.Iddle : BuildingAnimator.AnimState.Working);
                if(!value)
                {
                    nextOre = Time.time + miningDelay;
                }
            }
        }
    }

    public string Description => $"Extractor: Produces {MainColor} {type}";

    BuildingAnimator animator;

    float nextOre;

    Transform FindSpot()
    {
        foreach (var spot in spots)
        {
            if (spot.childCount == 0) return spot;
        }

        return null;
    }

    void RecheckSpots()
    {
        OverFlown = FindSpot() == null;
    }

    private void Start()
    {
        animator = GetComponent<BuildingAnimator>();
        nextOre = Time.time + miningDelay;
        RecheckSpots();
    }

    private void Update()
    {
        if (overflown) return;

        if(Time.time >= nextOre)
        {
            var spot = FindSpot();

            if(spot)
            {
                ResourceCarrier.Assign(spot, new ResourceData(type, MainColor));
                nextOre = Time.time + miningDelay;
            }

            RecheckSpots();
        }
    }

    public int CanTakeCount(PlayerCtrl player)=> GetComponentsInChildren<ResourceId>().Length;

    public void TakeItem(PlayerCtrl player)
    {
        var item = GetComponentInChildren<ResourceId>();
        ResourceCarrier.Assign(player, item.gameObject);

        RecheckSpots();
    }

    public string Save()
    {
        return "";
        //return ResourceData.ListToString(GetComponentsInChildren<ResourceId>().Select(x => x.data));
    }

    public void Load(string line)
    {
        //nothin
    }

    public bool CanPlace(ResourceId resource) => upgrades.Contains(resource.data.type);

    public void PlaceItem(PlayerCtrl player, ResourceId item)
    {
        var id = upgrades.Get(item.data.type);
        var prefab = MyPrefabs.current.GetPrefab(id);

        var instance = Instantiate(prefab, transform.position, Quaternion.identity);
        if(instance.TryGetComponent<ColorableComponent>(out var colorable))
        {
            colorable.MainColor = MainColor;
        }
        MyEffects.current.AddEffect(AnimEffect.Upgrade, transform.position);
        MyAudio.current.Play(Sounds.BuildingUpgrade);


        Destroy(gameObject);
    }
}
