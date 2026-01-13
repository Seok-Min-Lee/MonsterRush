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
    [SerializeField] private GameObject[] playerAbilities;
    [SerializeField] private GameObject[] weaponAbilities;

    [SerializeField] private TextMeshProUGUI character;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI killCount;
    [SerializeField] private TextMeshProUGUI playTime;
    [SerializeField] private TextMeshProUGUI dateTime;

    [SerializeField] private TextMeshProUGUI[] playerStats;
    [SerializeField] private TextMeshProUGUI[] weaponLevels;

    [SerializeField] private Image resultSticker;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Color positiveColor;
    [SerializeField] private Color negativeColor;

    private HistoryRaw raw;

    private Sequence seq;
    public void Init(HistoryRaw raw, Sprite characterIcon, string characterName)
    {
        if (!raw.Equals(default(HistoryRaw)))
        {
            this.raw = raw;

            this.characterIcon.sprite = characterIcon;
            character.text = characterName;
            level.text = raw.level.ToString();
            killCount.text = raw.killCount.ToString("#,##0");
            playTime.text = Utils.FormatTimeTommss(raw.playTime);
            dateTime.text = raw.dateTime;

            for (int i = 0; i < raw.playerStats.Length; i++)
            {
                playerStats[i].text = raw.playerStats[i].ToString();
            }

            for (int i = 0; i < raw.weaponLevels.Length; i++)
            {
                weaponLevels[i].text = raw.weaponLevels[i] == 16 ? "MAX" : raw.weaponLevels[i].ToString();
            }
            //
            for (int i = 0; i < playerAbilities.Length; i++)
            {
                playerAbilities[i].SetActive(false);
            }
            for (int i = 0; i < raw.playerAbilities.Length; i++)
            {
                playerAbilities[raw.playerAbilities[i]].SetActive(true);
            }

            //
            for (int i = 0; i < weaponAbilities.Length; i++)
            {
                weaponAbilities[i].SetActive(false);
            }
            for (int i = 0; i < raw.weaponAbilities.Length; i++)
            {
                weaponAbilities[raw.weaponAbilities[i]].SetActive(true);
            }

            if (raw.level < StaticValues.CHECKPOINT_LEVEL)
            {
                resultSticker.color = negativeColor;
                resultText.color = negativeColor;
                resultText.text = "패배";
            }
            else
            {
                resultSticker.color = positiveColor;
                resultText.color = positiveColor;
                resultText.text = "승리!";
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
    public void Highlight(Color color)
    {
        seq?.Kill();
        seq = DOTween.Sequence();

        seq.AppendCallback(() => highlightMask.color = new Color(color.r, color.g, color.b, 0f));
        seq.Append(highlightMask.DOColor(new Color(color.r, color.g, color.b, 0.5f), 0.25f).SetLoops(4, LoopType.Yoyo));
    }
}
