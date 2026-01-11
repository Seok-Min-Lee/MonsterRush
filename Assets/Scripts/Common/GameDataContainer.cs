using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataContainer : Singleton<GameDataContainer>
{
    private readonly string logPath = Path.Combine(Application.persistentDataPath, "GameResultLog.json");

    public bool IsLoaded => gameResultLogs != null;

    private List<GameResultLog> gameResultLogs;

    public void LoadGameResultLogs()
    {
        List<GameResultLog> logs;

        if (File.Exists(logPath))
        {
            string json = File.ReadAllText(logPath);
            logs = JsonConvert.DeserializeObject<List<GameResultLog>>(json);
        }
        else
        {
            logs = new List<GameResultLog>();
            File.WriteAllText(logPath, JsonConvert.SerializeObject(logs, Formatting.Indented));
        }

        gameResultLogs = logs;
    }
    public bool TrySaveGameResultLogs()
    {
        if (IsLoaded)
        {
            File.WriteAllText(logPath, JsonConvert.SerializeObject(gameResultLogs, Formatting.Indented));
            return true;
        }

        return false;
    }
    public bool TryAddGameResultLog(GameResultLog newLog)
    {
        if (IsLoaded)
        {
            gameResultLogs.Add(newLog);

            return true;
        }

        return false;
    }
    public bool TryGetGameResultLogs(out List<GameResultLog> value)
    {
        if (IsLoaded)
        {
            value = gameResultLogs;
            return true;
        }

        value = null;
        return false;
    }
}
[System.Serializable]
public class GameResultLog
{
    public GameResultLog(int characterNum, int level, int killCount, int[] playerStats, int[] weaponLevels, int[] playerAbilities, int[] weaponAbilities, float playTime, string dateTime)
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
