using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MyEffects : MonoBehaviour
{
    public static MyEffects current;

    public DictEffectSprites sprites;
    public OneShotAnimation prefab;

    private void Awake()
    {
        current = this;
    }

    public void AddEffect(AnimEffect type, Vector2 position, int loops = 1 )
    {
        var list = sprites.GetSprites(type);

        if (list == null) return;


        var instance = Instantiate(prefab, position, Quaternion.identity);
        instance.Init(list, loops);
    }
}

public enum AnimEffect
{
    None,
    Upgrade,
    Error,
    Warning
}

[System.Serializable]
public class DictEffectSprites
{
    public List<Item> items;
    Dictionary<AnimEffect, List<Sprite>> dict = null;

    [System.Serializable]
    public class Item
    {
        public AnimEffect id;
        public List<Sprite> sprites;
    }

    void Init()
    {
        dict = items.ToDictionary(x => x.id, x => x.sprites);
    }

    public List<Sprite> GetSprites(AnimEffect id)
    {
        if (dict == null) Init();

        if(dict.TryGetValue(id, out var value))
        {
            return value;
        }
        else
        {
            Debug.LogError($"Sprites not found for {id}");
            return null;
        }
    }

}
