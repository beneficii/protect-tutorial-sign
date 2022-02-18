using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RecipePanel : MonoBehaviour
{
    public static System.Action<CombatRecipeData> OnUnlocked;
    public static System.Action OnClose;

    public InputData inputData;

    public GameObject panel;

    public List<RecipeCard> cards;
    public List<GameObject> covers;
    public List<TutorialId> unlocked = new List<TutorialId>();
    int idx = 0;

    float showCooldown;

    public static RecipePanel current;

    private void Awake()
    {
        current = this;
    }

    private void Start()
    {
        //AddAll();
    }

    public void Show()
    {
        if (Time.time < showCooldown) return;

        panel.SetActive(true);
    }

    public void AddAll()
    {
        idx = 0;
        foreach (var item in Database.current.asset.recipes)
        {
            Add(item);
        }
    }

    public void Uncover(int idx)
    {
        idx--;

        covers[idx].SetActive(false);
    }

    public void Add(CombatRecipeData data)
    {
        if (idx >= cards.Count) return; // we ran out of cards. should take care of it

        var card = cards[idx];
        card.Init(data);
        card.gameObject.SetActive(true);
        unlocked.Add(data.result);
        OnUnlocked?.Invoke(data);
        idx++;
    }

    private void Update()
    {
        if (!panel.activeSelf) return;

        if (inputData.hammerButton.State == KeyState.Down
            || inputData.useButton.State == KeyState.Down
            || inputData.Direction != Vector2Int.zero)
        {
            showCooldown = Time.time + 0.2f;
            panel.SetActive(false);
            OnClose?.Invoke();
        }
    }

}
