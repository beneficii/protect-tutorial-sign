using UnityEngine;
using System.Collections;

public class ResourceCarrier : MonoBehaviour
{
    public event System.Action<ResourceId> OnAssigned;
    public event System.Action OnCleared;

    public GameObject currentItem { get; private set; }
    public int currentItemLevel = 0;
    public Transform carryParent;

    public static bool Transfer(MonoBehaviour from, MonoBehaviour to)
    {
        var source = from.GetComponent<ResourceCarrier>();

        if (source == null || source.currentItem == null) return false; //nothing to give

        if(Assign(to, source.currentItem, source.currentItemLevel))
        {
            source.currentItem = null;
            source.OnCleared?.Invoke();
            return true;
        }

        return false;
    }

    public static void Assign(Transform target, GameObject item, int level = 0)
    {
        item.transform.parent = target;
        item.transform.localPosition = Vector3.zero;
    }

    public static void Assign(Transform target, ResourceData data, int level = 0)
    {
        var item = MyPrefabs.current.CreateResource(data, level);
        item.transform.parent = target;
        item.transform.localPosition = Vector3.zero;
    }

    public static bool Assign(MonoBehaviour to, GameObject item, int level = 0)
    {
        if (to.TryGetComponent<ResourceCarrier>(out var target) && target.currentItem == null)
        {
            target.currentItem = item;
            item.transform.parent = target.carryParent;
            item.transform.localPosition = Vector3.zero;
            target.currentItemLevel = level;
            if(target.currentItem.TryGetComponent<ResourceId>(out var resourceObj))
            {
                target.OnAssigned?.Invoke(resourceObj);
            }
            return true;
        }

        return false; //can't carry more
    }

    public static int Level(MonoBehaviour to)
    {
        if (to.TryGetComponent<ResourceCarrier>(out var target) && target.currentItem != null)
        {
            return target.currentItemLevel;
        }

        return -1;
    }

    public static bool Assign(MonoBehaviour to, ResourceData data, int level = 0)
    {
        var instance = MyPrefabs.current.CreateResource(data, level);

        if(!Assign(to, instance.gameObject, level))
        {
            Destroy(instance.gameObject);
            return false;
        }

        return true;
    }

    public static ResourceId Peek(MonoBehaviour from)
    {
        var source = from.GetComponent<ResourceCarrier>();
        return source.currentItem?.GetComponent<ResourceId>();
    }

    public static void Clear(MonoBehaviour from, bool delete = true)
    {
        var source = from.GetComponent<ResourceCarrier>();
        if(source.currentItem)
        {
            if(delete && source.currentItem.transform.parent == source.carryParent) Destroy(source.currentItem);
            source.currentItem = null;
            source.OnCleared?.Invoke();
        }
    }
    public static bool CanAssign(MonoBehaviour to)
    {
        if(to.TryGetComponent<ResourceCarrier>(out var target) && target.currentItem == null)
        {
            return true;
        }
        return false; //can't carry more
    }

}
