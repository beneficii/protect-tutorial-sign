using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StationTrashCan : MonoBehaviour, IInteractablePlace, IInfoForDescription, IDormant
{
    public Sprite workSprite;
    public SpriteRenderer render;

    public ResourceData? lastResource = null;

    public string Description => "Trash Can: Disposes anything you throw in";

    bool activated = true;
    public bool Active { set => activated = value; }

    public bool CanPlace(ResourceId resource) => activated;

    public void PlaceItem(PlayerCtrl player, ResourceId item)
    {
        lastResource = item.data;

        ResourceCarrier.Clear(player);
        StartCoroutine(Animate());
    }

    public void Load(string line)
    {
        //throw new System.NotImplementedException();
    }

    public string Save()
    {
        return "";
        //throw new System.NotImplementedException();
    }

    IEnumerator Animate()
    {
        var oldSprite = render.sprite;
        render.sprite = workSprite;
        yield return new WaitForSeconds(Constants.frameDelay);
        render.sprite = oldSprite;
    }
}

public interface IDormant
{
    bool Active { set; }
}
