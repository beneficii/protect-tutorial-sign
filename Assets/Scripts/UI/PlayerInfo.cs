using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHelperInfo
{
    List<GameObject> placedArrows = new List<GameObject>();

    public void HandleResourceAssigned(PlayerCtrl player)
    {
        var resource = ResourceCarrier.Peek(player);
        if (!resource) return;

        foreach (var item in GameObject.FindGameObjectsWithTag("Interactable"))
        {
            if(item.TryGetComponent<IInteractablePlace>(out var placeTarget) && placeTarget.CanPlace(resource))
            {
                float delay = 0f;
                if(item.TryGetComponent<StationTrashCan>(out var _)) delay = 12f; //let's not lead people to trashcan instantly

                var instance = MyPrefabs.current.CreateArrow(MyPrefabs.ArrowSprite.CanPlace, item.transform.position, false, delay).gameObject;
                placedArrows.Add(instance);
            }
        }
    }

    public void HandleResourceCleared()
    {
        foreach (var item in placedArrows)
        {
            GameObject.Destroy(item);
        }
        placedArrows.Clear();
    }
}
