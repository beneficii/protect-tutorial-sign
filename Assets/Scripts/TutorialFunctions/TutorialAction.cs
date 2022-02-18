using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static class TutorialAction
{
    static Dictionary<string, TActionBase> dict = null;

    static void Init()
    {
        dict = new Dictionary<string, TActionBase>();
        var type = typeof(TActionBase);

        foreach(var tAction in Assembly.GetAssembly(type)
            .GetTypes()
            .Where(t => type.IsAssignableFrom(t) && t.IsAbstract == false))
        {
            TActionBase action = System.Activator.CreateInstance(tAction) as TActionBase;
            dict.Add(action.ActionId, action);
        }

    }

    public static void Execute(string line)
    {
        if (dict == null) Init();

        var lines = line.Split(' ');

        if (dict.TryGetValue(lines[0], out var action))
        {
            action.Exectue(string.Join(" ",lines.Skip(1)));
        }
        else
        {
            Debug.Log($"Warning: Action {lines[0]} not found!");
        }
    }
}

public abstract class TActionBase
{
    public abstract string ActionId { get; }

    public abstract void Exectue(string line);
}

public class TActionChest : TActionBase
{
    public override string ActionId => "chest";

    public override void Exectue(string line)
    {
        var resources = ResourceData.StringToList(line);

        Chest.Spawn(resources);
    }
}

public class TActionSave : TActionBase
{
    public override string ActionId => "save";

    public override void Exectue(string line)
    {
        MyWaves.current.Save();
    }
}

public class TActionDamage : TActionBase
{
    public override string ActionId => "damage";

    public override void Exectue(string line)
    {
        var args = line.Split(' ');
        if (!System.Enum.TryParse(args[0], true, out TutorialId id)) return;
        if (!int.TryParse(args[1], out int value)) return;

        var target = Object.FindObjectsOfType<ObjectWithHealth>()
            .FirstOrDefault(x => x.TryGetComponent<ObjectWithTutorial>(out var tObj) && tObj.id == id);

        if (target) target.Damage(value);
    }
}

public class TActionBuild : TActionBase
{
    public override string ActionId => "build";

    public override void Exectue(string line)
    {
        var args = line.Split(' ');
        int idx = 0;
        OreColor oreColor = OreColor.None;
        if (System.Enum.TryParse(args[idx], true, out OreColor color))
        {
            oreColor = color;
            idx++;
        }

        if (System.Enum.TryParse(args[idx], true, out TutorialId id))
        {
            var pos = MySpots.current.RandomFreeSpot();
            MySpots.current.taken.Add(pos);
            var prefab = MyPrefabs.current.GetPrefab(id);
            var instance = Object.Instantiate(prefab, new Vector3(pos.x, pos.y), Quaternion.identity);
            
            if (oreColor != OreColor.None && instance.TryGetComponent<ColorableComponent>(out var colorable))
            {
                colorable.MainColor = oreColor;
            }

            idx++;
        }
    }
}

public class TActionRecipe : TActionBase
{
    public override string ActionId => "recipe";

    public override void Exectue(string line)
    {
        if (line == "all")
        {
            RecipePanel.current.AddAll();
        }

        if(System.Enum.TryParse(line, true, out TutorialId id))
        {
            foreach (var item in Database.current.asset.recipes)
            {
                if(item.result == id)
                {
                    RecipePanel.current.Add(item);
                    
                    return;
                }
            }
        }
    }
}

public class TActionManualUncover : TActionBase
{
    public override string ActionId => "uncover";

    public override void Exectue(string line)
    {
        if (int.TryParse(line, out int idx))
        {
            RecipePanel.current.Uncover(idx);
        }
    }
}


public class TActionActivate : TActionBase
{
    public override string ActionId => "activate";

    public override void Exectue(string line)
    {
        if (System.Enum.TryParse(line, true, out TutorialId id))
        {
            var obj = GameObject.FindObjectsOfType<ObjectWithTutorial>().FirstOrDefault(x => x.id == id);

            if(obj && obj.TryGetComponent<IDormant>(out var dormant))
            {
                dormant.Active = true;
            }
        }
    }
}


public class TActionDeActivate : TActionBase
{
    public override string ActionId => "deactivate";

    public override void Exectue(string line)
    {
        if (System.Enum.TryParse(line, true, out TutorialId id))
        {
            var obj = GameObject.FindObjectsOfType<ObjectWithTutorial>().FirstOrDefault(x => x.id == id);

            if (obj && obj.TryGetComponent<IDormant>(out var dormant))
            {
                dormant.Active = false;
            }
        }
    }
}
