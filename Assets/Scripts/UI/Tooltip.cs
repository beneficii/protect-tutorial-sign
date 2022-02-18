using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public InputData input;

    public static Tooltip current;

    public TextMeshPro txtInfo;
    public TextMeshPro txtTutorialInfo;
    public SpriteRenderer rangeRender;
    public List<Transform> placementSpots;
    public BigHealthBar healthBar;

    private void Awake()
    {
        current = this;
    }
    
    private void OnEnable()
    {
        PlayerCtrl.OnClosestChanged += PlayerClosestChanged;
        MyWaves.OnWaveChanged += OnNextWave;
    }

    private void OnDisable()
    {
        PlayerCtrl.OnClosestChanged -= PlayerClosestChanged;
        MyWaves.OnWaveChanged += OnNextWave;
    }

    public void PlayerClosestChanged(Transform player, GameObject obj)
    {
        bool showRange = false;

        Hide(txtInfo);

        if(obj)
        {
            if(obj.TryGetComponent<IInfoForDescription>(out var descriptionComponent))
            {
                Show(txtInfo, descriptionComponent.Description);
            }

            if (obj.TryGetComponent<IInfoForRange>(out var rangeComponent))
            {
                rangeRender.sprite = Database.current.GetRangeSprite(rangeComponent.Range);
                rangeRender.transform.position = obj.transform.position;
                Database.SetColor(rangeRender, rangeComponent.MainColor);
                showRange = true;
            }

            if (obj.TryGetComponent<ObjectWithHealth>(out var healthObj))
            {
                healthBar.gameObject.SetActive(true);
                healthBar.Bind(healthObj);
            }
        }
        else
        {
            healthBar.Bind(null);
            healthBar.gameObject.SetActive(false);
        }

        rangeRender.gameObject.SetActive(showRange);
    }

    /*
    void SetFuthurerPosition(Transform source)
    {
        float distanceMax = float.MinValue;
        Transform futhurest = null;
        Vector2 sourcePos = source.transform.position + Vector3.up * 2; //some offset to make bot panel show more

        foreach (var item in placementSpots)
        {
            float distance = Vector2.Distance(sourcePos, item.transform.position);

            if(distance > distanceMax)
            {
                distanceMax = distance;
                futhurest = item;
            }
        }

        txtInfo.transform.parent.transform.position = futhurest.position;
    }*/


    static void Show(TextMeshPro text, string message, string description = null)
    {
        if (!text) return;
        if (description == null)
        {
            text.SetText(message);
        }
        else
        {
            text.SetText($"<#eae1f0>{message}: <#7e7185>{description}");
        }

        if(text) text.transform.parent.gameObject.SetActive(true);
    }

    static void Hide(TextMeshPro text)
    {
        if(text) text.transform.parent.gameObject.SetActive(false);
    }

    void OnNextWave(CombatWaveData data)
    {
        string message = data?.message;
        if(string.IsNullOrEmpty(message))
        {
            Hide(txtTutorialInfo);
        }
        else
        {
            Show(txtTutorialInfo, input.AddControlsToString(message));
        }
    }
}

interface IInfoForRange
{
    int Range { get; } 
    OreColor MainColor { get; }
}

interface IInfoForDescription
{
    string Description { get; }
}