using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyHelpArrow : MonoBehaviour
{
    //public static MyHelpArrow current;

    Dictionary<string, Transform> arrows = new Dictionary<string, Transform>();

    private void Awake()
    {
        //current = this;
    }

    private void OnEnable()
    {
        MyWaves.OnWaveChanged += HandleWaveChanged;
        MyTutorial.OnStepComplete += HandleTutorialStepDone;
    }

    private void OnDisable()
    {
        MyWaves.OnWaveChanged -= HandleWaveChanged;
        MyTutorial.OnStepComplete -= HandleTutorialStepDone;
    }

    void HandleWaveChanged(CombatWaveData data)
    {
        foreach (var item in arrows)
        {
            if(item.Value) Destroy(item.Value.gameObject);
        }
        arrows.Clear();

        if (!string.IsNullOrEmpty(data.tutorialKey) && !MyTutorial.current.IsComplete(data.tutorialKey))
        {
            AddArrow(data.tutorialKey);
        }

        if (!string.IsNullOrEmpty(data.tutorialKeyRepeatable))
        {
            AddArrow(data.tutorialKeyRepeatable);
        }
    }

    void HandleTutorialStepDone(string step)
    {
        if(arrows.ContainsKey(step))
        {
            var item = arrows[step];
            if (item) Destroy(item.gameObject);
            arrows.Remove(step);
        }
    }

    void AddArrow(string tutorialStep)
    {
        if (arrows.ContainsKey(tutorialStep)) return;

        var args = tutorialStep.Split(' ');

        void AddArrorForObj(bool hammer)
        {
            int idx = 1;

            var colorId = OreColor.Any;

            if (System.Enum.TryParse(args[idx], true, out OreColor color))
            {
                colorId = color;
                idx++;
            }

            if (System.Enum.TryParse(args[idx], true, out TutorialId id))
            {
                var one = ObjectWithTutorial.FindOne(id, colorId);
                if(one)
                {
                    var arrow = CreateArrow(one.transform.position, hammer);
                    arrows.Add(tutorialStep, arrow);
                } 
                else
                {
                    Debug.LogError($"Tutorial object for '{tutorialStep}' not found!");
                }
            }
        }


        switch (args[0])
        {
            case "Take":
            case "Place":
            case "Use":
                AddArrorForObj(false);
                break;
            case "Hammer":
            case "FullHp":
                AddArrorForObj(true);
                break;
            case "Read":
                {
                    var first = ObjectWithTutorial.FindOne(TutorialId.Sign);

                    if (first) arrows.Add(tutorialStep, CreateArrow(first.transform.position));
                }
                break;
                
            default:
                break;
        }
    }

    Transform CreateArrow(Vector2 pos, bool hammer = false)
    {
        var type = hammer ? MyPrefabs.ArrowSprite.TutorialHammer : MyPrefabs.ArrowSprite.TutorialUse;
        return MyPrefabs.current.CreateArrow(type, pos, true).transform;
    }
}
