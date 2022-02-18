using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//[RequireComponent(typeof(ObjectWithTutorial))]
public class StationBase : MonoBehaviour, IInteractablePlace, IInteractableTake, ISaveable, IInfoForDescription
{
    public List<Transform> spots = new List<Transform>();
    Queue<ResourceId> resources = new Queue<ResourceId>();

    public string Description => "Table: Use to place/take items";

    Transform FindSpot()
    {
        if (resources.Count >= spots.Count) return null;

        foreach (var spot in spots)
        {
            if (spot.childCount == 0) return spot;
        }

        return null;
    }

    public bool AddResource(ResourceId item)
    {
        var spot = FindSpot();

        if (spot == null) return false;

        resources.Enqueue(item);
        ResourceCarrier.Assign(spot, item.gameObject);

        return true;
    }

    public bool CanPlace(ResourceId resource) => FindSpot();

    public void PlaceItem(PlayerCtrl player, ResourceId item)
    {
        AddResource(item);
    }

    public int CanTakeCount(PlayerCtrl player) => resources.Count;


    public void TakeItem(PlayerCtrl player)
    {
        var item = resources.Dequeue();

        ResourceCarrier.Assign(player, item.gameObject);
    }

    public string Save()
    {
        return ResourceData.ListToString(resources.Select(x=>x.data));
    }

    public void Load(string line)
    {
        foreach (var item in ResourceData.StringToList(line))
        {
            AddResource(MyPrefabs.current.CreateResource(item));
        }
    }
}

