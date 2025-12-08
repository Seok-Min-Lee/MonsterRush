using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class HistoryWindow : MonoBehaviour
{
    [SerializeField] private HistoryRow prefab;
    [SerializeField] private Transform parent;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;

    [SerializeField] private TextMeshProUGUI playCount;
    [SerializeField] private TextMeshProUGUI levelSum;
    [SerializeField] private TextMeshProUGUI killSum;
    [SerializeField] private TextMeshProUGUI playTimeSum;

    [SerializeField] private TextMeshProUGUI levelMax;
    [SerializeField] private TextMeshProUGUI killMax;
    [SerializeField] private TextMeshProUGUI playTimeMax;

    [SerializeField] private CharacterData[] characters;

    private Dictionary<int, CharacterData> characterDictionary = new Dictionary<int, CharacterData>();

    private List<HistoryRow> rows = new List<HistoryRow>();
    private HistoryRow maxLevelRow = null;
    private HistoryRow maxKillRow = null;
    private HistoryRow maxPlayTimeRow = null;

    private Sequence seq;
    private void Awake()
    {
        characterDictionary = new Dictionary<int, CharacterData>();
        for (int i = 0; i < characters.Length; i++)
        {
            characterDictionary.Add(characters[i].id, characters[i]);
        }
    }

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        List<GameResultLog> logs;
        int sumLevel = 0, sumKill = 0, maxLevel = 0, maxKill = 0;
        float sumPlayTime = 0f, maxPlayTime = 0f;

#if UNITY_EDITOR
        GameDataContainer.Instance.LoadGameResultLogs();
#endif

        if (GameDataContainer.Instance.TryGetGameResultLogs(value: out logs) && logs.Count > 0)
        {
            for (int i = 0; i < logs.Count; i++)
            {
                GameResultLog log = logs[i];
                CharacterData character = characterDictionary[log.characterNum];

                HistoryRow row = GameObject.Instantiate<HistoryRow>(prefab, parent);
                row.Init(
                    gameResultLog: log,
                    characterIcon: character.icon,
                    characterName: character.name
                );

                rows.Add(row);

                sumLevel += log.level;
                sumKill += log.killCount;
                sumPlayTime += log.playTime;

                if (log.level > maxLevel)
                {
                    maxLevel = log.level;
                    maxLevelRow = row;
                }
                if (log.killCount > maxKill)
                {
                    maxKill = log.killCount;
                    maxKillRow = row;
                }
                if (log.playTime > maxPlayTime)
                {
                    maxPlayTime = log.playTime;
                    maxPlayTimeRow = row;
                }
            }

            playCount.text = logs.Count.ToString("#,##0");
            levelSum.text = sumLevel.ToString("#,##0");
            killSum.text = sumKill.ToString("#,##0");
            playTimeSum.text = Utils.FormatTimeToHHmmss(sumPlayTime);

            levelMax.text = maxLevel.ToString("#,##0");
            killMax.text = maxKill.ToString("#,##0");
            playTimeMax.text = Utils.FormatTimeToHHmmss(maxPlayTime);
        }
        else
        {
            playCount.text = "-";
            levelSum.text = "-";
            killSum.text = "-";
            playTimeSum.text = "--:--:--";

            levelMax.text = "-";
            killMax.text = "-";
            playTimeMax.text = "--:--:--";
        }

        if (logs.Count < 3)
        {
            int emptyCount = 3 - logs.Count;
            for (int i = 0; i < emptyCount; i++)
            {
                HistoryRow row = GameObject.Instantiate<HistoryRow>(prefab, parent);
                row.Init(null, null, null);
                rows.Add(row);
            }
        }
    }
    public void OnClickMaxLevel()
    {
        int index = rows.IndexOf(maxLevelRow);
        ScrollToIndex(index);
    }
    public void OnClickMaxKill()
    {
        int index = rows.IndexOf(maxKillRow);
        ScrollToIndex(index);
    }
    public void OnClickMaxPlayTime()
    {
        int index = rows.IndexOf(maxPlayTimeRow);
        ScrollToIndex(index);
    }
    private void ScrollToIndex(int index)
    {
        float duration = 0.5f;

        seq?.Kill();
        seq = DOTween.Sequence();

        int count = content.childCount;
        if (count > 3)
        {
            index = Mathf.Clamp(index, 0, count - 1);

            float normalized = 1f - (float)index / (count - 1);

            seq.Append(DOTween.To(
                () => scrollRect.verticalNormalizedPosition, 
                x => scrollRect.verticalNormalizedPosition = x, 
                normalized, 
                duration
            ));
        }

        seq.AppendCallback(() => rows[index].Highlight());
    }
}

[System.Serializable]
public struct CharacterData
{
    public int id;
    public string name;
    public Sprite icon;
}
