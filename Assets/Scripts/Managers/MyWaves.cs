using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyWaves : MonoBehaviour
{
    public static event System.Action<CombatWaveData> OnWaveChanged;
    public static event System.Action OnWaveCleared;

    SaveableGameState saveData = new SaveableGameState();
    public static bool loadFromSave = false;
    public static GameMode mode = GameMode.Tutorial;

    public bool allFinished = false;
    HashSet<string> waitingTutorial = new HashSet<string>();

    int unitsOnField = 0;
    int unitsInWave = 0;

    public List<Spawner> entrances;
    public List<Enemy> enemies;

    ListWithIndex<CombatWaveData> queue;
    public int WaveIdx => queue.idx;

    float nextAction;
    CombatWaveData currentData;

    public static MyWaves current;

    private void Awake()
    {
        current = this;
    }

    private void OnEnable()
    {
        MyTutorial.OnStepComplete += TutorialStepDone;
        Enemy.OnKilled += EnemyKilled;
    }

    private void OnDisable()
    {
        MyTutorial.OnStepComplete -= TutorialStepDone;
        Enemy.OnKilled -= EnemyKilled;
    }

    void EnemyKilled(Enemy enemy)
    {
        unitsOnField--;
        if(unitsOnField == 0 && currentData!= null && unitsInWave == 0)
        {
            OnWaveCleared?.Invoke();
            waitingTutorial.Remove("clear");
        }
    }

    void Start()
    {
        if(loadFromSave)
        {
            saveData.Load();
        }
        else
        {
            Init(0);
        }

        loadFromSave = false;
    }

    public void Init(int startingIdx)
    {
        queue = new ListWithIndex<CombatWaveData>(Database.current.asset.GetWavesForGameMode(mode))
        {
            idx = startingIdx
        };
        TakeNextWave();
    }

    public void Save()
    {
        saveData.Save();
    }

    void TakeNextWave()
    {
        if (queue.IsEmpty)
        {
            allFinished = true;
            GameEvents.current.Finish(true);
            return;
        }

        if (currentData != null && waitingTutorial.Count > 0) return;

        currentData = queue.Pop();
        unitsInWave = currentData.amount;

        if (!string.IsNullOrEmpty(currentData.tutorialKey) && !MyTutorial.current.IsComplete(currentData.tutorialKey))
        {
            waitingTutorial.Add(currentData.tutorialKey);
        }

        if (!string.IsNullOrEmpty(currentData.tutorialKeyRepeatable))
        {
            waitingTutorial.Add(currentData.tutorialKeyRepeatable);
        }

        foreach (var reward in currentData.rewards)
        {
            if (string.IsNullOrEmpty(reward)) continue;

            TutorialAction.Execute(reward);
        }

        nextAction = Time.time + currentData.delay;
        OnWaveChanged?.Invoke(currentData);
    }

    void NextAction()
    {
        unitsInWave--;

        if (0 < currentData.unitLevel)
        {
            Spawn(currentData.unitLevel-1, currentData.entrance);
        } //else just chill

        nextAction = Time.time + currentData.delay;

        if (unitsInWave == 0)
        {
            TakeNextWave();
        }
    }

    void Spawn(int level, int entrance = 0)
    {
        entrances[entrance].Spawn(enemies[level]);
        unitsOnField++;
    }

    void Update()
    {
        if (allFinished) return;

        if (currentData == null || unitsInWave <= 0)
        {
            TakeNextWave();
            return;
        }

        if(Time.time >= nextAction)
        {
            NextAction();
        }
    }


    void TutorialStepDone(string step)
    {
        if (waitingTutorial.Remove(step) && waitingTutorial.Count == 0)
        {
            nextAction = Time.time + currentData.delay;
        }
    }
}

[System.Serializable]
public class ListWithIndex<T> : List<T>
{
    public int idx = 0;

    public bool IsEmpty => idx >= Count;

    public T Pop()
    {
        if (IsEmpty) return default;

        var item = this[idx];
        idx++;

        return item;
    }

    public ListWithIndex(IEnumerable<T> other):base(other)
    {
    }
}


