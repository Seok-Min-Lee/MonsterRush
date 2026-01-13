using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryWindow : MonoBehaviour
{
    [SerializeField] private HistoryRow prefab;
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
        int sumLevel = 0, sumKill = 0, maxLevel = 0, maxKill = 0;
        float sumPlayTime = 0f, maxPlayTime = 0f;

#if UNITY_EDITOR
        GameHistoryService.Instance.Load();
#endif
        int count = GameHistoryService.Instance.HistoryRaws.Count;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                HistoryRaw raw = GameHistoryService.Instance.HistoryRaws[i];
                CharacterData character = characterDictionary[raw.characterNum];

                HistoryRow row = GameObject.Instantiate<HistoryRow>(prefab, content);
                row.Init(
                    raw: raw,
                    characterIcon: character.icon,
                    characterName: character.name
                );

                rows.Add(row);

                sumLevel += raw.level;
                sumKill += raw.killCount;
                sumPlayTime += raw.playTime;

                if (raw.level > maxLevel)
                {
                    maxLevel = raw.level;
                    maxLevelRow = row;
                }
                if (raw.killCount > maxKill)
                {
                    maxKill = raw.killCount;
                    maxKillRow = row;
                }
                if (raw.playTime > maxPlayTime)
                {
                    maxPlayTime = raw.playTime;
                    maxPlayTimeRow = row;
                }
            }

            playCount.text = count.ToString("#,##0");
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

        if (count < 4)
        {
            int emptyCount = 4 - count;
            for (int i = 0; i < emptyCount; i++)
            {
                HistoryRow row = GameObject.Instantiate<HistoryRow>(prefab, content);
                row.Init(default, null, null);
                rows.Add(row);
            }
        }
    }
    public void OnClickMaxLevel()
    {
        int index = rows.IndexOf(maxLevelRow);
        ScrollToIndex(index, Color.red);
    }
    public void OnClickMaxKill()
    {
        int index = rows.IndexOf(maxKillRow);
        ScrollToIndex(index, Color.green);
    }
    public void OnClickMaxPlayTime()
    {
        int index = rows.IndexOf(maxPlayTimeRow);
        ScrollToIndex(index, Color.blue);
    }
    private void ScrollToIndex(int index, Color color)
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

        seq.AppendCallback(() => rows[index].Highlight(color));
    }
}

[System.Serializable]
public struct CharacterData
{
    public int id;
    public string name;
    public Sprite icon;
}
