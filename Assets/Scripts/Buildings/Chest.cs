using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(BuildingAnimator))]
[RequireComponent(typeof(SpotGridItem))]
[RequireComponent(typeof(ObjectWithTutorial))]
public class Chest : MonoBehaviour, IInteractableHammer, ISaveable, IInfoForDescription
{
    public int swings = 1;
    public int HammerSwingsNeeded => swings;

    public string Description => string.Join(", ", resources.Select(x => x.ToLine()));

    public Queue<ResourceData> resources;
    bool ready = false;

    public bool CanHammer(PlayerCtrl player) => ready && ResourceCarrier.Peek(player) == null;

    public void Hammer(PlayerCtrl player)
    {
        if(resources.Count > 0)
        {
            ResourceCarrier.Assign(player, resources.Dequeue());
        }

        if (resources.Count == 0)
        {
            GetComponent<SpotGridItem>().Remove();
        }
        player.StopSwinging();
    }

    void Start()
    {
        StartCoroutine(Appear());
    }


    IEnumerator Appear()
    {
        ready = false;
        var animator = GetComponent<BuildingAnimator>();
        animator.SetAnimation(BuildingAnimator.AnimState.Working, false);
        yield return new WaitUntil(animator.Finished);
        ready = true;
    }

    public static void Spawn(ResourceData resource)
    {
        Spawn(new List<ResourceData> { resource });
    }

    public static void Spawn(List<ResourceData> resources)
    {
        Vector2Int pos = MySpots.current.RandomFreeSpot();
        MySpots.current.taken.Add(pos);

        var instance = Instantiate(MyPrefabs.current.chest, new Vector3(pos.x, pos.y), Quaternion.identity);
        instance.resources = new Queue<ResourceData>(resources);
    }

    public string Save()
    {
        return ResourceData.ListToString(resources);
    }

    public void Load(string line)
    {
        resources = new Queue<ResourceData>(ResourceData.StringToList(line));
        ready = true;
    }
}
