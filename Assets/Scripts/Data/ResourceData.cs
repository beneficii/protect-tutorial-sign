using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct ResourceData
{
    public ResourceType type;
    public OreColor color;
    public int level; 

    public ResourceData(ResourceType type, OreColor color)
    {
        this.color = color;
        this.type = type;
        level = 0;
    }

    public ResourceData(ResourceType type, OreColor color, int level)
    {
        this.color = color;
        this.type = type;
        this.level = level;
    }

    public static bool TryParse(string color, string type, out ResourceData data)
    {
        if (System.Enum.TryParse(type, true, out ResourceType tValue)
            && System.Enum.TryParse(color, true, out OreColor tColor))
        {
            data = new ResourceData(tValue, tColor);
            return true;
        }

        data = default;
        return false;
    }

    public override string ToString() => $"[{type} {color} ({level})]";

    [CreateFromString]
    public static ResourceData FromString(string s)
    {
        var split = s.Trim(' ').Split(' ');
        TryParse(split[0], split[1], out var data);
        

        return data;
    }

    public string ToLine() => $"{color} {type}";

    public static List<ResourceData> StringToList(string s)
    {
        var result = new List<ResourceData>();
        if (string.IsNullOrEmpty(s)) return result;
        var args = s.Split(' ');

        for (int i = 0; i < args.Length; i += 2)
        {
            if (ResourceData.TryParse(args[i], args[i + 1], out var data))
            {
                result.Add(data);
            }
        }

        return result;
    }

    public static string ListToString(IEnumerable<ResourceData> list) => string.Join(" ", list.Select(x => x.ToLine()));
}

public enum OreColor
{
    Grey,
    Red,
    Blue,
    None,
    White,
    Teal,
    Orange,
    Brown,
    Black,
    Any
}

public enum ResourceType
{
    Ore,
    Bar,
    Gem,
    Potion,
    Upgrade_Tower,
    Upgrade_Furnace,
    Upgrade_Pyramid,
    Upgrade_Pot,
    None
}


