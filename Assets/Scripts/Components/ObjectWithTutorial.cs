using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectWithTutorial : MonoBehaviour
{
    public TutorialId id = TutorialId.None;

    public static event System.Action<GameObject> OnKilled;
    public static event System.Action<GameObject> OnAdded;

    public void Killed()
    {
        OnKilled?.Invoke(gameObject);
    }

    private void Start()
    {
        OnAdded?.Invoke(gameObject);
    }

    public static ObjectWithTutorial FindOne(TutorialId id, OreColor color = OreColor.Any)
    {
        foreach (var item in FindObjectsOfType<ObjectWithTutorial>())
        {
            if (item.id != id) continue;
            if (color != OreColor.Any
                && (
                !item.TryGetComponent<ColorableComponent>(out var colorable)
                || colorable.MainColor != color)) continue;

            return item;
        }


        return null;
    }
}
