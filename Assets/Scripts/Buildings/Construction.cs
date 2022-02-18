using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Construction : MonoBehaviour, IInteractablePlace, IInteractableTake, IInteractableHammer, ISaveable, IInfoForDescription
{
    List<BuildingBlock> blocks = new List<BuildingBlock>();

    public int HammerSwingsNeeded => blocks.Count;

    public string Description => "Hammer to finish building";

    public void Hammer(PlayerCtrl player)
    {
        Finish();
        player.StopSwinging();
    }

    public static Vector3 GetBlockOffset(int count, ResourceType type)
    {
        return Vector3.up * Constants.pixelSize  * (count * 3 + (type == ResourceType.Gem ? 1 : 0));
    }

    void Finish()
    {
        bool placed = false;
        foreach (var item in Database.current.asset.recipes)
        {
            if(item.Match(blocks, out var color))
            {
                var instance = Instantiate(MyPrefabs.current.GetPrefab(item.result), transform.position, transform.rotation);
                if(instance.TryGetComponent<ColorableComponent>(out var colorComponent))
                {
                    colorComponent.MainColor = color;
                }
                placed = true;
                break;
            }
        }

        if (!placed)
        {
            MyEffects.current.AddEffect(AnimEffect.Error, transform.position);
            MyAudio.current.Play(Sounds.BigError);

        }
        else
        {
            MyAudio.current.Play(Sounds.PlaceBuilding);
            Destroy(gameObject);
        }
    }

    public bool CanHammer(PlayerCtrl player) => blocks.Count > 1;

    public bool CanPlace(ResourceId resource) => resource.data.type != ResourceType.Ore;

    public void PlaceItem(PlayerCtrl player, ResourceId item)
    {
        AddBlock(item.data);
    }

    void AddBlock(ResourceData data)
    {
        int offset = data.type == ResourceType.Gem ? 1 : 0;

        var instance = Instantiate(
            MyPrefabs.current.block,
            transform.position + GetBlockOffset(blocks.Count, data.type),
            Quaternion.identity,
            transform);

        MyAudio.current.Play(Sounds.PlaceBlock);

        instance.resource = data;
        instance.render.sortingOrder = blocks.Count - 2;
        blocks.Add(instance);
    }

    public int CanTakeCount(PlayerCtrl player) => 1;

    public void TakeItem(PlayerCtrl player)
    {
        var res = blocks.Last().resource;
        Destroy(blocks.Last().gameObject);
        blocks.RemoveAt(blocks.Count - 1);

        if (blocks.Count == 0)
        {
            GetComponent<SpotGridItem>().Remove();
        }

        ResourceCarrier.Assign(player, res);
    }

    public string Save()
    {
        return ResourceData.ListToString(blocks.Select(x => x.resource));
    }

    public void Load(string line)
    {
        foreach (var item in ResourceData.StringToList(line))
        {
            AddBlock(item);
        }
    }
}
