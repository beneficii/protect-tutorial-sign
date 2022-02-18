using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TheSign : MonoBehaviour, IInfoForDescription, IAttackable, IInteractableUse
{
    public static event System.Action OnSignDestroyed;

    public string Description => "Tutorial Sign: Use to see recipes";

    public SpriteRenderer render;

    public Sprite activeSprite;
    Sprite defaultSprite;

    public bool IsEnemyTeam => false;
    public bool IsAlive { get; private set; } = true;
    public bool HasHp => false;

    bool hasNotification;
    bool HasNotification
    {
        get => hasNotification;
        set
        {
            hasNotification = value;
            if(!value)
            {
                render.sprite = defaultSprite;
            }
        }
    }

    float nextFrame;
    float frameIdx = 0;

    private void OnEnable()
    {
        RecipePanel.OnUnlocked += RecipeUnlocked;
    }

    private void OnDisable()
    {
        RecipePanel.OnUnlocked -= RecipeUnlocked;
    }

    void RecipeUnlocked(object _)
    {
        HasNotification = true;
    }

    private void Awake()
    {
        defaultSprite = render.sprite;
    }

    public void Damage(int damage)
    {
        if (!IsAlive) return;
        IsAlive = false;
        OnSignDestroyed?.Invoke();
        GameEvents.current.Finish(false);
        Destroy(gameObject);
    }

    public bool CanUse(PlayerCtrl player) => true;

    public void Use(PlayerCtrl player)
    {
        RecipePanel.current.Show();
        HasNotification = false;
    }

    private void Update()
    {
        if (!HasNotification) return;

        if(Time.time >= nextFrame)
        {
            frameIdx = (frameIdx + 1) % 2;
            if(frameIdx == 0)
            {
                render.sprite = defaultSprite;
            }
            else
            {
                render.sprite = activeSprite;
            }

            nextFrame = Time.time + 0.75f;
        }
    }
}
