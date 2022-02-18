using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public abstract class BuffBase
{
    static Dictionary<OreColor, BuffBase> dict = null;

    static void Init()
    {
        dict = new Dictionary<OreColor, BuffBase>();
        var type = typeof(BuffBase);

        foreach (var buffType in Assembly.GetAssembly(type)
            .GetTypes()
            .Where(t => type.IsAssignableFrom(t) && t.IsAbstract == false))
        {
            BuffBase buff = System.Activator.CreateInstance(buffType) as BuffBase;
            dict.Add(buff.Id, buff);
        }
    }

    public static BuffBase Get(OreColor color)
    {
        if (dict == null) Init();
        if (dict.TryGetValue(color, out var buff))
        {
            return buff;
        }
        else
        {
            Debug.Log($"Warning: Buff {color} not implemented!");
            return Get(OreColor.None); //default buff;
        }
    }
    
    public abstract OreColor Id { get; }
    public abstract string Description { get; }
    public virtual void Apply(PlayerCtrl player) { }
    public virtual void Remove(PlayerCtrl player) { }
    public virtual float HammerSpeed(TutorialId target) => 1f;
    public virtual ResourceData TransformResource(ResourceData input) => input;
}

public class BuffNone : BuffBase
{
    public override OreColor Id => OreColor.None;

    public override string Description => "";

    public override void Apply(PlayerCtrl player)
    {
        //nothing
    }

    public override void Remove(PlayerCtrl player)
    {
        //nothing
    }
}


public class BuffGrey : BuffBase
{
    public override OreColor Id => OreColor.Grey;

    public override string Description => "Increase ore mining speed";

    public override float HammerSpeed(TutorialId target)
    {
        if (target == TutorialId.Mine) return 2f;

        return base.HammerSpeed(target);
    }
}

public class BuffRed : BuffBase
{
    const float bonus = 2f;

    public override OreColor Id => OreColor.Red;
    public override string Description => "Increase movement speed";

    public override void Apply(PlayerCtrl player)
    {
        player.speed += bonus;
    }

    public override void Remove(PlayerCtrl player)
    {
        player.speed -= bonus;
    }
}

public class BuffBlue : BuffBase
{
    public override OreColor Id => OreColor.Blue;
    public override string Description => "Increase wall repair speed";


    public override float HammerSpeed(TutorialId target)
    {
        if (target == TutorialId.Wall) return 2f;

        return base.HammerSpeed(target);
    }
}

public class BuffOrange : BuffBase
{
    public override OreColor Id => OreColor.Orange;
    public override string Description => "Recolors items you touch";


    public override ResourceData TransformResource(ResourceData input)
    {
        input.color = OreColor.Orange;
        return input;
    }
}

public class BuffTeal : BuffBase
{
    public override OreColor Id => OreColor.Teal;
    public override string Description => "Transforms ore you touch";


    public override ResourceData TransformResource(ResourceData input)
    {
        if(input.type == ResourceType.Ore)
        {
            input.type = ResourceType.Bar;
        }
        return input;
    }
}

public class BuffBrown : BuffBase
{
    public override OreColor Id => OreColor.Brown;
    public override string Description => "Transforms ore you touch";

    public override ResourceData TransformResource(ResourceData input)
    {
        if (input.type == ResourceType.Ore)
        {
            input.type = ResourceType.Gem;
        }
        return input;
    }
}

