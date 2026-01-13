using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameHistoryService : Singleton<GameHistoryService>
{
    private readonly string logPath = Path.Combine(Application.persistentDataPath, "GameResultLog.json");

    public bool IsLoaded { get; private set; } = false;

    public IReadOnlyList<HistoryRaw> HistoryRaws => historyRaws;
    private List<HistoryRaw> historyRaws;

    public void Load()
    {
        List<HistoryRaw> raws;

        if (File.Exists(logPath))
        {
            string json = File.ReadAllText(logPath);
            raws = JsonConvert.DeserializeObject<List<HistoryRaw>>(json);
        }
        else
        {
            raws = new List<HistoryRaw>();
            File.WriteAllText(logPath, JsonConvert.SerializeObject(raws, Formatting.Indented));
        }

        historyRaws = raws;

        IsLoaded = true;
    }
    public bool TrySave()
    {
        if (IsLoaded)
        {
            File.WriteAllText(logPath, JsonConvert.SerializeObject(historyRaws, Formatting.Indented));

            return true;
        }

        return false;
    }
    public bool TryAdd(HistoryRaw newLog)
    {
        if (IsLoaded)
        {
            historyRaws.Add(newLog);

            return true;
        }

        return false;
    }
}
[System.Serializable]
public struct HistoryRaw
{
    public HistoryRaw(int characterNum, int level, int killCount, int[] playerStats, int[] weaponLevels, int[] playerAbilities, int[] weaponAbilities, float playTime, string dateTime)
    {
        this.characterNum = characterNum;
        this.level = level;
        this.killCount = killCount;
        this.playerStats = playerStats;
        this.weaponLevels = weaponLevels;
        this.playerAbilities = playerAbilities;
        this.weaponAbilities = weaponAbilities;
        this.playTime = playTime;
        this.dateTime = dateTime;
    }

    public int characterNum;
    public int level;
    public int killCount;
    public int[] playerStats;
    public int[] weaponLevels;
    public int[] playerAbilities;
    public int[] weaponAbilities;
    public float playTime;
    public string dateTime;
}
