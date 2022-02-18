using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MyPrefabs : MonoBehaviour
{
    public static MyPrefabs current;

    public ResourceId resource;
    public Construction contruction;
    public BuildingBlock block;
    public ProgressBar timer;
    public Projectile projectile; 
    public Chest chest; 
    public Tower tower;

    public InfoArrow arrow;
    public List<Sprite> arrowSprites;

    public SpriteRenderer justSpriteRender;
    public SpriteRenderer blinkingSpriteRender;
    public List<ObjectWithTutorial> tutorialPrefabs;
    public Dictionary<TutorialId, ObjectWithTutorial> cachedTutorialPrefabs = null;

    private void Awake()
    {
        current = this;
    }

    public SpriteRenderer GetShowCaseSprite(bool isBlinking) => isBlinking ? blinkingSpriteRender : justSpriteRender;

    public GameObject CreateObject(TutorialId id)
    {
        if (cachedTutorialPrefabs == null) cachedTutorialPrefabs = tutorialPrefabs.ToDictionary(t => t.id);

        if(cachedTutorialPrefabs.TryGetValue(id, out var prefab))
        {
            return Instantiate(prefab).gameObject;
        }


        return null;
    }

    public GameObject GetPrefab(TutorialId id)
    {
        if (cachedTutorialPrefabs == null) cachedTutorialPrefabs = tutorialPrefabs.ToDictionary(t => t.id);

        if (cachedTutorialPrefabs.TryGetValue(id, out var prefab))
        {
            return prefab.gameObject;
        }

        return null;
    }

    public InfoArrow CreateArrow(ArrowSprite sprite, Vector2 position, bool animated = true, float showDelay = 0f)
    {
        return CreateArrow(arrowSprites[(int)sprite], position, animated, showDelay);
    }

    public InfoArrow CreateArrow(Sprite sprite, Vector2 position, bool animated = true, float showDelay = 0f)
    {
        var instance = Instantiate(arrow, position, Quaternion.identity);

        instance.Init(sprite, animated, showDelay);

        return instance;
    }

    public ResourceId CreateResource(ResourceData data, int level = 0)
    {
        return CreateResource(data.type, data.color, level);
    }

    public ResourceId CreateResource(ResourceType type, OreColor color, int level = 0)
    {
        var instance = Instantiate(resource);

        instance.Init(type, color, level);

        return instance;
    }

    public enum ArrowSprite
    {
        Selector,
        Grid,
        TutorialUse,
        TutorialHammer,
        CanPlace,
    }
}
