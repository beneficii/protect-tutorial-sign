using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ResourceCarrier))]
[RequireComponent(typeof(BuildingAnimator))]
//[RequireComponent(typeof(ObjectWithTutorial))]
public class StationExchange : MonoBehaviour, IInteractablePlace, IInteractableTake, IInteractableHammer, ISaveable, IInfoForDescription
{
    public static event System.Action<StationExchange> OnReady;

    public ResourceType inputResource;
    public ResourceType outputResource;
    public ResourceType disasembleResource = ResourceType.None;
    public float delay = 2f;
    public int maxColors = 1;

    public SpriteRenderer render;
    BuildingAnimator animator;
    ObjectWithTutorial tutorialObj;

    bool working = false;

    List<OreColor> colors = new List<OreColor>();

    private void Awake()
    {
        animator = GetComponent<BuildingAnimator>();
        tutorialObj = GetComponent<ObjectWithTutorial>();
    }

    private void Start()
    {
        Database.ClearColors(render);
    }

    public void AddResource(OreColor color)
    {
        colors.Add(color);
        Database.SetColor(render, color, colors.Count);
        if (colors.Count >= maxColors)
        {
            if(maxColors == 1)
            {
                StartCoroutine(ExchangeRoutine(color, delay));
            }
            else if (Database.TryCombine(colors, out var outputColor))
            {
                Database.SetColor(render, outputColor, 3);
                StartCoroutine(ExchangeRoutine(outputColor, delay));
            }
            else
            {
                StartCoroutine(ExchangeRoutine(OreColor.None, 2));
            }
        }
    }

    OreColor workingColor = OreColor.None;

    public int HammerSwingsNeeded => 3;

    public string Description
    {
        get
        {
            switch (tutorialObj.id)
            {
                case TutorialId.Combiner: return $"{tutorialObj.id}: Combines 2 basic ores";
                case TutorialId.Cooker:
                    {
                        var potion = ResourceCarrier.Peek(this);
                        if (potion != null) return $"{potion.data.ToLine()}: {BuffBase.Get(potion.data.color).Description}";
                    }
                    break;
            }

            return $"{tutorialObj.id}: Accepts {inputResource}, produces {outputResource}";
        }
    }

    IEnumerator ExchangeRoutine(OreColor outputColor, float delay)
    {
        workingColor = outputColor;
        working = true;
        var timer = Instantiate(MyPrefabs.current.timer, transform.position, Quaternion.identity, transform);

        animator.SetAnimation(BuildingAnimator.AnimState.Working);

        int totalFrames = timer.TotalFrames;
        for (int i = 0; i < totalFrames; i++)
        {
            yield return new WaitForSeconds(delay / totalFrames);
            timer.Set(i, totalFrames);
        }

        Database.ClearColors(render);
        colors.Clear();
        animator.SetAnimation(BuildingAnimator.AnimState.Iddle);
        if(outputColor != OreColor.None)
        {
            ResourceCarrier.Assign(this, new ResourceData(outputResource, outputColor));
        }
        Destroy(timer.gameObject);
        working = false;
        workingColor = OreColor.None;
        OnReady?.Invoke(this);
    }

    public bool CanPlace(ResourceId resource) =>
        !working
        && resource.data.type == inputResource
        && !ResourceCarrier.Peek(this);

    public void PlaceItem(PlayerCtrl player, ResourceId item)
    {
        AddResource(item.data.color);
    }

    public int CanTakeCount(PlayerCtrl player) =>
        (!working && ResourceCarrier.Peek(this)) ? 1 : 0;

    public void TakeItem(PlayerCtrl player)
    {
        ResourceCarrier.Transfer(this, player);
    }

    public string Save()
    {
        if(workingColor != OreColor.None)
        {
            return new ResourceData(inputResource, workingColor).ToString();
        }
        var item = ResourceCarrier.Peek(this);
        if (item)
        {
            return item.data.ToString();
        }

        return "";
    }

    public void Load(string line)
    {
        if (string.IsNullOrEmpty(line)) return;

        ResourceCarrier.Assign(this, ResourceData.FromString(line));
    }

    public bool CanHammer(PlayerCtrl player) => disasembleResource != ResourceType.None;

    public void Hammer(PlayerCtrl player)
    {
        GetComponent<SpotGridItem>().Remove();
        ResourceCarrier.Assign(player, new ResourceData(disasembleResource, GetComponent<ColorableComponent>()?.MainColor??OreColor.None));
        player.StopSwinging();
    }
}
