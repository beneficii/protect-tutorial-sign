using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

public class WaveGenerator : MonoBehaviour
{
    public int maxDifficulty = 8;

    StringBuilder data;

    [EasyButtons.Button]
    void Generate()
    {
        Debug.Log("Generating..");

        data = new StringBuilder();

        for (int dif = 1; dif <= maxDifficulty; dif++)
        {
            CreateWave(2, 2, dif * 5, dif, WaveTemplate.Even, 16);

            if (dif < 2) continue;

            CreateWave(3, 3, dif * 1, dif, WaveTemplate.Even, 16);

            if (dif < 3) continue;

            CreateWave(2, 4, dif * 2, dif, WaveTemplate.Boss, 16);

            if (dif < 4) continue;
            CreateWave(2, 4, dif * 2, dif, WaveTemplate.Even, 16);

            if (dif < 5) continue;

            CreateWave(2, 2, dif * 4, dif, WaveTemplate.Even, 16);
            CreateWave(2, 5, 2, dif-4, WaveTemplate.Boss, 16);

            if (dif < 6) continue;

            CreateWave(2, 5, dif, dif, WaveTemplate.Even, 16);

            if (dif < 7) continue;

            CreateWave(6, 6, dif - 6, dif, WaveTemplate.Even, 16);

            AppendPause(10);
        }

        Debug.Log(data.ToString());
        GUIUtility.systemCopyBuffer = data.ToString();
    }

    [EasyButtons.Button]
    void GenerateHard()
    {
        Debug.Log("Generating..");

        data = new StringBuilder();

        for (int dif = 1; dif <= maxDifficulty; dif++)
        {
            var realDif = dif * 3;
            AppendPause(10, $"Day {dif}!");
            CreateWave(2, 2, 2, realDif, WaveTemplate.Even, 16);

            CreateWave(3, 3, dif * 2, realDif, WaveTemplate.Even, 16);

            CreateWave(2, 5, dif * 3, realDif, WaveTemplate.Boss, 16);

            if (dif < 3) continue;

            CreateWave(4, 4, dif * 3, realDif, WaveTemplate.Even, 16);

            CreateWave(5, 5, dif, realDif, WaveTemplate.Even, 16);

            if (dif < 5) continue;

            CreateWave(6, 6, dif - 4, realDif, WaveTemplate.Even, 16);

            if (dif < 8) continue;

            CreateWave(6, 6, dif, realDif, WaveTemplate.Even, 16);
        }

        Debug.Log(data.ToString());
        GUIUtility.systemCopyBuffer = data.ToString();
    }

    void CreateWave(int minLevel, int maxLevel, int totalAmount, int difficulty, WaveTemplate template, int pause)
    {
        float rate = Mathf.Pow((minLevel + maxLevel) / 2f, 2f) / difficulty;

        int totalSteps = maxLevel - minLevel + 1;
        switch (template)
        {
            case WaveTemplate.Even:
                int perLevel = totalAmount / totalSteps;
                for (int i = minLevel; i <= maxLevel; i++)
                {
                    AppendLevel(i, perLevel, rate);
                }
            break;
            case WaveTemplate.Boss:
                AppendLevel(minLevel, totalAmount - 1, rate);
                AppendLevel(maxLevel, 1, rate);
                break;
            default:
                break;
        }

        AppendPause(pause);
    }

    void AppendLevel(int level, int perLevel, float rate)
    {
        data.AppendLine($"{level}\t{perLevel}\t{rate.ToString("0.##", CultureInfo.InvariantCulture)}\t{0}");
    }

    void AppendPause(int time, string extra = null)
    {
        string line = $"{0}\t{1}\t{time}\t{0}";
        if(!string.IsNullOrEmpty(extra))
        {
            line = line + $"\t{extra}";
        }
        data.AppendLine(line);
    }

    public enum WaveTemplate
    {
        Even,
        Boss,
    }
}
