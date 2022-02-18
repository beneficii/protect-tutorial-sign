using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using UnityEngine;

public abstract class MenuAction
{
    public abstract string Name { get; }

    protected abstract void SetFromLine(List<string> line);
    public abstract void Execute();
    public virtual bool IsActive => true;

    // static stuff starts here

    static Dictionary<string, Type> dict;

    static void Init()
    {
        dict = new Dictionary<string, Type>();
        var type = typeof(MenuAction);

        foreach (var tAction in Assembly.GetAssembly(type)
            .GetTypes()
            .Where(t => type.IsAssignableFrom(t) && t.IsAbstract == false))
        {
            var action = Activator.CreateInstance(tAction) as MenuAction;
            dict.Add(action.Name, tAction);
        }
    }

    public static MenuAction Create(string line)
    {
        if (dict == null) Init();

        var lines = line.Split(' ');

        if (dict.TryGetValue(lines[0], out var actionType))
        {
            var action = Activator.CreateInstance(actionType) as MenuAction;
            action.SetFromLine(lines.Skip(1).ToList());

            return action;
        }
        else
        {
            return null;
        }
    }
}

public class MenuActionScene : MenuAction
{
    public override string Name => "Scene";

    public string sceneId;

    public override void Execute()
    {
        SceneManager.LoadScene(sceneId);
    }

    protected override void SetFromLine(List<string> line)
    {
        sceneId = line[0];
    }
}

public class MenuActionNothing : MenuAction
{
    public override string Name => "Nothing";

    public override void Execute()
    {
    }

    protected override void SetFromLine(List<string> line)
    {
    }
}

public class MenuActionCloseEnd : MenuAction
{
    public override string Name => "CloseEnd";

    public override void Execute()
    {
        GameEvents.current.endScreen.gameObject.SetActive(false);
    }

    protected override void SetFromLine(List<string> line)
    {
    }
}

public class MenuActionRetry : MenuAction
{
    public override string Name => "Retry";

    public override void Execute()
    {
        bool load = false;
        if(PlayerPrefs.HasKey(SaveableGameState.prefsKey))
        {
            var state = new SaveableGameState();
            state.Load();
            if(state.mode == MyWaves.mode)
            {
                load = true;
            }
        }

        MyWaves.loadFromSave = load;
        SceneManager.LoadScene("Game");
    }

    protected override void SetFromLine(List<string> line)
    {
    }
}

public class MenuActionLoad : MenuAction
{
    public override string Name => "Load";

    public override bool IsActive => PlayerPrefs.HasKey(SaveableGameState.prefsKey);

    public override void Execute()
    {
        MyWaves.loadFromSave = true;
        SceneManager.LoadScene("Game");
    }

    protected override void SetFromLine(List<string> line)
    {
    }
}

public class MenuActionRestart : MenuAction
{
    public override string Name => "Restart";

    public override bool IsActive => true;

    public override void Execute()
    {
        MyWaves.loadFromSave = false;
        SceneManager.LoadScene("Game");
    }

    protected override void SetFromLine(List<string> line)
    {
    }
}

public class MenuActionNewGame : MenuAction
{
    public override string Name => "Play";

    public override bool IsActive => true;

    public GameMode mode;

    public override void Execute()
    {
        MyWaves.loadFromSave = false;
        MyWaves.mode = mode;
        SceneManager.LoadScene("Game");
    }

    protected override void SetFromLine(List<string> line)
    {
        Enum.TryParse(line[0], true, out mode);
    }
}

public class MenuActionLog : MenuAction
{
    public override string Name => "Log";

    public string msg;

    public override void Execute()
    {
        UnityEngine.Debug.Log(msg);
    }

    protected override void SetFromLine(List<string> line)
    {
        msg = string.Join(" ", line);
    }
}

public class MenuActionTestLog : MenuAction
{
    public override string Name => "Inactivce";

    public string msg;

    public override void Execute()
    {
        UnityEngine.Debug.Log(msg);
    }

    protected override void SetFromLine(List<string> line)
    {
        msg = string.Join(" ", line);
    }

    public override bool IsActive => false;
}