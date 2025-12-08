using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataContainer : Singleton<GameDataContainer>
{
    private readonly string logPath = Path.Combine(Application.persistentDataPath, "GameResultLog.json");

    List<GameResultLog> gameResultLogs;

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
        if (gameResultLogs != null)
        {
            File.WriteAllText(logPath, JsonConvert.SerializeObject(gameResultLogs, Formatting.Indented));
            return true;
        }

        return false;
    }
    public bool TryAddGameResultLog(GameResultLog newLog)
    {
        if (gameResultLogs != null)
        {
            gameResultLogs.Add(newLog);

            return true;
        }

        return false;
    }
    public bool TryGetGameResultLogs(out List<GameResultLog> value)
    {
        if (gameResultLogs != null)
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
    public GameResultLog(int characterNum, int level, int killCount, int[] playerStats, int[] weaponLevels, int[] abilityStack, float playTime, string dateTime)
    {
        this.characterNum = characterNum;
        this.level = level;
        this.killCount = killCount;
        this.playerStats = playerStats;
        this.weaponLevels = weaponLevels;
        this.abilityStack = abilityStack;
        this.playTime = playTime;
        this.dateTime = dateTime;
    }

    public int characterNum;
    public int level;
    public int killCount;
    public int[] playerStats;
    public int[] weaponLevels;
    public int[] abilityStack;
    public float playTime;
    public string dateTime;
}
