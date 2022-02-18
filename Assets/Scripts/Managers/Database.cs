using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database current;

    public SDictResourceSprite spritesForResource;
    public SDictOreColor colorsForOre;
    public List<Sprite> spritesForRange;
    public SDictResourceToBuilding upgrades;

    public DatabaseAsset asset;

    public CombatData GetEnemyData(string name)
    {
        foreach (var item in asset.enemies)
        {
            if (name == item.name) return item;
        }

        return null;
    }

    public CombatData GetEnemyData(int level)
    {
        foreach (var item in asset.enemies)
        {
            if (level == item.level) return item;
        }

        return null;
    }

    public CombatData GetTowerData(OreColor color, int level)
    {
        foreach (var item in asset.towers)
        {
            if (item.color == color)
            {
                if (level < item.levels.Count)
                {
                    return item.levels[level];
                }
                else
                {
                    return null;
                }
            }
        }

        return null;
    }

    private void Awake()
    {
        current = this;
    }

    public Sprite GetRangeSprite(int range)
    {
        if (range == 0) return null;

        return spritesForRange[range - 1];
    }

    public static void SetColor(SpriteRenderer render, OreColor color, int idx)
    {
        render.material.SetColor($"Color_{idx}", current.colorsForOre.Get(color));
    }

    public static void SetColor(SpriteRenderer render, params OreColor[] colors)
    {
        for (int i = 0; i < colors.Length; i++)
        {
            render.material.SetColor($"Color_{i+1}", current.colorsForOre.Get(colors[i]));
        }
    }

    public static void SetOutlineColor(SpriteRenderer render, OreColor color)
    {
        render.material.SetColor($"Color_outline", current.colorsForOre.Get(color));
    }

    public static void ClearColors(SpriteRenderer render)
    {
        for (int i = 0; i < 3; i++)
        {
            render.material.SetColor($"Color_{i + 1}", current.colorsForOre.Get(OreColor.None));
        }
        SetOutlineColor(render, OreColor.Black);
    }

    static OreColor GetCombination(OreColor a, OreColor b)
    {
        if (a == OreColor.Grey && b == OreColor.Blue) return OreColor.Teal;
        if (a == OreColor.Grey && b == OreColor.Red) return OreColor.Orange;
        if (a == OreColor.Red && b == OreColor.Blue) return OreColor.Brown;

        return OreColor.None;
    }

    public static bool TryCombine(List<OreColor> ab, out OreColor result)
    {
        if(ab.Count != 2)
        {
            result = OreColor.None;
            return false;
        }

        result = GetCombination(ab[0], ab[1]);
        if (result != OreColor.None) return true;

        result = GetCombination(ab[1], ab[0]);
        if (result != OreColor.None) return true;

        return false;
    }
}

public static class Constants
{
    public const float pixelSize = 0.0625f;
    public const float frameDelay = 0.15f;
}

public static class HelpersDatabase
{
    public static TValue Get<TKey,TValue>(this SerializableDictionary<TKey, TValue> dict, TKey key)
    {
        if(dict.TryGetValue(key, out var value))
        {
            return value;
        }

        return default;
    }
}



[System.Serializable]
public class SDictResourceSprite : SerializableDictionary<ResourceType, Sprite> { }

[System.Serializable]
public class SDictOreColor : SerializableDictionary<OreColor, Color> { }

[System.Serializable]
public class SDictResourceToBuilding : SerializableDictionary<ResourceType, TutorialId> { }


