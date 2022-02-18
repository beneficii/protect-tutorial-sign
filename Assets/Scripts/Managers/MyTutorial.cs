using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MyTutorial : MonoBehaviour
{
    public static MyTutorial current;

    public static System.Action<string> OnStepFirstComplete;
    public static System.Action<string> OnStepComplete;
    public Dictionary<string, int> stepsDict = new Dictionary<string, int>();

    private void Awake()
    {
        current = this;
    }

    private void OnEnable()
    {
        ObjectWithHealth.OnFullHp += OnFullHp;
        ObjectWithHealth.OnZeroHp += OnKilled;
        ObjectWithTutorial.OnAdded += OnAdded;

        StationExchange.OnReady += HandleReady;

        Tower.LevelUpdated += TowerLevelUpdated;

        RecipePanel.OnClose += OnRecipeRead;

        //Player actions
        PlayerCtrl.OnHammer += OnPlayerHammer;
        PlayerCtrl.OnTake += OnPlayerTake;
        PlayerCtrl.OnPlace += OnPlayerPlace;
        PlayerCtrl.OnUse += OnPlayerUse;

    }

    private void OnDisable()
    {
        ObjectWithHealth.OnFullHp -= OnFullHp;
        ObjectWithHealth.OnZeroHp -= OnKilled;
        ObjectWithTutorial.OnAdded -= OnAdded;
        RecipePanel.OnClose -= OnRecipeRead;

        StationExchange.OnReady -= HandleReady;

        Tower.LevelUpdated -= TowerLevelUpdated;

        //Player actions
        PlayerCtrl.OnHammer -= OnPlayerHammer;
        PlayerCtrl.OnTake -= OnPlayerTake;
        PlayerCtrl.OnPlace -= OnPlayerPlace;
        PlayerCtrl.OnUse -= OnPlayerUse;
    }

    public void Complete(string step, GameObject obj)
    {
        if (stepsDict.ContainsKey(step))
        {
            stepsDict[step]++;
        }
        else
        {
            stepsDict.Add(step, 1);
            OnStepFirstComplete?.Invoke(step);
        }
        OnStepComplete?.Invoke(step);
    }

    public bool IsComplete(string step, int times = 1)
    {
        if (stepsDict.TryGetValue(step, out var value))
        {
            return value >= times;
        }

        return false;
    }

    void OnReceiveAction(string action, GameObject obj)
    {
        if (obj && obj.TryGetComponent<ObjectWithTutorial>(out var tutorialObject))
        {
            Complete($"{action} {tutorialObject.id}", obj);

            if (obj.TryGetComponent<ColorableComponent>(out var colorable))
            {
                Complete($"{action} {colorable.MainColor} {tutorialObject.id}", obj);
            }
        }
        else
        {
            Complete(action, obj);
        }
    }

    void OnPlayerTake(PlayerCtrl player, GameObject obj) => OnReceiveAction("Take", obj);
    void OnPlayerPlace(PlayerCtrl player, GameObject obj) => OnReceiveAction("Place", obj);
    void OnPlayerUse(PlayerCtrl player, GameObject obj) => OnReceiveAction("Use", obj);
    void OnPlayerHammer(PlayerCtrl player, GameObject obj) => OnReceiveAction("Hammer", obj);

    void OnKilled(ObjectWithHealth obj) => OnReceiveAction("Destroy", obj.gameObject);
    void OnFullHp(ObjectWithHealth obj) => OnReceiveAction("FullHp", obj.gameObject);
    void HandleReady(StationExchange obj) => OnReceiveAction("Wait", obj.gameObject);

    void OnAdded(GameObject obj) => OnReceiveAction("Add", obj.gameObject);

    void OnRecipeRead() => OnReceiveAction("Read", null);


    void TowerLevelUpdated(Tower obj)
    {
        int level = obj.level+1;

        if (obj.TryGetComponent<ObjectWithTutorial>(out var tutorialObject))
        {
            var key = $"TowerLevel {level}";
            Complete(key, obj.gameObject);
        }
    }
}

public enum TutorialId
{
    None,
    Chest,
    Tower,
    Dummy,
    Table,
    Furnace,
    Pyramid,
    HairBot,
    Construction,
    MiniBot,
    RedBot,
    BlueBot,
    LegBot,
    Mine,
    Extractor,
    Sign,
    Wall,
    Combiner,
    TrashCan,
    BarExtractor,
    GemExtractor,
    Cooker,
    TowerExtractor,
}


public static class EnumUtil
{
    public static IEnumerable<T> GetValues<T>()
    {
        return System.Enum.GetValues(typeof(T)).Cast<T>();
    }
}