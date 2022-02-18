using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "LTRO-1/Data/Database")]
public class DatabaseAsset : ScriptableObject
{
    public List<TextAsset> files;

    public List<TowerCombatData> towers;
    public List<CombatData> enemies;
    public List<ListOfWaves> waves;
    public List<CombatRecipeData> recipes;

    public List<CombatWaveData> GetWavesForGameMode(GameMode mode)
    {
        return waves[(int)mode];
    }

    public List<T> ReadData<T>(string tabName) where T : new()
    {
        // if it fails it fails
        var file = files.First(x=>x.name==tabName);

        return FancyCSV.FromText<T>(file.text);
    }

    [EasyButtons.Button]
    public void LoadTowersData()
    {
        towers = ReadData<CombatData>("Towers")
            .GroupBy(
                x => x.color,
                (key, val) => new TowerCombatData(key, val)
            ).ToList();
    }

    [EasyButtons.Button]
    public void LoadEnemiesData()
    {
        enemies = ReadData<CombatData>("Enemies");
    }

    [EasyButtons.Button]
    public void LoadWavesData()
    {
        waves = new List<ListOfWaves>();
        foreach (var item in EnumUtil.GetValues<GameMode>())
        {
            waves.Add(ReadData<CombatWaveData>($"Waves_{item}"));
        }
    }

    [EasyButtons.Button]
    public void LoadRecipeData()
    {
        recipes = ReadData<CombatRecipeData>("Recipes");
    }

    [EasyButtons.Button]
    public void LoadAll()
    {
        LoadTowersData();
        LoadEnemiesData();
        LoadWavesData();
        LoadRecipeData();
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}


[System.Serializable]
public class CombatData
{
    public OreColor color;
    public string name;
    public int level;
    public int hp;
    public int damage;
    public int range;
    public float attackRate;
    public float moveSpeed;
    public float splash;
    public int targets;
    public string description;
}

[System.Serializable]
public class CombatWaveData
{
    //unitLevel	amount	delay	message	entrance
    public int unitLevel;
    public int amount;
    public float delay;
    public string message;
    public int entrance;
    public string tutorialKey;
    public string tutorialKeyRepeatable;
    public List<string> rewards;

    public CombatWaveData() { }

    public CombatWaveData MakeCopy()
    {
        return (CombatWaveData)MemberwiseClone();
    }
}

[System.Serializable]
public class CombatRecipeData
{
    //result description items
    public TutorialId result;
    public string description;
    public List<ResourceData> items = new List<ResourceData>();

    public CombatRecipeData() { }

    public bool Match(List<BuildingBlock> blocks, out OreColor color)
    {
        color = OreColor.Any;

        if (items.Count != blocks.Count) return false;

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var block = blocks[i];

            if (item.color == OreColor.Any && item.type == block.resource.type)
            {
                if (color == OreColor.Any)
                {
                    color = block.resource.color;
                }
                else if (color != block.resource.color)
                {
                    return false;
                }
            }
            else if (!item.Equals(block.resource))
            {
                return false;
            }
        }

        return true;
    }
}

[System.Serializable]
public class TowerCombatData
{
    public OreColor color;
    public List<CombatData> levels = new List<CombatData>();

    public TowerCombatData(OreColor color)
    {
        this.color = color;
        levels = new List<CombatData>();
    }

    public TowerCombatData(OreColor color, IEnumerable<CombatData> levels)
    {
        this.color = color;
        this.levels = levels.OrderBy(x=>x.level).ToList();
    }
}

public enum GameMode
{
    Tutorial,
    Normal,
    Hardcore
}

[System.Serializable]
public class ListOfWaves
{
    public List<CombatWaveData> innerList;

    public CombatWaveData this[int key]
    {
        get => innerList[key];
        set => innerList[key] = value;
    }

    public ListOfWaves()
    {
        innerList = new List<CombatWaveData>();
    }

    public ListOfWaves(List<CombatWaveData> list)
    {
        innerList = list;
    }

    public static implicit operator List<CombatWaveData>(ListOfWaves list) => list.innerList;
    public static implicit operator ListOfWaves(List<CombatWaveData> list) => new ListOfWaves(list);
}