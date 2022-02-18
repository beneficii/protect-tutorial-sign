using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SaveableGameState
{
    public const string prefsKey = "save_0.1b";
    public int waveIdx = 0;
    public int wallHp = 0;
    public List<string> unlockedRecipes;
    public List<ObjectInfo> savedObjects;
    public string currentResource;
    public GameMode mode;
   
    public void Save()
    {
        var listOfSavedObjects = new List<ObjectInfo>();
        wallHp = 0;

        foreach (var tutorialItem in Object.FindObjectsOfType<ObjectWithTutorial>())
        {
            if (tutorialItem.id == TutorialId.Wall) wallHp = tutorialItem.GetComponent<ObjectWithHealth>().Hp;

            if (!tutorialItem.TryGetComponent<ISaveable>(out var itemSaveable)) continue;

            var color = tutorialItem.GetComponent<ColorableComponent>()?.MainColor ?? OreColor.None;

            var objInfo = new ObjectInfo(
                    tutorialItem.id,
                    tutorialItem.transform.position,
                    color,
                    itemSaveable.Save());

            listOfSavedObjects.Add(objInfo);
        }

        currentResource = null;
        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        if(player)
        {
            var res = ResourceCarrier.Peek(player);
            if(res)
            {
                currentResource = res.data.ToLine();
            }
        }

        waveIdx = MyWaves.current.WaveIdx;
        unlockedRecipes = RecipePanel.current.unlocked.Select(x=>x.ToString()).ToList();
        savedObjects = listOfSavedObjects;
        mode = MyWaves.mode;

        PlayerPrefs.SetString(prefsKey, JsonUtility.ToJson(this));
    }

    public void Load()
    {
        var json = PlayerPrefs.GetString(prefsKey);
        JsonUtility.FromJsonOverwrite(json, this);

        foreach (var item in savedObjects)
        {
            if(!System.Enum.TryParse(item.objectId, false, out TutorialId id))
            {
                Debug.Log($"Can't find: {id}");
                continue;
            }

            var instance = MyPrefabs.current.CreateObject(id);
            instance.transform.position = new Vector3(item.x, item.y);
            if (instance.TryGetComponent<ColorableComponent>(out var cComponent))
            {
                cComponent.MainColor = item.color;
            }

            instance.GetComponent<ISaveable>().Load(item.line);
        }

        var recipeDict = Database.current.asset.recipes.ToDictionary(x => x.result.ToString());

        foreach (var item in unlockedRecipes)
        {
            if(recipeDict.TryGetValue(item, out var recipe))
            {
                RecipePanel.current.Add(recipe);
            }
        }

        var wall = Object.FindObjectsOfType<ObjectWithHealth>()
            .FirstOrDefault(x => x.TryGetComponent<ObjectWithTutorial>(out var tObj) && tObj.id == TutorialId.Wall);

        if (wall) wall.Hp = wallHp;

        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        if (player && !string.IsNullOrEmpty(currentResource))
        {
            ResourceCarrier.Assign(player, ResourceData.FromString(currentResource));
        }

        MyWaves.mode = mode;

        MyWaves.current.Init(waveIdx);
    }

    [System.Serializable]
    public class ObjectInfo
    {
        public string objectId;
        public float x;
        public float y;
        public OreColor color;

        public string line;

        public ObjectInfo(TutorialId objectId, Vector2 pos, OreColor color, string line)
        {
            this.objectId = objectId.ToString();
            this.x = pos.x;
            this.y = pos.y;
            this.line = line;
            this.color = color;
        }
    }

#if UNITY_EDITOR

    [UnityEditor.MenuItem("MyMenu/Clear prefs")]
    static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [UnityEditor.MenuItem("MyMenu/Show prefs")]
    static void ShowSave()
    {
        Debug.Log(PlayerPrefs.GetString(prefsKey));
    }
#endif
}

public interface ISaveable
{
    GameObject gameObject { get; }

    string Save();
    void Load(string line);
}
