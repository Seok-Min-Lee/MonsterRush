using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryRow : MonoBehaviour
{
    [SerializeField] private GameObject onData;
    [SerializeField] private GameObject offData;
    [SerializeField] private Image highlightMask;

    [SerializeField] private Image characterIcon;
    [SerializeField] private TextMeshProUGUI character;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI killCount;
    [SerializeField] private TextMeshProUGUI playTime;
    [SerializeField] private TextMeshProUGUI dateTime;

    [SerializeField] private TextMeshProUGUI[] playerStats;
    [SerializeField] private TextMeshProUGUI[] weaponLevels;

    [SerializeField] private Image resultSticker;
    [SerializeField] private TextMeshProUGUI resultText;

    private GameResultLog gameResultLog;

    private Sequence seq;
    public void Init(GameResultLog gameResultLog, Sprite characterIcon, string characterName)
    {
        if (gameResultLog != null)
        {
            this.gameResultLog = gameResultLog;

            this.characterIcon.sprite = characterIcon;
            character.text = characterName;
            level.text = gameResultLog.level.ToString();
            killCount.text = gameResultLog.killCount.ToString("#,##0");
            playTime.text = Utils.FormatTimeTommss(gameResultLog.playTime);
            dateTime.text = gameResultLog.dateTime;

            for (int i = 0; i < gameResultLog.playerStats.Length; i++)
            {
                playerStats[i].text = gameResultLog.playerStats[i].ToString();
            }

            for (int i = 0; i < gameResultLog.weaponLevels.Length; i++)
            {
                weaponLevels[i].text = gameResultLog.weaponLevels[i] == 16 ? "MAX" : gameResultLog.weaponLevels[i].ToString();
            }

            for (int i = 0; i < gameResultLog.weaponAbilities.Length; i++)
            {
                weaponLevels[gameResultLog.weaponAbilities[i]].color = new Color(0.5f, 0f, 1f, 1f);
            }

            if (gameResultLog.level < 80)
            {
                resultSticker.color = Color.black;
                resultText.color = Color.black;
                resultText.text = "ÆÐ¹è";
            }
            else
            {
                resultSticker.color = Color.yellow;
                resultText.color = Color.yellow;
                resultText.text = "½Â¸®!";
            }

            onData.SetActive(true);
            offData.SetActive(false);
        }
        else
        {
            onData.SetActive(false);
            offData.SetActive(true);
        }
    }
    public void Highlight()
    {
        seq?.Kill();
        seq = DOTween.Sequence();

        seq.AppendCallback(() => highlightMask.color = new Color(0f, 0.5f, 1f, 0f));
        seq.Append(highlightMask.DOColor(new Color(0f, 0.5f, 1f, 0.5f), 0.25f).SetLoops(4, LoopType.Yoyo));
    }
}
